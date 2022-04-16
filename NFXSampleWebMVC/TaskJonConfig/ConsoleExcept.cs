using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public  class ConsoleExcept
    {
        public static void WriteLine(string msg, ConsoleColor forecolor = ConsoleColor.White, ConsoleColor backcolor = ConsoleColor.Black)
        {
            Console.ForegroundColor = forecolor;
            Console.BackgroundColor = backcolor;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
        }

    }
}
