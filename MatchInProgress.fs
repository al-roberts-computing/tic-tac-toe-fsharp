namespace TicTacToe
module MatchInProgress =

    open System
    open Types

    let checkToken (targetCoord : Coord) (token : Token) (gameBoard : Gameboard) : Result<Coord,GameError> =
        if not(gameBoard.ContainsKey targetCoord) then Error InvalidGridPosition
        elif gameBoard.Item targetCoord <> None then Error CellAlreadyFilled
        else Ok targetCoord

    let swapTurns (players : Players) =
        let (previousTurnPlayer,turnPlayer) = players
        (turnPlayer,previousTurnPlayer)

    let displayVictor (player : Player) = printfn "%A has won!" player.name

    let checkCoordInput (board : Gameboard) (coordinate : string) : Result<Coord,GameError> =
        if coordinate
            |> Seq.forall (fun x -> Char.IsNumber(x) || x = ' ')
            && coordinate.Length = 3
        then
            let splitStr : char array = coordinate |> Seq.toArray
            Coord.create (byte(int(splitStr.[0] |> Char.GetNumericValue))) (byte(int(splitStr.[2] |> Char.GetNumericValue)))
            |> function
                | Some(x) -> if board.Item(x) = None then Ok x else Error CellAlreadyFilled
                | None -> Error InvalidGridPosition
        else Error InvalidGridPosition

    let rec getCoordHuman (board : Gameboard) : Coord =
        printfn "(0,2) | (1,2) | (2,2)"
        printfn "------+----+------"
        printfn "(0,1) | (1,1) | (2,1)"
        printfn "------+----+------"
        printfn "(0,0) | (1,0) | (2,0)"

        match getInfo("Enter coordinate in the form 'x y':") |> checkCoordInput (board) with
            | Ok coord -> coord
            | Error e -> getErrorMsg(e) |> displayError; getCoordHuman board

    let getRandomCell (board : Gameboard) : Coord =
        let possibilities = board |> Map.filter (fun coord cell -> cell = None) |> Map.keys |> Seq.toArray
        possibilities.[(Random().Next(0,possibilities.Length))]

    let decideCoordMedium (c1 : Coord) (c2 : Coord) =
        if Random().Next(0,2) = 0 then c1 else c2

    let getCellSelection (kind : PlayerKind) (board : Gameboard) (turnToken : Token) (opponentToken : Token) : Coord =
        match kind with
            | Human -> getCoordHuman board
            | CPU(Hard) -> MiniMax.findBestMove board turnToken opponentToken
            | CPU(Medium) -> decideCoordMedium (MiniMax.findBestMove board turnToken opponentToken) (getRandomCell (board))
            | CPU(Easy) -> getRandomCell (board)

    let hasWon (board : Gameboard) (turnToken : Token) (opponentToken : Token) : bool =
        match MiniMax.evaluateBoard (board) (turnToken) (opponentToken) with
            | 10 -> true
            | _ -> false

    let displayBoard (board : Gameboard) =
        let display = (
            board
            |> Map.map (fun key value -> match value with Some X -> "X" | Some O -> "O" | None -> " ")
            |> Map.values
            |> Seq.toArray
            )
        printfn " %s | %s | %s " (display.[2]) (display.[5]) (display.[8])
        printfn "---+---+----"
        printfn " %s | %s | %s " (display.[1]) (display.[4]) (display.[7]) 
        printfn "---+---+----"
        printfn " %s | %s | %s " (display.[0]) (display.[3]) (display[6])
        
    let displayWhosTurn (player : Player) = printfn "\nIt is %s's turn" (Name.getName player.name)

    let rec matchInProgress (board : Gameboard) (players : Players) =
        let turnPlayer, opponent = players
        displayWhosTurn (turnPlayer)
        let newBoard = (getCellSelection (turnPlayer.kind) (board) (turnPlayer.token) (opponent.token)
                        |> Gameboard.placeToken (turnPlayer.token) (board))
        displayBoard (newBoard)

        if hasWon (newBoard) (turnPlayer.token) (opponent.token) then printfn "\n%s has won the game!" (Name.getName turnPlayer.name)
        elif MiniMax.isMovesLeft(newBoard) = false then printfn "\nIt is a draw."
        else matchInProgress (newBoard) (swapTurns (turnPlayer,opponent))
