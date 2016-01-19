        
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

let generateGetWebParts (service:service) : WebPart<'a> = 
    let createPathScanString = new PrintfFormat<_,_,_,_,_>(sprintf "/%s/%s" service.Name "%s")
    let url segment = 
        sprintf "http://%s:%u/%s" service.Server service.Port segment
    GET >=> pathStarts (sprintf "/%s" service.Name) >=> pathScan createPathScanString (fun segment -> OK (url segment))
    
let generatePostWebParts (service:service) : WebPart<'a> = 
    let createPathScanString = new PrintfFormat<_,_,_,_,_>(sprintf "/%s/%s" service.Name "%s")
    let url segment = 
        sprintf "http://%s:%u/%s" service.Server service.Port segment
    POST >=> pathStarts (sprintf "/%s" service.Name) >=> pathScan createPathScanString (fun segment -> OK (url segment))

[<EntryPoint>]
let main argv =     
    let gets = List.map generateGetWebParts services
    let posts = List.map generatePostWebParts services
    let routes = (NOT_FOUND "No handlers found") :: (List.append posts gets)
    let app = choose (List.rev routes)
    startWebServer defaultConfig app
    0