        
open System
open Suave
open Suave.Operators
open Suave.Filters
open Suave.Successful
open Suave.RequestErrors

type service = { Name:string; Server:string; Port:uint16 }

let services = 
    [
        {Name="nations"; Server="nationserver"; Port=8001us }
        {Name="states"; Server="stateserver"; Port=8002us }
        {Name="city"; Server="cityserver"; Port=8001us }
    ]

let generateWebPart service = 
    let createPathScanString = new PrintfFormat<_,_,_,_,_>(sprintf "%s/%s" service.Name "%s")
    let url segment = 
        sprintf "http://%s:%u/%s" service.Server service.Port segment
    (GET >=> pathStarts (sprintf "/%s" service.Name) >=> pathScan createPathScanString (fun segment -> OK (url segment))) :> WebPart

[<EntryPoint>]
let main argv =     
    let routes = List.map generateWebPart services 
    let app = choose [ 
                routes
                GET >=> pathStarts "/louisiana" >=> pathScan "%s" (fun segment -> OK (sprintf "http://otherserver:8001/%s" segment))
                GET >=> pathScan "/%s" (fun segment -> OK segment) 
                POST >=> pathScan "/%s" (fun segment -> OK segment) 
                NOT_FOUND "No handlers found"]

    startWebServer defaultConfig app

    0