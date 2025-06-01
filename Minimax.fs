namespace TicTacToe
module MiniMax =
    open System
    open Types

    let isMovesLeft (board : Gameboard) : bool = board.Values.Contains None

    let evaluateBoard (board : Gameboard) (turnToken : Token) (opponentToken : Token) : int =
        board |> Map.values |> Seq.toArray
        |> function
            | [|a;b;c;_;_;_;_;_;_|] when a = Some(turnToken) && b = Some(turnToken) && c = Some(turnToken) -> 10
            | [|_;_;_;a;b;c;_;_;_|] when a = Some(turnToken) && b = Some(turnToken) && c = Some(turnToken) -> 10
            | [|_;_;_;_;_;_;a;b;c|] when a = Some(turnToken) && b = Some(turnToken) && c = Some(turnToken) -> 10
            | [|a;_;_;b;_;_;c;_;_|] when a = Some(turnToken) && b = Some(turnToken) && c = Some(turnToken) -> 10
            | [|_;a;_;_;b;_;_;c;_|] when a = Some(turnToken) && b = Some(turnToken) && c = Some(turnToken) -> 10
            | [|_;_;a;_;_;b;_;_;c|] when a = Some(turnToken) && b = Some(turnToken) && c = Some(turnToken) -> 10
            | [|a;_;_;_;b;_;_;_;c|] when a = Some(turnToken) && b = Some(turnToken) && c = Some(turnToken) -> 10
            | [|_;_;a;_;b;_;c;_;_|] when a = Some(turnToken) && b = Some(turnToken) && c = Some(turnToken) -> 10
            | [|a;b;c;_;_;_;_;_;_|] when a = Some(opponentToken) && b = Some(opponentToken) && c = Some(opponentToken) -> -10
            | [|_;_;_;a;b;c;_;_;_|] when a = Some(opponentToken) && b = Some(opponentToken) && c = Some(opponentToken) -> -10
            | [|_;_;_;_;_;_;a;b;c|] when a = Some(opponentToken) && b = Some(opponentToken) && c = Some(opponentToken) -> -10
            | [|a;_;_;b;_;_;c;_;_|] when a = Some(opponentToken) && b = Some(opponentToken) && c = Some(opponentToken) -> -10
            | [|_;a;_;_;b;_;_;c;_|] when a = Some(opponentToken) && b = Some(opponentToken) && c = Some(opponentToken) -> -10
            | [|_;_;a;_;_;b;_;_;c|] when a = Some(opponentToken) && b = Some(opponentToken) && c = Some(opponentToken) -> -10
            | [|a;_;_;_;b;_;_;_;c|] when a = Some(opponentToken) && b = Some(opponentToken) && c = Some(opponentToken) -> -10
            | [|_;_;a;_;b;_;c;_;_|] when a = Some(opponentToken) && b = Some(opponentToken) && c = Some(opponentToken) -> -10
            | _ -> 0

    let rec minimax (depth : int) (isMaximizingPlayer : bool) (turnToken : Token) (opponentToken : Token) (board : Gameboard) : int =
        let score : int = evaluateBoard (board) (turnToken) (opponentToken)
        let movesLeft = isMovesLeft (board)

        if (score = 0 && movesLeft) then
            let mutable best = 0
            let mutable tokenToChange : Token = turnToken
            if isMaximizingPlayer then
                best <- -100
            else
                tokenToChange <- opponentToken
                best <- 100
            for pair in board do
                if pair.Value = None then
                    let newBest : int = (board |> Map.change pair.Key (function Some(x) -> Some(Some(tokenToChange)) | None -> None)
                        |> minimax (depth+1) (not(isMaximizingPlayer)) (turnToken) (opponentToken))
                    if isMaximizingPlayer then
                        best <- (max (best) (newBest))
                    else
                        best <- (min (best) (newBest))
            best
        elif score <> 0 then score
        else 0

    let findBestMove (board : Gameboard) (turnToken : Token) (opponentToken : Token): Coord =
        let mutable bestVal : int = -100
        let mutable moveVal : int = bestVal
        let mutable bestMove : Coord = (Coord.create 0uy 0uy) |> Option.get
        for pair in board do
            if pair.Value = None then
                moveVal <- ((board |> Map.change pair.Key (function Some(x) -> Some(Some(turnToken)) | None -> None) |> minimax (0) (false) (turnToken) (opponentToken)))
                if (moveVal > bestVal) then
                    bestMove <- pair.Key
                    bestVal <- moveVal
        bestMove