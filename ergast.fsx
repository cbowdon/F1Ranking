#load "xpath.fsx"

open System

module Ergast =
    open System.Net
    open XPath

    let api = Uri("http://ergast.com/api/f1/")

    let download (uri: Uri) = 
        let wc = new WebClient()
        wc.DownloadString uri

    let rounds year =
        Uri(api, sprintf "%i" year)
        |> download
        |> XPath.parseXml

    let results year round =
        Uri(api, sprintf "%i/%i/results" year round)
        |> download
        |> XPath.parseXml

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

//results |> Seq.iter (fun (y,r,xml) -> xml.Save(sprintf "results/results_%i_%i.xml" y r))
