#load "types.fsx"
#load "xpath.fsx"

open System
open System.IO
open Types
open XPath

let raceName el =
    match selectValue el "/RaceTable/Race/RaceName" with
    | Some x -> x
    | None -> failwith "Failed to parse race name"

let round el =
    match selectAttr el "/RaceTable/Race" "round" with
    | Some x -> Int32.Parse x
    | None -> failwith "Failed to parse round"

let result el =
    let driver =
        match selectAttr el "./Driver" "driverId" with
        | Some x -> x
        | None -> failwith "Failed to parse driver"
    let team =
        match selectAttr el "./Constructor" "constructorId" with
        | Some x -> x
        | None -> failwith "Failed to parse team"
    let position =
        match attribute "position" el with
        | Some x -> Int32.Parse x
        | None -> failwith "Failed to parse position"
    let time =
        selectAttr el "./Time" "millis"
        |> Option.map Double.Parse
        |> Option.map TimeSpan.FromMilliseconds
    { driver = driver
      team = team
      position = position
      time = time }

let results el =
    let rs = select el "/RaceTable/Race/ResultsList/Result"
    seq { for r in rs do
          yield result r }

let yearResults year =
    let pattern = sprintf "results_%i_*.xml" year
    let files = Directory.EnumerateFiles("results", pattern)
    seq { for file in files do
          let xml = loadXml file
          yield { year = year
                  round = round xml
                  name = raceName xml
                  results = results xml } }
