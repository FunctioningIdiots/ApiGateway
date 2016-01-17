namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("ApiGateway")>]
[<assembly: AssemblyProductAttribute("ApiGateway")>]
[<assembly: AssemblyDescriptionAttribute("An API Gateway")>]
[<assembly: AssemblyVersionAttribute("0.0.1")>]
[<assembly: AssemblyFileVersionAttribute("0.0.1")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "0.0.1"
