#r "System.Xml.Linq.dll"

open System
open System.Xml.Linq
open System.Xml
open System.Xml.XPath

module Ergast =
    open System
    open System.Net

    let api = Uri("http://ergast.com/api/f1/")

    let download (uri: Uri) = 
        let wc = new WebClient()
        wc.DownloadString uri

    let parseXml (xml: string) =
        xml.Replace("xmlns", "fuckyouxml")
        |> XElement.Parse
        |> XDocument

    let rounds year =
        Uri(api, sprintf "%i" year)
        |> download
        |> parseXml

    let results year round =
        Uri(api, sprintf "%i/%i/results" year round)
        |> download
        |> parseXml

module XPath = 
    let select (xml: XDocument) (query: string) =
        xml.XPathSelectElements query

    let selectAttrs (xml: XDocument) (query: string) (attr: string) =
        select xml query
        |> Seq.map (fun node -> node.Attribute(XName.Get attr))
        |> Seq.filter (fun node -> null <> node)
        |> Seq.map (fun node -> node.Value)

let years = Seq.init 16 (fun i -> 2000 + i)

let rounds = seq { for year in years do
                   let races = Ergast.rounds year
                   let rounds = XPath.selectAttrs races "//Race" "round"
                                |> Seq.map Int32.Parse
                   yield year, rounds }

let results = seq { for y,rs in rounds do
                    for r in rs do
                    let results = Ergast.results y r
                    yield y, r, results }

results
|> Seq.iter (fun (y,r,xml) -> xml.Save(sprintf "results/results_%i_%i.xml" y r))
