#load "match.fsx"
#load "elo.fsx"

open Types
open Match
open Elo

let initialScore = 200.0

let drivers =
    allDrivers
    |> Seq.map (fun d -> d, initialScore)
    |> Map.ofSeq

let races = yearResults 2000
let trs = teamBattleResults races

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
    let driverRating = Map.find t.driver drivers
    let opponentRating = Map.find t.opponent drivers
    let expected = expectedScore driverRating opponentRating
    let actual = t.totalScore / (float t.rounds)
    let newRating = updateRating driverRating expected actual
    Map.add t.driver newRating drivers
    
let updatedDrivers =
    trs
    |> seasonResults
    |> Seq.fold updateDriver drivers

// expect Hakkinen to be on 202.05
// and Coulthard on 197.79
// after the 2000 season
