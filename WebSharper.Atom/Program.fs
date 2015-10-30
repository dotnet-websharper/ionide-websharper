namespace WebSharper.Atom

open System
open System.IO
open System.Reflection

open IntelliFactory.Core
open WebSharper

open global.Owin
open Microsoft.Owin.Hosting
open Microsoft.Owin.FileSystems
open Microsoft.Owin.StaticFiles
open WebSharper.Owin

module Program =
    
    let run dll bp root =
        let url = "http://localhost:9000/"
        
        try
            use server = WebApp.Start(url, fun ab ->
                ab.UseStaticFiles(
                    StaticFileOptions(
                        FileSystem = PhysicalFileSystem(root)))
                    .UseDiscoveredSitelet(root, bp) |> ignore)
            
            Message (Info, "Serving sitelet...", None, None, Some ("{\"url\":\"" + url + "\"}"))
            |> dispatch

            Console.ReadLine() |> ignore
            exit None 0
        with
            | _ as error -> exit (Some (Message (Error, error.Message, Some dll, None, None))) 5

    let compile dll assemblies root =
        let ar = AssemblyResolver.Create()
        let loader = Compiler.FrontEnd.Loader.Create ar stderr.WriteLine
        let references =
            assemblies
            |> Seq.map loader.LoadFile
            |> Seq.toList
        let options =
            { Compiler.FrontEnd.Options.Default with
                References = references }
        let compiler = Compiler.FrontEnd.Prepare options (eprintfn "%O")
        let assembly = loader.LoadFile dll

        if compiler.CompileAndModify assembly then
            let target = createDirectory (root +/+ ["bin"; "Debug"])
            
            assembly.Write None (target +/ dll)
            File.Delete(root +/ dll)

            assemblies
            |> Seq.iter (fun ap ->
                let fn = Path.GetFileName(ap)
                
                File.Copy(ap, target +/ fn, true)
            )

            // !!!
            WebSharper.outputFile root assembly (Path.GetFileNameWithoutExtension(dll))
            WebSharper.outputFiles root references

            Message (Info, "... done", None, None, None)
            |> dispatch

            run dll target root
        else
            exit (Some (Message (Error, "Can not compile with WebSharper", Some dll, None, None))) 4
        
    let rec findProjectFile paths =
        paths
        |> Seq.tryPick (fun path ->
            let fp =
                Directory.EnumerateFiles(path)
                |> Seq.tryFind (fun fp -> fp.EndsWith("fsproj"))
            
            match fp with
            | None ->
                Directory.EnumerateDirectories(path)
                |> findProjectFile
            | _ -> fp
        )

    [<EntryPoint>]
    let main arguments = 
        match findProjectFile arguments with
        | Some project ->
            let ((errors, warnings), output, references) = Fcs.compile project
            
            if errors.Length = 0 then
                log warnings

                let ws =
                    references
                    |> List.tryFind (fun path -> path.EndsWith("WebSharper.Core.dll"))
                    |> Option.map Path.GetDirectoryName
                
                match ws with
                | Some path ->
                    let assemblies =
                        path +/+ [".."; ".."; "tools"; "net40"]
                        |> Directory.EnumerateFiles
                        |> Seq.filter (fun fp -> fp.EndsWith("dll"))
                        |> Seq.append references
                        |> Seq.distinctBy (fun fp -> Path.GetFileNameWithoutExtension(fp))
                    
                    AppDomain.CurrentDomain.add_AssemblyResolve(fun _ assembly ->
                        let an = assembly.Name.Substring(0, assembly.Name.IndexOf(','))
                        let path =
                            assemblies
                            |> Seq.tryFind (fun path ->
                                Path.GetFileNameWithoutExtension(path) = an
                            )

                        match path with
                        | Some path ->
                            Assembly.LoadFile(path)
                        | None ->
                            null
                    )

                    let root = Path.GetDirectoryName(project)

                    compile output assemblies root
                | None ->
                    exit (Some (Message (Error, "Can not find WebSharper", Some project, None, None))) 3
            else
                log errors
                log warnings
                exit None 2
        | _ ->
            exit (Some (Message (Error, "Can not find any F# project", None, None, None))) 1
