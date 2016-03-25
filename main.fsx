#load "types.fsx"
#load "xpath.fsx"

open System
open System.IO
open Types
open XPath

let raceName el =
    selectValue el "/RaceTable/Race/RaceName"

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
    Directory.EnumerateFiles("results", sprintf "results_%i_*.xml" year)
    |> Seq.map loadXml
    |> Seq.map results
    |> Seq.cache

let y2k = yearResults 2000;;    
