using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno_Spil
{
    class Dice
    {
        private Random random = new Random();

        public int Roll() => random.Next(6) + 1;
    }
}
