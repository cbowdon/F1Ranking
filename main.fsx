#load "match.fsx"

open Types
open Match

let races = yearResults 2000

let bats = seq { for race in races do
                 let bs = battles race
                 yield! bs }
      
// TODO fold over battles and get war outcome
             
