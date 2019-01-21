using System;
using System.IO;

namespace heliSAT
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            if (args.Length == 0)
            {
                Console.WriteLine("Please specify the cnf file");
                return;
            }
        }
    }
}
