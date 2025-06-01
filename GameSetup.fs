namespace TicTacToe
module GameSetup =
    open System
    open Types

    module InfoChecks =
        let checkToken (args : string array) : Result<string array,GameError> =
            if args.[1] = "X" || args.[1] = "x" || args.[1] = "O" || args.[1] = "o" then Ok args
            else Error InvalidToken

        let checkMode (args : string array) : Result<string array,GameError> =
            if args.[4] = "easy" || args.[4] = "hard" then Ok args
            else Error InvalidDifficulty

        let checkKind (args : string array) : Result<string array, GameError> =
            if args.[3] = "yes" || args.[3] = "no" then Ok args
            else Error InvalidKind

        let checkCpuDiff (diff : string) : Result<CPUDifficulty, GameError> =
            match diff with
                | "easy" -> Ok Easy
                | "medium" -> Ok Medium
                | "hard" -> Ok Hard
                | _ -> Error InvalidDifficulty

        let checkCpuDiffArray (args : string array) : Result<string array, GameError> =
            match args.[4] with
                | "easy" -> Ok args
                | "medium" -> Ok args
                | "hard" -> Ok args
                | "" -> Ok args
                | _ -> Error InvalidDifficulty

    let getInfoStr() : Result<string array,GameError> =
        printfn "Please enter your name:"
        let name : string = Console.ReadLine()

        printfn "Please enter what token you would like (X or O):"
        let token = Console.ReadLine()

        printfn "Would you like to player single- or two-player:"
        let mode = Console.ReadLine()

        printfn "Would you like the computer to play for you (yes or no)?:"
        let kind = Console.ReadLine()

        printfn "If you would like the computer to play for you, please enter the difficulty (easy, medium, hard), otherwise please leave this field blank:"
        let diff = Console.ReadLine()

        Ok [|name;token;mode;kind;diff|]

    module MatchExpressions =
        let matchToken (token : string) : Token option =
            match token with
                | "X" -> Some X
                | "x" -> Some X
                | "O" -> Some O
                | "o"-> Some O
                | _ -> None

        let matchKind (kind : string) (diff : string) : PlayerKind option =
            match kind with
                | "no" -> Some Human
                | "yes" when diff = "easy" -> Some(CPU(Easy))
                | "yes" when diff = "medium" -> Some(CPU(Medium))
                | "yes" when diff = "hard" -> Some(CPU(Hard))
                | _ -> None

        let matchMode (mode : string) : GameMode option =
            match mode with
                | "single" -> Some SinglePlayer
                | "two" -> Some TwoPlayer
                | _ -> None

    open InfoChecks
    open MatchExpressions

    let validatePlayer (request : Result<string array,GameError>) : Result<string array,GameError> =
        request
        |> Result.bind Name.checkNameArray
        |> Result.bind checkToken
        |> Result.bind checkMode
        |> Result.bind checkKind
        |> Result.bind checkCpuDiffArray

    let convertPlayerOneInfo (args : string array) : Player*GameMode =
        ({name = (Name.create args.[0] |> Option.get);
        token = matchToken(args.[1]) |> Option.get;
        kind = (matchKind(args.[3]) (args.[4])) |> Option.get}, (matchMode (args.[2])) |> Option.get)

    let rec getPlayerOneInfo() : Player*GameMode =
        let req = getInfoStr() |> validatePlayer
        if Result.isOk req then convertPlayerOneInfo ((Result.toOption req) |> Option.get)
        else
            req |> (function Error e -> Some(e) | Ok k -> None) |> Option.get |> getErrorMsg |> displayError
            getPlayerOneInfo()

    let rec getPlayerTwoInfo (p1Info : Player*GameMode) : Players =
        let player : Player = fst p1Info
        let mode : GameMode = snd p1Info
        let player2Token : Token = if player.token = X then O else X

        if mode = SinglePlayer then
            let diff = getInfo("Please enter the difficulty you would like the CPU to be (easy, medium, hard):") |> checkCpuDiff
            if Result.isOk diff then
                let player2 : Player = {name=Name.create("Computer") |> Option.get;
                                        token=player2Token;
                                        kind = CPU(
                                                (Result.toOption(diff) |> Option.get)
                                            )
                                        }

                if player2Token = X then (player2,player)
                else (player,player2)
            else
                diff |> (function Error e -> Some(e) | Ok k -> None) |> Option.get |> getErrorMsg |> displayError
                getPlayerTwoInfo (player,mode)
        else
            let p2Name = [|getInfo("Player 2, please enter your name:")|] |> Name.checkNameArray
            if Result.isOk p2Name then
                let player2 : Player = {name=Name.create((Result.toOption(p2Name) |> Option.get).[0]) |> Option.get ;token=player2Token;kind=Human}

                if player2Token = X then (player2,player)
                else (player,player2)
            else
                p2Name |> (function Error e -> Some(e) | Ok k -> None) |> Option.get |> getErrorMsg |> displayError
                getPlayerTwoInfo (player,mode)

    let gameSetup() : Players =
        getPlayerOneInfo() |> getPlayerTwoInfo
