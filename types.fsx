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

type Battle = { team: string
                results: Result * Result }

type War = { year: int
             driver: string
             opponent: string
             score: int }
