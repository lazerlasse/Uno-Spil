using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Uno_Spil
{
    class Program
    {
        static void Main(string[] args)
        {
            // Write velcome message...
            Console.WriteLine("Velkommen til Lasse´s UNO Spil.\n\n\nHvor mange skal spille? (Fra 2 til 9 spillere)");

            // Create user input string...
            string input = Console.ReadLine();

            // Check input value is valid or try again...
            while (!Regex.IsMatch(input, "[2-9]"))
            {
                Console.Clear();
                Console.WriteLine("Hov prøv igen... Det skal være mellem 2 og 9 spillere...");
                input = Console.ReadLine();
            }

            // Parse input to int playersCount
            int.TryParse(input, out int playersCount);

            // Create new game instance...
            Game game = new Game();

            // Start the game and push players to play count...
            game.NewGame(playersCount);
        }
    }
}
