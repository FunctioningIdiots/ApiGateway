open HttpClient
open System
open System.Text
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
     //: (HttpContext -> Async<HttpContext option>)
let respond service (context:HttpContext) =
    let pathScanString = new PrintfFormat<_,_,_,_,_>(sprintf "/%s/%s" service.Name "%s")
    let createNewUrl segment = sprintf "http://%s:%u/%s" service.Server service.Port segment
    let urlSegment =
      try 
        let r = Suave.Sscanf.sscanf pathScanString context.request.url.AbsolutePath
        Some r
      with _ -> None
    
    match urlSegment with 
    | None -> Suave.Response.response HTTP_500 (Encoding.UTF8.GetBytes "")
    | Some x -> 
        let url = createNewUrl x
        let httpMethod = 
            match context.request.``method`` with
            | Suave.Http.HttpMethod.POST -> HttpClient.HttpMethod.Post
            | _ -> HttpClient.HttpMethod.Get
        let serviceResponse = createRequest httpMethod url |> getResponse
        let code = 
            match serviceResponse.StatusCode with
            | 100 -> HTTP_100
            | 200 -> HTTP_200
            | 300 -> HTTP_300
            | 404 -> HTTP_404
            | _ -> HTTP_500 // just being lazy right now
        let body = 
            match serviceResponse.EntityBody with
            | None -> Encoding.UTF8.GetBytes ""
            | Some x -> Encoding.UTF8.GetBytes x
        Suave.Response.response code body
       

// reminder: type WebPart = HttpContext -> Async<HttpContext option>

let generateGetWebParts (service:service) : WebPart<'a> = 
    GET >=> pathStarts (sprintf "/%s" service.Name) >=> (respond service)
    
let generatePostWebParts (service:service) : WebPart<'a> = 
    let createPathScanString = new PrintfFormat<_,_,_,_,_>(sprintf "/%s/%s" service.Name "%s")
    let url segment = sprintf "http://%s:%u/%s" service.Server service.Port segment
    POST >=> pathStarts (sprintf "/%s" service.Name) >=> pathScan createPathScanString (fun segment -> OK (url segment))

[<EntryPoint>]
let main argv =     
    let gets = List.map generateGetWebParts services
    let posts = List.map generatePostWebParts services
    let routes = (NOT_FOUND "No handlers found") :: (List.append posts gets)
    let app = choose (List.rev routes)
    startWebServer defaultConfig app
    0