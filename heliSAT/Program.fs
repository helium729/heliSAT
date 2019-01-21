open System.Threading

// Learn more about F# at http://fsharp.org

open System
open System.IO
open System.Collections.Generic

let readFile (path:String) =
    let reader = new StreamReader(path)
    let list = new List<String>()
    list.Add(reader.ReadLine())
    list

let listToDict (list:List<String>) =
    
    0

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    0 // return an integer exit code