module Types

open System

type Result = { driver: string
                team: string
                position: int
                time: TimeSpan option }

type Race = { year: int
              round: int
              name: string
              results: Result seq }
