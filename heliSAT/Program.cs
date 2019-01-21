using System;
using System.IO;
using System.Collections.Generic;

namespace heliSAT
{
    class Program
    {
        static int v_size, c_size;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            if (args.Length == 0)
            {
                Console.WriteLine("Please specify the cnf file");
                return;
            }
            var path = args[0];


        }

        List<Dictionary<int, bool?>> readFile(string path)
        {
            try
            {
                StreamReader reader = new StreamReader(path);
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var splited = line.Split(" ");
                    if (splited[0] == "c")
                        continue;
                    else if (splited[0] == "p")
                    {
                        v_size = Convert.ToInt32(splited[2]);
                        c_size = Convert.ToInt32(splited[3]);
                        continue;
                    }
                    else
                    {
                        Dictionary<int, bool?> clause = new Dictionary<int, bool?>();

                    }
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine("CNF file exception");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                return null;
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
