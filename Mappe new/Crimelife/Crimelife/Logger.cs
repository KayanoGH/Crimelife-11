using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crimelife
{
    public static class Logger
    {
        public static void Print(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("(1dot1) ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(msg);
        }
    }
}

