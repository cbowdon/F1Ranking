module Match

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

let result el: Result =
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

let yearResults year: Race seq =
    let pattern = sprintf "results_%i_*.xml" year
    let files = Directory.EnumerateFiles("results", pattern)
    seq { for file in files do
          let xml = loadXml file
          yield { year = year
                  round = round xml
                  name = raceName xml
                  results = results xml } }

let scoreA resultA resultB =
    resultA.position / (resultA.position + resultB.position)

let teams (race: Race) =
    race.results
    |> Seq.fold (fun map x ->
                let results =
                    match Map.tryFind x.team map with
                    | Some xs -> Set.add x xs
                    | None -> Set.add x Set.empty
                Map.add x.team results map) Map.empty

let otherDriver (driver: Result) (set: Set<Result>) =
    let diff = Set.difference (Set.add driver Set.empty) set
    match Set.toList diff with
    | [ x ] -> Some x
    | _ -> None

let battles (race: Race) =
    let ts = teams race
    seq { for result in race.results do
            let teammates = Map.find result.team ts
            let other = otherDriver result teammates
            match other with
            | None -> do ()
            | Some opponent ->
                let score = scoreA result opponent
                yield { year = race.year
                        round = race.round
                        driver = result.driver
                        opponent = opponent.driver
                        score = score } }
