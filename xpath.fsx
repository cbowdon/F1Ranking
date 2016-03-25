module XPath

#r "System.Xml.Linq.dll"

open System.Xml
open System.Xml.Linq
open System.Xml.XPath

let parseXml (xml: string) =
    xml.Replace("xmlns", "fuckyouxml")
    |> XElement.Parse

let loadXml (path: string) =
    XElement.Load path

let select (xml: XElement) (query: string) =
    xml.XPathSelectElements query

let selectValue (xml: XElement) (query: string) =
    xml.XPathSelectElement query
    |> Option.ofObj
    |> Option.map (fun o -> o.Value)

let attribute (attr: string) (xml: XElement) =
    xml.Attribute(XName.Get attr)
    |> Option.ofObj
    |> Option.map (fun n -> n.Value)

let selectAttr (xml: XElement) (query: string) (attr: string) =
    xml.XPathSelectElement query
    |> Option.ofObj
    |> Option.bind (attribute attr)

let selectAttrs (xml: XElement) (query: string) (attr: string) =
    select xml query
    |> Seq.filter (fun node -> null <> node)
    |> Seq.map (fun node -> node.Attribute(XName.Get attr))
    |> Seq.filter (fun node -> null <> node)
    |> Seq.map (fun node -> node.Value)
