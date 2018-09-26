using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno_Spil
{
    class Card
    {
        public int Value { get; set; }
        public CardColor Color { get; set; }
        public CardSpecial SpecialCard { get; set; }

        public enum CardColor
        {
            ChangeColor,
            Red,
            Blue,
            Green,
            Yellow
        }

        public enum CardSpecial
        {
            Normal,
            TakeTwo,
            TakeFour,
            ChangeDirection,
            JumpOver,
            ChangeColor,
        }

        public Card()
        {

        }

        public Card(int value, CardColor color)
        {
            this.Value = value;
            this.Color = color;
        }

        public Card(CardColor color, CardSpecial specialCard)
        {
            this.Color = color;
            this.SpecialCard = specialCard;
        }

        public override string ToString()
        {
            string Print;

            if (SpecialCard == CardSpecial.Normal)
            {
                Print = Color.ToString() + " - " + Value.ToString();
            }
            else
            {
                Print = Color.ToString() + " - " + SpecialCard.ToString();
            }

            if (Color == CardColor.Blue)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
            }
            else if (Color == CardColor.Green)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (Color == CardColor.Red)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (Color == CardColor.Yellow)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else
            {
                Console.ResetColor();
            }

            return Print;
        }
    }
}
