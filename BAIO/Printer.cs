using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ensage;

namespace BAIO
{
    internal class Printer
    {
        public static void Print(string text)
        {
            Game.PrintMessage(text);
        }
    }
}
