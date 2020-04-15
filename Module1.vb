Imports System.Text
' ####################################################################
' # High/Low Card Game: L.Minett 2020                                # 
' #                                                                  #
' # Ideas for improvements:                                          #
' # 1) Make the improvements highlighted with ToDo:                  #
' #    You can go to View -> Other Windows -> Task List to see these #
' # 2) Add sound effects                                             #
' # 3) Improve/optimise code                                         #
' # 4) Easy game could include a hint of the next card's suit        #
' #    (could help if keeping track of cards already used)           #
' # 5) Game switches to lines of 5 cards and user has to guess       #
' #    all correctly to win a line. A user can have a pot of money   #
' #    and can gamble either on winning the entire line, or          #
' #    decides before the line how much to bet and each correct      #
' #    guess doubles the gamble, but a single loss loses all won     #
' #    for that line.                                                #
' # 6) Add other modes for difficulty such as attempt guesses, etc.  #
' #                                                                  #
' ####################################################################

Module Module1
    Enum enDifficulty
        Easy = 1
        Medium = 3
        Hard = 5
    End Enum
    Structure strCard
        Enum EnumSuit ' each represents the unicode symbol for that suit
            Diamonds = &H2666
            Spades = &H2660
            Hearts = &H2665
            Clubs = &H2663
        End Enum
        Enum EnumFace
            Two = 2
            Three = 3
            Four = 4
            Five = 5
            Six = 6
            Seven = 7
            Eight = 8
            Nine = 9
            Ten = 10
            Jack = 11
            Queen = 12
            King = 13
            Ace = 14
        End Enum
        Dim Suit As EnumSuit ' card's suit
        Dim Face As EnumFace ' cards face (e.g. king, three)
    End Structure

    Dim Cardh As Short = 5 ' card height
    Dim Cardw As Short = 7 ' card width
    Sub Main()
        Console.OutputEncoding = Encoding.UTF8
        Console.CursorVisible = False
        Do
            Console.ForegroundColor = ConsoleColor.White
            Console.BackgroundColor = ConsoleColor.Black
            Console.Clear()
            Console.WriteLine("High/Low Card Game" & vbCrLf)

            Console.WriteLine($"1: Easy game using {CInt(enDifficulty.Easy)} deck" & If(CInt(enDifficulty.Easy) = 1, "", "s"))
            Console.WriteLine($"2: Medium game using {CInt(enDifficulty.Medium)} deck" & If(CInt(enDifficulty.Medium) = 1, "", "s"))
            Console.WriteLine($"3: Hard game using {CInt(enDifficulty.Hard)} deck" & If(CInt(enDifficulty.Hard) = 1, "", "s"))
            Console.WriteLine("Q: Quit game")
            Do
                Select Case Console.ReadKey(True).Key
                    Case ConsoleKey.D1
                        Game(enDifficulty.Easy)
                        Exit Do
                    Case ConsoleKey.D2
                        Game(enDifficulty.Medium)
                        Exit Do
                    Case ConsoleKey.D3
                        Game(enDifficulty.Hard)
                        Exit Do
                    Case ConsoleKey.Q
                        End
                End Select

            Loop
        Loop
    End Sub
    Sub Game(Difficulty As enDifficulty)
        ' main game loop
        Console.Clear()
        ' create deck of cards for the game
        Dim Cards As New Queue(Of strCard)(Shuffle(GenerateDeck(CInt(Difficulty)))) ' generate shuffled deck(s) of cards depending on game difficulty

        'Teaching note, the above line could be performed separately as shown below
        '###########################################################################
        '# Dim Cards As New List(Of strCard)(GenerateDeck(CInt(Difficulty))) 
        '# Cards = Shuffle(Cards)
        '# Dim CardDecks As New Queue(Of strCard)(Cards) ' push list into queue

        Dim EndGame As Boolean = False
        Dim CurrentCard As strCard = Cards.Dequeue ' get first card
        Dim CorrectGuesses As Short = 0
        Dim Decision As Boolean = False ' will store the result of the user's assertion

        Do While Not EndGame
            Console.Clear()
            Console.SetCursorPosition(0, 0)
            Console.WriteLine("Correct Guesses:" & CStr(CorrectGuesses).PadLeft(3, " "))
            Console.SetCursorPosition(0, 2)
            Console.WriteLine("The current card is:")
            DrawCard(0, 4, False, CurrentCard)
            DrawCard(Cardw + 5, 4, True)
            Console.SetCursorPosition(0, Cardh + 6)
            Console.WriteLine("Is the new card (h)igher, (l)ower of (e)qual to the current card? (Q to quit game)")
            Do
                Select Case Console.ReadKey(True).Key
                    Case ConsoleKey.H
                        Decision = CInt(Cards.Peek.Face) > CInt(CurrentCard.Face)
                        Exit Do
                    Case ConsoleKey.L
                        Decision = CInt(Cards.Peek.Face) < CInt(CurrentCard.Face)
                        Exit Do
                    Case ConsoleKey.E
                        Decision = CInt(Cards.Peek.Face) = CInt(CurrentCard.Face)
                        Exit Do
                    Case ConsoleKey.Q
                        Exit Sub
                End Select
            Loop
            CurrentCard = Cards.Dequeue ' logic already made, make next card current card
            DrawCard(Cardw + 5, 4, False, CurrentCard)
            Console.SetCursorPosition(0, Cardw + 10)
            If Decision Then
                Console.WriteLine("Well done")
                System.Threading.Thread.Sleep(1000)
                CorrectGuesses += 1
            Else
                Console.WriteLine("Incorrect, you lose.  Sorry! Press any key to continue")
                Console.ReadKey()
                Exit Do
            End If
        Loop

    End Sub
    Sub DrawCard(x As Short, y As Short, Mask As Boolean, Optional card As strCard = Nothing)
        'draws a card at a given location

        'ToDo: Find a way to ensure that if no card is supplied, the game handles it without crashing (see below)
        ' if Mask is true, the outline of a card is shown, but not the actual card
        ' setting Mask to false and not supplying a card will crash

        'ToDo: Optimise the code below
        If Mask Then
            ' draw a blank card with question marks in middle and corners
            Console.BackgroundColor = ConsoleColor.DarkBlue
            Console.ForegroundColor = ConsoleColor.White
            For Line As Integer = y To (y + Cardh) - 1
                Console.SetCursorPosition(x, Line)
                If Line = y Or Line = (y + Cardh) - 1 Then ' write question marks in corners
                    Console.Write("?" & "?".PadLeft(Cardw - 1, " "))
                ElseIf (Math.Ceiling(Cardh / 2) + y) - 1 = Line Then ' middle line
                    Dim middle As Short = Math.Floor(Cardw / 2) 'StrDup cannot take functions as a parameter
                    Console.Write(StrDup(middle, " ") & "?" & StrDup(middle, " "))
                Else
                    Console.Write(StrDup(Cardw, " "))
                End If
            Next

        Else ' enum not set up for reference types, so if no card passed in, it will crash
            ' draw the card
            Console.BackgroundColor = ConsoleColor.White
            Console.ForegroundColor = If(card.Suit = strCard.EnumSuit.Hearts Or card.Suit = strCard.EnumSuit.Diamonds, ConsoleColor.Red, ConsoleColor.Black)
            For Line As Integer = y To (y + Cardh) - 1
                Console.SetCursorPosition(x, Line)
                If Line = y Or Line = (y + Cardh) - 1 Then ' write suit symbol
                    Console.Write(ChrW(CInt(card.Suit)) & CStr(ChrW(CInt(card.Suit))).PadLeft(Cardw - 1, " ")) ' Chrw doesn't support padleft
                ElseIf (Math.Ceiling(Cardh / 2) + y) - 1 = Line Then
                    Dim Cardtxt As String = card.Face.ToString
                    Console.Write(StrDup(Cardw, " "))
                    Console.SetCursorPosition((CInt((Cardw - Cardtxt.Length) / 2) + x), Line)
                    Console.Write(Cardtxt)
                Else
                    Console.Write(StrDup(Cardw, " "))

                End If
            Next
        End If
        'ToDo: Have these a global variables so the game could change
        Console.BackgroundColor = ConsoleColor.Black
        Console.ForegroundColor = ConsoleColor.White
    End Sub


    Function GenerateDeck(NoDecks As Integer) As List(Of strCard)
        ' will generate and pass back a single full deck of cards

        Dim TempDeck As New List(Of strCard)
        Dim TempCard As New strCard

        ' Teaching note: A simple for loop to iterate would break given that the suits are assigned their Unicode symbol value
        ' see the VB companion guide (pg 77) for an example of how this simpler code could work if the suits were unnumbered

        For decks = 1 To NoDecks
            For Each face As strCard.EnumFace In [Enum].GetValues(GetType(strCard.EnumFace))
                For Each suit As strCard.EnumSuit In [Enum].GetValues(GetType(strCard.EnumSuit))
                    TempDeck.Add(New strCard With {.Face = face, .Suit = suit})
                Next
            Next
        Next
        Return TempDeck
    End Function
    Function Shuffle(Deck As List(Of strCard)) As List(Of strCard)
        ' will shuffle a given deck of cards
        ' the algorithm will start at the front and swap each card in series with a random position within the list
        Dim NewDeck As List(Of strCard) = Deck
        Dim Rand As New Random()
        Randomize() ' generate random seed

        For Index = 0 To NewDeck.Count - 1
            Swap(NewDeck(Index), NewDeck(Rand.Next(0, NewDeck.Count)))
        Next

        Return NewDeck
    End Function
    Sub Swap(ByRef c1 As strCard, ByRef c2 As strCard)
        ' will swap the two cards given in the parameters
        Dim tCard As strCard = c1
        c1 = c2
        c2 = tCard
    End Sub
End Module
