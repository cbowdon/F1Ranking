module XPath

#r "System.Xml.Linq.dll"

open System.Xml
open System.Xml.Linq
open System.Xml.XPath

let parseXml (xml: string) =
    xml.Replace("xmlns", "fuckyouxml")
    |> XElement.Parse
    |> XDocument

let select (xml: XDocument) (query: string) =
    xml.XPathSelectElements query

let selectAttrs (xml: XDocument) (query: string) (attr: string) =
    select xml query
    |> Seq.map (fun node -> node.Attribute(XName.Get attr))
    |> Seq.filter (fun node -> null <> node)
    |> Seq.map (fun node -> node.Value)
