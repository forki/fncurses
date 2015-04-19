﻿open Fncurses.Core

module Example =

    let helloworld () =
        ncurses {
            do! "Greetings from fncurses!".ToCharArray() 
                |> NcursesArray.iter (fun ch ->
                    ncurses {
                        do! addch ch
                        do! refresh ()
                        do! napms 100s
                    })
        }

    let stringinputoutput () =
        ncurses {
            let message ="Enter a string: "
            let! row,col = getmaxyx <| stdscr ()
            let y = row / 2s
            let x = (col - int16 message.Length) / 2s
            do! mvprintw y x "%s" message
            let! str = getstr ()
            do! mvprintw (LINES () - 2s) 0s "You Entered: %s" str
        }

    let annoy () =
        ncurses {
            let text = [|"Do"; "you"; "find"; "this"; "silly?"|]
            do! [|0 .. 4|]
                |> NcursesArray.iter (fun a ->
                    ncurses {
                        do! [|0 .. 4|]
                            |> NcursesArray.iter (fun b ->
                                ncurses {
                                    if b = a then do! attrset (A_BOLD ||| A_UNDERLINE)
                                    do! printw "%s" text.[b]
                                    if b = a then do! attroff (A_BOLD ||| A_UNDERLINE)
                                    do! addch ' '
                                })
                        do! addstr "\b\n"
                    })
            do! refresh ()
        }

// runners

let run f =
    ncurses {
        let! win = initscr ()
        do! f ()
        let! ch = wgetch win
        return! endwin ()
    }

//    let runLoop f =
//        result {
//            let! win = initscr ()
//            do! keypad win true
//            let rec loop() =
//                result {
//                    let! ch = getch ()
//                    do! f ch
//                    if ch <> int '\n' then return! loop ()
//                }
//            do! loop ()
//            return! endwin ()
//        }

// TODO: set the environment variables when initscr is called or just use getter per variable?

[<EntryPoint>]
let main argv =
    match run Example.annoy with
    | Success _ -> 
        0
    | Failure reason -> printfn "%s" reason; 1
