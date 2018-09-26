using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno_Spil
{
    class Player
    {
        public string PlayerName { get; set; }
        public List<Card> PlayerHand { get; }
        public int PlayerScore { get; set; }
        public bool Turn { get; set; }
        public int DiceRoll { get; set; }

        public Player(string name)
        {
            this.PlayerName = name;
            this.PlayerHand = new List<Card>();
            this.PlayerScore = PlayerScore;
            this.Turn = Turn;
            this.DiceRoll = DiceRoll;
        }
    }
}
