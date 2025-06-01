namespace TicTacToe
module Types =
    open System

    (* --- Types for the gameboard ---*)
    type Token = X | O
    type Cell = option<Token>
    type Coord = private Coordinate of byte*byte
    type Gameboard = Map<Coord,Cell>

    (* --- Type for CPU difficulty --- *)
    type CPUDifficulty = Easy | Medium | Hard
    type GameMode = SinglePlayer | TwoPlayer

    (* --- Type for game errors ---*)
    type GameError = 
        | InvalidGameMode
        | InvalidGridPosition
        | CellAlreadyFilled
        | InvalidToken
        | InvalidNameSpecChar
        | InvalidNameLength
        | InvalidDifficulty
        | InvalidKind

    (* --- Types for the player and scoreboard ---*)
    type Name = private ProposedName of string

    type PlayerKind = Human | CPU of CPUDifficulty

    type Player = {name : Name; token : Token; kind : PlayerKind}

    type Players = Player*Player

    (* --- modules for types --- *)

    module Coord =
        let create (x : byte) (y : byte) : Coord option =
            if (x <= 2uy) && (y <= 2uy) then
                Some (Coordinate (x,y))
            else
                None

        let getX (Coordinate (x,y)) : byte = x

        let getY (Coordinate(x,y)) : byte = y

    module Name =
        let create (name : string) : Name option =
            if name.Length <= 3 || name.Length > 30 then None
            else if name
                |> Seq.forall (fun x -> Char.IsLetter(x) || Char.IsNumber(x))
                |> not
                then None
            else Some(ProposedName(name))

        let getName (name : Name) : string =
            match name with ProposedName(name) -> name

        let checkNameArray (args : string array) : Result<string array,GameError> =
            let name : string = args[0]
            if name.Length <= 3 || name.Length > 30 then Error InvalidNameLength
            else if name
                |> Seq.forall (fun x -> Char.IsLetter(x) || Char.IsNumber(x))
                |> not
                then Error InvalidNameSpecChar
            else Ok args

    module Gameboard =
        let create (newGameboard : Map<Coord,Cell>) : Gameboard option =
            if newGameboard.Count = 9 then Some(newGameboard)
            else None

        let placeToken (turnToken : Token) (gameBoard : Gameboard) (targetCoord : Coord) : Gameboard =
            gameBoard |> Map.change targetCoord (function
                                                    | Some(x) -> Some(Some(turnToken))
                                                    | None -> None
                                                )

    let getInfo (prompt : string) : string =
        printfn "%s" prompt
        Console.ReadLine()

    let getErrorMsg (error : GameError) : string =
        match error with
            | InvalidGameMode -> "ERROR: the game mode is not valid."
            | InvalidNameLength -> "ERROR: the name must be between 4 and 30 characters long."
            | InvalidNameSpecChar -> "ERROR: the name can only contain English letters and numbers."
            | InvalidToken -> "ERROR: invalid token."
            | InvalidGridPosition -> "ERROR: invalid grid position."
            | CellAlreadyFilled -> "ERROR: the cell is already filled."
            | InvalidDifficulty -> "ERROR: the difficulty is not valid."
            | _ -> "ERROR: unknown error."

    let displayError (error : string) =
        printfn "\n%s\n" error

            