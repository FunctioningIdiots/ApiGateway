namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("ApiGateway")>]
[<assembly: AssemblyProductAttribute("ApiGateway")>]
[<assembly: AssemblyDescriptionAttribute("An API Gateway")>]
[<assembly: AssemblyVersionAttribute("1.0")>]
[<assembly: AssemblyFileVersionAttribute("1.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.0"
