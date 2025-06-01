namespace TicTacToe
module Main =
    open System
    open Types
    open GameSetup
    open MatchInProgress

    [<EntryPoint>]
    let main args =
        let board = (Gameboard.create Map[
            (Coord.create(0uy)(0uy)).Value,None;
            (Coord.create(1uy)(0uy)).Value,None;
            (Coord.create(2uy)(0uy)).Value,None;
            (Coord.create(0uy)(1uy)).Value,None;
            (Coord.create(1uy)(1uy)).Value,None;
            (Coord.create(2uy)(1uy)).Value,None;
            (Coord.create(0uy)(2uy)).Value,None;
            (Coord.create(1uy)(2uy)).Value,None;
            (Coord.create(2uy)(2uy)).Value,None]) |> Option.get

        printfn "------ Welcome to Tic-Tac-Toe! ------"
        
        gameSetup() |> matchInProgress (board)

        //MiniMax.evaluateBoard (board) (X) (O) |> printfn "%d"

        0