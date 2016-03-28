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
    let status =
        match selectValue el "./Status" with
        | Some status -> status
        | None -> failwith "Failed to parse status"
    let raceResult =
        match status with
        | "Finished" -> Classified (position, Finish time)
        | "Collision" -> Classified (position, Collision)
        | "Spun off" -> Classified (position, Collision)
        | other -> if other.StartsWith "+"
                   then Classified (position, Finish None)
                   else Unclassified other
    team, driver, raceResult

let raceResults el =
    let rs = select el "/RaceTable/Race/ResultsList/Result"
    seq { for r in rs do
          let t,d,rr = result r
          yield t, (d, rr) }
    |> Seq.groupBy (fun (t,drr) -> t)
    |> Map.ofSeq
    |> Map.map (fun _ s -> s |> Seq.map (fun (t,drr) -> drr))

let yearResults year: Race seq =
    let pattern = sprintf "results_%i_*.xml" year
    let files = Directory.EnumerateFiles("results", pattern)
    seq { for file in files do
          let xml = loadXml file
          let r = round xml
          let n = raceName xml
          let raceResults = raceResults xml
          yield { year = year
                  round = r
                  name = n
                  teamResults = raceResults } }

let scoreA positionA positionB =
    let a = float positionA
    let b = float positionB
    a / (a + b)
    
let driverResults (rs: (Driver * IndividualResult) seq) =
    seq { for (nameA, indResA) in rs do
          for (nameB, indResB) in rs |> Seq.filter (fun (n, ir) -> n <> nameA) do
          let score =
              match indResA, indResB with
              | Classified(pA, _), Classified(pB, _) -> scoreA pA pB |> Some
              | _ -> None
          yield { driver = nameA
                  opponent = nameB
                  score  = score } }

let teamBattleResults (races: Race seq) =
    seq { for race in races do
          for (team, drs) in Map.toSeq race.teamResults do
          // TODO need to pass around the year
          // so we can ensure correct order
          // because rating may not be commutative
          // (Elo isn't)
          yield! driverResults drs }
