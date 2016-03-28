module Elo

open System

let KFactor = 32.0

let expectedScore (playerA: float) (playerB: float) =
    1.0 / (1.0 + Math.Pow(10.0, (playerA - playerB) / 400.0))

let updateRating (player: float) (expectedScore: float) (actualScore: float) =
    player + KFactor * (actualScore - expectedScore)

module WikiExample =
    // https://en.wikipedia.org/wiki/Elo_rating_system#Theory

    type Player = { name: string
                    rating: float }

    let players = [ { name = "A"; rating = 1613.0 }
                    { name = "B"; rating = 1609.0 }
                    { name = "C"; rating = 1477.0 }
                    { name = "D"; rating = 1388.0 }
                    { name = "E"; rating = 1586.0 }
                    { name = "F"; rating = 1720.0 } ]

    let expectedScores =
        players
        |> Seq.map (fun p -> expectedScore players.[0].rating p.rating)
        |> Seq.zip players
        |> Seq.skip 1 // drop A

    let expectation: float =
        expectedScores
        |> Seq.fold (fun acc (_, s) -> acc + s) 0.0

    let actualScores: float list = [0.0; 0.5; 1.0; 1.0; 0.0]

    let actual: float = Seq.fold (+) 0.0 actualScores

    let updatedRating = updateRating players.[0].rating actual expectation 

    // updatedRating should be 1601
