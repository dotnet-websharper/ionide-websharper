namespace WebSharper.Atom

open System
open System.IO

open Microsoft.FSharp.Compiler

[<AutoOpen>]
module Utilities =
    
    let inline (+.) a b = Path.ChangeExtension(a, b)

    let inline (+/) a b = Path.Combine(a, b)

    let inline (+/+) a b = Seq.fold (+/) a b

    let createDirectory path =
        Directory.CreateDirectory(path) |> ignore
        path

    type MessageType =
        | Info
        | Warning
        | Error
        
        override this.ToString () =
            match this with
            | Info    -> "info"
            | Warning -> "warning"
            | Error   -> "error"

    module String =
        
        let wrap = sprintf "\"%s\""

    type Json =

        static member Stringify mapping =
            Option.fold (fun _ -> mapping) "null"

    type Message =
        | Message of MessageType * string * string option * (int * int) option * string option

    let dispatch (Message (mt, text, file, location, details)) =
        let fn = Json.Stringify (String.wrap << Path.GetFileName) file
        let location =
            location
            |> Json.Stringify (fun (line, column) ->
                sprintf "{\"line\":%d,\"column\":%d}" line column
            )
        let et = text.Replace("\\", "\\\\")
        let details = Json.Stringify id details

        printfn "{\"type\":\"%s\",\"text\":\"%s\",\"file\":%s,\"location\":%s,\"details\":%s}" (string mt) et fn location details

    let exit message code =
        Option.iter dispatch message
        code

    let log (errors: FSharpErrorInfo []) =
        errors
        |> Array.map (fun error ->
            let mt =
                match error.Severity with
                | FSharpErrorSeverity.Error -> Error
                | _ -> Warning

            Message (mt, error.Message, Some error.FileName, Some (error.StartLineAlternate, error.StartColumn), None)
        )
        |> Array.iter dispatch
