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

type Player = { name: string
                rating: float }

// battles/wars in the sense of matches/tournament
type Battle = { year: int
                round: int
                driver: string
                opponent: string
                score: int }

type War = { year: int
             driver: string
             opponent: string
             score: int }
