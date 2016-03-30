#load "match.fsx"
#load "elo.fsx"

open System.IO
open Types
open Match
open Elo

let initialScore = 200.0

let drivers =
    allDrivers
    |> Seq.map (fun d -> d, initialScore)
    |> Map.ofSeq

let totalScoreRounds (trs: TeamBattleResult seq) =
    let scores = seq { for t in trs do
                       match t.score with
                       | Some score -> yield score
                       | None -> do () }
    Seq.fold (+) 0.0 scores, Seq.length scores

let seasonResults (trs: TeamBattleResult seq) =
    trs
    |> Seq.groupBy (fun t -> t.driver, t.opponent)
    |> Seq.map (fun ((d,o),v) ->
                let total,rnds = totalScoreRounds v
                { driver = d
                  opponent = o
                  rounds = rnds
                  totalScore = total })

let updateDriver (drivers: Map<Driver, float>) (t: SeasonResult) =
    if t.rounds = 0 then
        drivers
    else
        let driverRating = Map.find t.driver drivers
        let opponentRating = Map.find t.opponent drivers
        let expected = expectedScore driverRating opponentRating
        // TODO perhaps some weighting so 1 or 2 races
        // against temp drivers don't have a big effect
        let actual = t.totalScore / (float t.rounds)
        let newRating = updateRating driverRating expected actual
        Map.add t.driver newRating drivers

let sprintDrivers (drivers: Map<Driver, float>) =
    drivers
    |> Map.toSeq
    |> Seq.map fst
    |> Seq.sort
    |> String.concat "\t"

let sprintScores (drivers: Map<Driver, float>) = 
    drivers
    |> Map.toSeq
    |> Seq.sortBy fst
    |> Seq.map (snd >> string)
    |> String.concat "\t"

let dataFile = "scores.tsv"

let appendLine file line =
    File.AppendAllText(file, sprintf "%s\n" line)
    
let updateDrivers (drivers: Map<Driver, float>) (trs: TeamBattleResult seq) =
    let updatedDrivers =
        trs
        |> seasonResults
        |> Seq.fold updateDriver drivers
    
    appendLine dataFile (sprintScores updatedDrivers)

    updatedDrivers

// // Test:
// // after the 2000 season
// // expect Hakkinen to be on 202.05
// // and Coulthard on 197.79
// // yes this should be a unit test. what you gonna do?
// let races = yearResults 2000
// let trs = teamBattleResults races
// let y2kResult = updateDrivers drivers trs

// TODO let's go to when schumi started
let years = Seq.init 16 ((+) 2000)

File.Delete dataFile
appendLine dataFile (sprintDrivers drivers)

let finalResults =
    years
    |> Seq.map yearResults
    |> Seq.map teamBattleResults
    |> Seq.fold updateDrivers drivers
