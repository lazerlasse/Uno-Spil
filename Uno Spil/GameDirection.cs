using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno_Spil
{
    class GameDirection
    {
        public bool DirectionClockwise { get; set; }
        public int Turn { get; set; }
        private int PlayerCount { get; set; } 

        // Control game player turn...
        public void NextTurn()
        {
            if (DirectionClockwise == true)
            {
                GameDirection_Clockwise();
            }

            else
            {
                GameDirection_CounterClockwise();
            }
        }

        // Control clockwise gamerotation...
        public void GameDirection_Clockwise()
        {
            if (Turn == PlayerCount - 1)
            {
                Turn = 0;
            }
            else
            {
                Turn++;
            }
        }

        // Control counter clockwise gamerotation...
        public void GameDirection_CounterClockwise()
        {
            if (Turn == 0)
            {
                Turn = PlayerCount - 1;
            }
            else
            {
                Turn--;
            }
        }

        public void ChangeDirection()
        {
            if (DirectionClockwise == true)
            {
                DirectionClockwise = false;
            }
            else
            {
                DirectionClockwise = true;
            }
        }

        public GameDirection(int playerCount)
        {
            this.PlayerCount = playerCount;
        }
    }
}
