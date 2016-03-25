module Match

#load "types.fsx"

open Types

// in this module we are aiming to
// get the results of team mate battles / yr

let battles (race: Race) =
    race.results
    |> Seq.groupBy (fun r -> r.team)
    |> Seq.filter (fun (k, v) -> Seq.length v <> 2)
    |> Seq.map (fun (k, v) -> { team = k
                                results = Seq.head v, Seq.last v })

let scoreA resultA resultB =
    resultA.position / (resultA.position + resultB.position)
