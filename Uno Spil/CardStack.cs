using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno_Spil
{
    class CardStack
    {
        public CardStack()
        {
            
        }

        public static Stack<Card> Stack()
        {
            Stack<Card> stack = new Stack<Card>();

            // Create normal card 0 - 9 and 1 - 9 in every color.
            foreach (int color in Enum.GetValues(typeof(Card.CardColor)))
            {
                if (color != 0)
                {
                    for (int CardNr = 0; CardNr < 10; CardNr++)
                    {
                        stack.Push(new Card(CardNr, (Card.CardColor)Enum.ToObject(typeof(Card.CardColor), color)));
                    }

                    for (int CardNr = 1; CardNr < 10; CardNr++)
                    {
                        stack.Push(new Card(CardNr, (Card.CardColor)Enum.ToObject(typeof(Card.CardColor), color)));
                    }

                    for (int i = 1; i < 3; i++)
                    {
                        stack.Push(new Card((Card.CardColor)Enum.ToObject(typeof(Card.CardColor), color), Card.CardSpecial.TakeTwo));
                        stack.Push(new Card((Card.CardColor)Enum.ToObject(typeof(Card.CardColor), color), Card.CardSpecial.ChangeDirection));
                        stack.Push(new Card((Card.CardColor)Enum.ToObject(typeof(Card.CardColor), color), Card.CardSpecial.JumpOver));
                    }
                }
            }

            for (int i = 1; i < 5; i++)
            {
                stack.Push(new Card(Card.CardColor.ChangeColor, Card.CardSpecial.TakeFour));
                stack.Push(new Card(Card.CardColor.ChangeColor, Card.CardSpecial.ChangeColor));
            }

            stack.Shuffel();

            return stack;
        }
    }
}