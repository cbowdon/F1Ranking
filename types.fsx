module Types

type Team = string

type Driver = string

type Position = int

type Classification =
    // Time is in millis, not TimeSpan
    // because prints horribly in REPl!
    | Finish of double option
    // Collision are considered driver's own fault
    // because serial victims are rarer than
    // serial crashers
    | Collision

type IndividualResult =
    | Classified of Position * Classification
    // A genuine DNF will mean no result
    | Unclassified of string

type TeamBattleResult = { driver: Driver
                          opponent: Driver
                          score: float option }

type Race = { year: int
              round: int
              name: string
              teamResults: Map<Team, (Driver * IndividualResult) seq> }

type Player = { name: string
                rating: float }
