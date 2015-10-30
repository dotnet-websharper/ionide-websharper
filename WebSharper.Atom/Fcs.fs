namespace WebSharper.Atom

open System.IO
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.SimpleSourceCodeServices

module Fcs =

    let parseProject project =
        let info = FSharpProjectFileInfo.Parse(project)
        let output =
            match info.OutputFile with
            | Some path ->
                Path.GetFileName(path)
            | None -> 
                Path.GetFileNameWithoutExtension(project)

        (info.References, info.CompileFiles, output +. "dll")

    let private service = SimpleSourceCodeServices()

    let compile project =
        let (references, files, output) = parseProject project
        let ep = project.Replace("\\", "\\\\")

        Message (Info, "Compiling... (" + project + ")", None, None, None)
        |> dispatch

        Directory.SetCurrentDirectory(Path.GetDirectoryName(project))

        let (messages, _) =
            [|
                yield! references
                    |> List.map (fun path ->
                        "--reference:" + path
                    )
                yield "--out:" + output
                yield "--target:library"
                yield "--noframework"
                yield! files
            |]
            |> service.Compile
        
        let split (messages: FSharpErrorInfo []) =
            messages
            |> Array.partition (fun error ->
                match error.Severity with
                | FSharpErrorSeverity.Error ->
                    true
                | _ ->
                    false
            )

        (split messages, output, references)
