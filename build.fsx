#load "tools/includes.fsx"

open System.IO
open IntelliFactory.Build

let bt =
    BuildTool().PackageId("WebSharper.Atom")
        .VersionFrom("WebSharper")
        .WithFramework(fun fw -> fw.Net45)
        .WithFSharpVersion(FSharp31)

let main =
    bt.FSharp.ConsoleExecutable("WebSharper.Atom")
        .SourcesFromProject()
        .References(fun ref ->
            [
                ref.NuGet("FSharp.Compiler.Service").Version("(,99.0.0)").Reference()
                ref.NuGet("WebSharper.Compiler").Reference()
                ref.NuGet("Microsoft.Owin.SelfHost").Reference()
                ref.NuGet("Microsoft.Owin.StaticFiles").Reference()
                ref.NuGet("WebSharper.Owin").Reference()
                ref.Assembly("System.Web")
            ])

bt.Solution [
    main
]
|> bt.Dispatch

let inline (+/) a b = Path.Combine(a, b)
let target = "ionide-websharper" +/ "binaries"

Directory.CreateDirectory(target)

[
    "FSharp.Compiler.Service.dll"
    "HttpMultipartParser.dll"
    "Microsoft.Owin.Diagnostics.dll"
    "Microsoft.Owin.dll"
    "Microsoft.Owin.FileSystems.dll"
    "Microsoft.Owin.Host.HttpListener.dll"
    "Microsoft.Owin.Hosting.dll"
    "Microsoft.Owin.StaticFiles.dll"
    "Owin.dll"
    "WebSharper.Atom.exe"
    "WebSharper.Owin.dll"
]
|> List.iter (fun fn ->
    File.Copy("build" +/ "net45" +/ fn, target +/ fn, true)
)

File.Copy("WebSharper.Atom" +/ "App.config", target +/ "WebSharper.Atom.exe.config", true)
