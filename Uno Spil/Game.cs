using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Uno_Spil
{
    class Game
    {
        public List<Player> Players { get; private set; }
        public Player PlayingPlayer { get; set; }
        public bool GameRunning { get; set; }
        public Stack<Card> MainStack { get; private set; }
        public Stack<Card> TableStack { get; private set; }
        public bool CardMoveValidated { get; private set; }
        public bool NextPlayerTakeCard { get; private set; }
        public int CardToTakeCounter { get; private set; }

        // Create a new game and deal card to players...
        public void NewGame(int PlayersToPlay)
        {
            Players = new List<Player>();
            TableStack = new Stack<Card>();
            MainStack = CardStack.Stack();

            // Ask to play a computer player...
            Console.Clear();
            Console.WriteLine("Spil mod computer? (j/n)");

            // If yes? Add computer player
            if (Console.ReadKey().Key == ConsoleKey.J)
            {
                Player player = new Player("Computer");
                Players.Add(player);
                PlayersToPlay--;
            }

            // Add human players...
            for (int i = 0; i < PlayersToPlay; i++)
            {
                // Clear console and ask for new player name...
                Console.Clear();
                Console.WriteLine("Angiv nyt spiller navn: ");

                // Create input string...
                string newPlayer = Console.ReadLine();

                // Check if the input is valid...
                while (!Regex.IsMatch(newPlayer, "[a-åA-Å0-9]"))
                {
                    Console.WriteLine("Hov prøv igen... Accepteret er store og små bogstaver, samt tal 1 - 9.");
                    newPlayer = Console.ReadLine();
                }

                // Create the new player and add to players list...
                Player player = new Player(newPlayer);
                Players.Add(player);
            }

            // Create new Dice and DiceRoll int list...
            Dice dice = new Dice();
            List<int> playerDiceRollList = new List<int>();

            // Who starts?
            Console.Clear();
            Console.WriteLine("Terningen er kastet, så hvem starter?\n\n");
            System.Threading.Thread.Sleep(1000);

            // Roll the dice...
            foreach (Player player in Players)
            {
                player.DiceRoll = dice.Roll();
                playerDiceRollList.Add(player.DiceRoll);
                Console.WriteLine("{0} slog en {1}", player.PlayerName, player.DiceRoll);
                System.Threading.Thread.Sleep(2000);
            }

            // Find player to start...
            int StartingPlayerIndex = 0;

            foreach (Player player in Players)
            {
                if (player.DiceRoll == playerDiceRollList.Max())
                {
                    StartingPlayerIndex = Players.IndexOf(player);
                }
            }

            Console.WriteLine("\n\n{0} Starter...", Players.ElementAt(StartingPlayerIndex).PlayerName);
            System.Threading.Thread.Sleep(3000);

            // Give card to players...
            for (int i = 0; i < 7; i++)
            {
                foreach (Player GivePlayerCard in Players)
                {
                    GivePlayerCard.PlayerHand.Add(MainStack.Pop());
                }
            }

            // Deal firs card to table and start the game...
            TableStack.Push(MainStack.Pop());
            GameRunning = true;
            PlayGame(StartingPlayerIndex);
        }

        // Play game...
        public void PlayGame(int PlayerToStart)
        {
            // Declare varible to use in playgame...
            int CardChosen = 0;

            // Set wich player to start...
            PlayingPlayer = Players.ElementAt(PlayerToStart);

            // Check if first player should change color...
            if (TableStack.Peek().SpecialCard == Card.CardSpecial.ChangeColor || TableStack.Peek().SpecialCard == Card.CardSpecial.TakeFour)
            {
                // Print color choise menu...
                PrintChoseColorMenu(TableStack.Peek().SpecialCard.ToString());

                // Create temp card before changing color...
                Card tempCard = TableStack.Pop();

                // Declare int for color choise...
                int choise = 0;

                // Play as computer player...
                if (PlayingPlayer.PlayerName == "Computer")
                {
                    // Computer makes a color choise...
                    choise = ComputerPlayer(true);
                }

                // Play as human player...
                else
                {
                    // Create input string and read input...
                    string input = Console.ReadLine();

                    // Validate input and try again on fail...
                    while (!Regex.IsMatch(input, "[1-4]"))
                    {
                        Console.WriteLine("Hov prøv igen...");
                        input = Console.ReadLine();
                    }

                    // Parse validated input to int...
                    int.TryParse(input, out choise);
                }

                // Change the card color based on the choise...
                tempCard.Color = (Card.CardColor)choise;  // Chosing color from color index number..

                // Put card back in the stack...
                TableStack.Push(tempCard);
            }

            // Create gamedirection instance and set to clockwise...
            GameDirection Direction = new GameDirection(Players.Count)
            {
                DirectionClockwise = true
            };

            // Start game rotation...
            while (GameRunning == true)
            {
                while (CardMoveValidated == false)
                {
                    if (NextPlayerTakeCard == true)
                    {
                        bool Contains = false;

                        if (TableStack.Peek().SpecialCard == Card.CardSpecial.TakeFour)
                        {
                            CardToTakeCounter += 4;
                        }
                        else if (TableStack.Peek().SpecialCard == Card.CardSpecial.TakeTwo)
                        {
                            CardToTakeCounter += 2;
                        }

                        // Check if player have a +2 or +4 to play...
                        foreach (Card card in PlayingPlayer.PlayerHand)
                        {
                            if (card.SpecialCard == Card.CardSpecial.TakeFour && TableStack.Peek().SpecialCard == Card.CardSpecial.TakeFour)
                            {
                                Contains = true;
                            }
                            else if (card.SpecialCard == Card.CardSpecial.TakeTwo && TableStack.Peek().SpecialCard == Card.CardSpecial.TakeTwo)
                            {
                                Contains = true;
                            }
                            else
                            {
                                Contains = false;
                            }
                        }

                        // Clear console and set text...
                        Console.Clear();
                        Console.WriteLine("Det er {0}´s tur.\n\n", PlayingPlayer.PlayerName);
                        Console.WriteLine("Der er blevet lagt en {1}\n\n", PlayingPlayer.PlayerName, TableStack.Peek());
                        Console.ResetColor();

                        // Print players choice screen...
                        if (Contains == true)
                        {
                            // Print players turn and hand...
                            ShowPlayersTurn(PlayingPlayer);

                            // Create an validator...
                            bool CardAccepted = false;

                            // Player can only play TakeFour or TakeTwo...
                            while (CardAccepted == false)
                            {
                                // Show hand and chose a card to play...
                                CardChosen = ChoseCardToPlay();  // Return card index number.

                                // Check if the card match a TakeFour or TakeTwo...
                                if (TableStack.Peek().SpecialCard == Card.CardSpecial.TakeFour && PlayingPlayer.PlayerHand.ElementAt(CardChosen).SpecialCard == Card.CardSpecial.TakeFour)
                                {
                                    CardAccepted = true;
                                }
                                else if (TableStack.Peek().SpecialCard == Card.CardSpecial.TakeTwo && PlayingPlayer.PlayerHand.ElementAt(CardChosen).SpecialCard == Card.CardSpecial.TakeTwo)
                                {
                                    CardAccepted = true;
                                }
                            }

                            // Push the temp card to tablestack...
                            TableStack.Push(PlayingPlayer.PlayerHand.ElementAt(CardChosen));
                            PlayingPlayer.PlayerHand.RemoveAt(CardChosen);
                            ValidateCardMove(Direction);

                            // Validate card move before continue...
                            CardMoveValidated = true;
                            Contains = false;
                        }
                        else
                        {
                            Console.WriteLine("{0} Du har ikke en {1} på hånden. Du har derfor trukket {2} kort op og mister din tur.", PlayingPlayer.PlayerName, TableStack.Peek().SpecialCard, CardToTakeCounter);
                            Console.ResetColor();
                            for (int i = 0; i < CardToTakeCounter; i++)
                            {
                                PlayingPlayer.PlayerHand.Add(MainStack.Pop());
                            }

                            CardToTakeCounter = 0;
                            NextPlayerTakeCard = false;
                            CardMoveValidated = true;
                        }
                    }

                    else
                    {
                        // Create Can Play validator...
                        bool CanPlay = false;

                        // Use ValidateCard to see if the card can be played...
                        foreach (Card card in PlayingPlayer.PlayerHand)
                        {
                            if (ValidateCard(card))
                            {
                                CanPlay = true;
                            }
                        }

                        // Show players turn and hand...
                        ShowPlayersTurn(PlayingPlayer);

                        // If the player have a card to play...
                        if (CanPlay == true)
                        {
                            // Create card index varible...
                            int cardIndex = 0;

                            // Chose card to play and get card index....
                            cardIndex = ChoseCardToPlay();

                            // If card move is validated remove the card from players hand.
                            if (ValidateCard(PlayingPlayer.PlayerHand.ElementAt(cardIndex)))
                            {
                                TableStack.Push(PlayingPlayer.PlayerHand.ElementAt(cardIndex));
                                PlayingPlayer.PlayerHand.RemoveAt(cardIndex);

                                // Validate move and change color if needed...
                                ValidateCardMove(Direction);

                                CardMoveValidated = true;
                            }
                        }

                        else
                        {
                            // Player have no card to play and take one card for mainstack...
                            Console.Clear();
                            Console.WriteLine("Du har desværre ingen kort at ligge, og må derfor trække et kort op.\n\nTryk på en tast for at fortsætte...");
                            Console.ReadKey();

                            // Check if the card can be played...
                            Card tempCard = MainStack.Pop();

                            // Player can play the card...
                            if (ValidateCard(tempCard) == true)
                            {
                                Console.Clear();
                                Console.WriteLine("Du trak en {0} som kan ligges og du har derfor automatisk lagt kortet...", tempCard.ToString());
                                Console.ResetColor();
                                System.Threading.Thread.Sleep(3000);
                                TableStack.Push(tempCard);
                                ValidateCardMove(Direction);
                            }

                            // Player have taked a card and can not play...
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("Du trak en {0} som ikke kan spilles og er tilføjet din hånd.", tempCard.ToString());
                                Console.ResetColor();
                                System.Threading.Thread.Sleep(2000);
                                PlayingPlayer.PlayerHand.Add(tempCard);
                            }

                            CardMoveValidated = true;
                        }
                    }
                }

                // Play next turn...
                CardMoveValidated = false;
                CheckGameRunning();
                Direction.NextTurn();
                PlayingPlayer = Players.ElementAt(Direction.Turn);
            }
        }

        // Card move validator...
        public bool ValidateCard(Card card)
        {
            bool Validated = false;

            if (card.SpecialCard == Card.CardSpecial.ChangeColor || card.SpecialCard == Card.CardSpecial.TakeFour)
            {
                Validated = true;
            }
            else
            {
                if (TableStack.Peek().SpecialCard == Card.CardSpecial.Normal)
                {
                    if (card.Color == TableStack.Peek().Color || card.Value == TableStack.Peek().Value)
                    {
                        Validated = true;
                    }
                    else
                    {
                        Validated = false;
                    }
                }
                else if (TableStack.Peek().SpecialCard == Card.CardSpecial.ChangeColor || TableStack.Peek().SpecialCard == Card.CardSpecial.TakeFour)
                {
                    if (TableStack.Peek().Color == card.Color)
                    {
                        Validated = true;
                    }
                    else if (card.SpecialCard == Card.CardSpecial.ChangeColor || card.SpecialCard == Card.CardSpecial.TakeFour)
                    {
                        Validated = true;
                    }
                }
                else if (TableStack.Peek().SpecialCard == Card.CardSpecial.TakeTwo || TableStack.Peek().SpecialCard == Card.CardSpecial.JumpOver || TableStack.Peek().SpecialCard == Card.CardSpecial.ChangeDirection)
                {
                    if (TableStack.Peek().SpecialCard == card.SpecialCard)
                    {
                        Validated = true;
                    }
                    else if (TableStack.Peek().Color == card.Color)
                    {
                        Validated = true;
                    }
                    else
                    {
                        Validated = false;
                    }
                }
                else
                {
                    Validated = false;
                }
            }

            return Validated;
        }

        public void ValidateCardMove(GameDirection Direction)
        {
            if (TableStack.Peek().SpecialCard == Card.CardSpecial.TakeFour || TableStack.Peek().SpecialCard == Card.CardSpecial.ChangeColor)
            {
                // Create temp card before changing color...
                Card TempCard = TableStack.Pop();
                int Choise;

                // Check if player are computer...
                if (PlayingPlayer.PlayerName == "Computer")
                {
                    if (TempCard.SpecialCard == Card.CardSpecial.TakeFour)
                    {
                        // Play as computer and return color choise...
                        Choise = ComputerPlayer(true);

                        // Set next player take card to true +4...
                        NextPlayerTakeCard = true;
                    }
                    else
                    {
                        // Play as computer and return color choise...
                        Choise = ComputerPlayer(true);

                        // Set next player take card to false - only color change...
                        NextPlayerTakeCard = false;
                    }
                }

                // Player are human...
                else
                {
                    // Change color and set next player to take four...
                    if (TempCard.SpecialCard == Card.CardSpecial.TakeFour)
                    {
                        // Print the color change menu...
                        PrintChoseColorMenu("\"+4\"");

                        // Set next player take card to true...
                        NextPlayerTakeCard = true;
                    }

                    // Change color and set next player to not take card...
                    else
                    {
                        // Print color change menu...
                        PrintChoseColorMenu("\"Skift farve\"");

                        // Set next take card to false...
                        NextPlayerTakeCard = false;
                    }

                    // Create input string...
                    string input = Console.ReadLine();

                    // Check valid input...
                    while (!Regex.IsMatch(input, "[1-4]"))
                    {
                        Console.WriteLine("Hov prøv igen...");
                        input = Console.ReadLine();
                    }

                    // Parse input to int choise...
                    int.TryParse(input, out Choise);
                }

                // Change color on the card from the input choise...
                TempCard.Color = (Card.CardColor)Choise;  // Chose color from the colors index number..

                // Put card back in the stack...
                TableStack.Push(TempCard);
            }

            // Check if card are +2
            else if (TableStack.Peek().SpecialCard == Card.CardSpecial.TakeTwo)
            {
                NextPlayerTakeCard = true;
            }

            // Check if direction should turn...
            else if (TableStack.Peek().SpecialCard == Card.CardSpecial.ChangeDirection)
            {
                Direction.ChangeDirection();
            }

            // Check if card is jump over...
            else if (TableStack.Peek().SpecialCard == Card.CardSpecial.JumpOver)
            {
                Direction.NextTurn(); // Run next turn to jump next player.
            }
        }

        // Players Choise validator...
        public int ChoseCardToPlay()
        {
            // Chose card to play...
            Console.WriteLine("Hvilket kort vil du smide: ");

            // Set bool validator...
            bool inputValid = false;

            // Declare input value int...
            int validatedChoise = 0;

            // Play as computer player...
            if (PlayingPlayer.PlayerName == "Computer")
            {
                validatedChoise = ComputerPlayer(false);  // Autoplay and no color change.
            }

            // Human player...
            else
            {
                while (inputValid == false)
                {
                    // Create new input string...
                    string input = Console.ReadLine();

                    // Check if input is a number...
                    while (!Regex.IsMatch(input, "[0-9]"))
                    {
                        Console.WriteLine("Hov du fik tastet noget forkert. Prøv venligst igen...");
                        input = Console.ReadLine();
                    }

                    // Parse the input to int...
                    int.TryParse(input, out int inputChoise);

                    // Check if the number are a valid card number...
                    if (inputChoise >= 0 && inputChoise < PlayingPlayer.PlayerHand.Count)
                    {
                        validatedChoise = inputChoise;
                        inputValid = true;
                    }
                    else
                    {
                        Console.WriteLine("Hov du fik tastet noget forkert. Prøv venligst igen...");
                    }
                }
            }

            return validatedChoise;
        }

        // Check if game still running...
        public void CheckGameRunning()
        {
            if (PlayingPlayer.PlayerHand.Count == 0)
            {
                GameRunning = false;
                EndGameRound();
            }
            else if (PlayingPlayer.PlayerHand.Count == 1)
            {
                Console.Clear();
                Console.WriteLine("{0} siger UNO!!!", PlayingPlayer.PlayerName);
                System.Threading.Thread.Sleep(5000);
                GameRunning = true;
            }
            else
            {
                GameRunning = true;
            }
        }

        public void ShowPlayersTurn(Player player)
        {
            // Show active player and wait for player to start turn...
            Console.Clear();
            Console.WriteLine("Det er {0}´s tur. Tryk på en tast for at starte tur...", player.PlayerName);
            Console.ReadKey();

            // Show active player and tablestack...
            Console.Clear();
            Console.WriteLine("Det er {0}´s tur", player.PlayerName);
            Console.WriteLine("Der ligger {0} på bordet\n\n", TableStack.Peek().ToString());
            Console.ResetColor();

            // Dont show computer players hand...
            if (player.PlayerName == "Computer")
            {
                Console.WriteLine("Computer spiller sin tur...");
            }
            else
            {

            }

            // Show players hand and choices...
            Console.WriteLine("Din hånd");
            foreach (Card card in PlayingPlayer.PlayerHand)
            {
                Console.WriteLine("Valg: " + PlayingPlayer.PlayerHand.IndexOf(card) + ":  " + card.ToString());
            }

            // Reset console color before continue...
            Console.ResetColor();
        }

        public int ComputerPlayer(bool colorChange)
        {
            Random Ran = new Random();
            int IndexNumber = 0;
            System.Threading.Thread.Sleep(3000);

            // Change Color...
            if (colorChange == true)
            {
                // Make a color choise for computer player...
                foreach (Card card in PlayingPlayer.PlayerHand)
                {
                    if (card.Color == Card.CardColor.Red)
                    {
                        return 1;
                    }
                    else if (card.Color == Card.CardColor.Blue)
                    {
                        return 2;
                    }
                    else if (card.Color == Card.CardColor.Green)
                    {
                        return 3;
                    }
                    else if (card.Color == Card.CardColor.Yellow)
                    {
                        return 4;
                    }
                    else
                    {
                        return Ran.Next(1, 4);
                    }
                }
            }

            // Chose card to play...
            else
            {
                List<int> CanBePlayed = new List<int>();

                foreach (Card card in PlayingPlayer.PlayerHand)
                {
                    if (ValidateCard(card))
                    {
                        CanBePlayed.Add(PlayingPlayer.PlayerHand.IndexOf(card));
                    }
                }

                IndexNumber = CanBePlayed.ElementAt(Ran.Next(CanBePlayed.Count));
            }

            return IndexNumber;
        }

        public void EndGameRound()
        {
            // Calculate and set players score...
            PlayingPlayer.PlayerScore = CalculateScore();

            // Write scores to screen...
            Console.Clear();
            foreach (Player player in Players)
            {
                Console.WriteLine("Spiller: {0}: {1}\n\n", player.PlayerName, player.PlayerScore);
            }

            // Ask for new round or exit...
            Console.WriteLine("\nTryk på \"enter\" for at starte ny runde, eller \"Esc\" for at afslutte...");
            bool Valg = true;

            while (Valg == true)
            {
                ConsoleKey key = Console.ReadKey().Key;

                if (key == ConsoleKey.Enter)
                {
                    Valg = false;
                }
                else if (key == ConsoleKey.Escape)
                {
                    Console.Clear();
                    Console.WriteLine("Du er ved at afslutte. Tryk på en tast for at fortsætte.");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }

            // Game continue...
            Console.Clear();
            Console.WriteLine("Du fortsætter...");
            System.Threading.Thread.Sleep(1000);

            // Reset all players hand...
            foreach (Player player in Players)
            {
                player.PlayerHand.Clear();
            }

            // Reset table and main card stack...
            MainStack.Clear();
            TableStack.Clear();

            // Create new mainstack of cards...
            MainStack = CardStack.Stack();

            // Give card to players...
            for (int i = 0; i < 7; i++)
            {
                foreach (Player GivePlayerCard in Players)
                {
                    GivePlayerCard.PlayerHand.Add(MainStack.Pop());
                }
            }

            // Deal firs card to table...
            TableStack.Push(MainStack.Pop());
            GameRunning = true;

            // Start new round...
            PlayGame(Players.IndexOf(PlayingPlayer));
        }

        public int CalculateScore()
        {
            int Score = 0;

            foreach (Player player in Players)
            {
                foreach (Card card in player.PlayerHand)
                {
                    if (card.SpecialCard == Card.CardSpecial.ChangeColor || card.SpecialCard == Card.CardSpecial.TakeFour)
                    {
                        Score += 50;
                    }
                    else if (card.SpecialCard == Card.CardSpecial.ChangeDirection || card.SpecialCard == Card.CardSpecial.JumpOver || card.SpecialCard == Card.CardSpecial.TakeTwo)
                    {
                        Score += 20;
                    }
                    else
                    {
                        Score += card.Value;
                    }

                    // Put the card back in mainstack...
                    MainStack.Push(card);
                }
            }

            return Score;
        }

        public void PrintChoseColorMenu(string card)
        {
            Console.Clear();
            Console.WriteLine("Der blev lagt kortet " + card + "\n\nVælg ny farve:\n\n");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("1. Rød");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("2. Blå");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("3. Grøn");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("4. Gul");
            Console.ResetColor();
            Console.WriteLine("Vælg 1 - 4: ");
        }

        public Game()
        {

        }
    }
}
