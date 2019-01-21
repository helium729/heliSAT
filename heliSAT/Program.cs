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
            var clauses = readFile(path);
            
        }

        static List<Dictionary<int, bool?>> readFile(string path)
        {
            try
            {
                List<Dictionary<int, bool?>> r = new List<Dictionary<int, bool?>>();
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
                        foreach (var a in splited)
                        {
                            if (a == "0")
                                break;
                            int temp = Convert.ToInt32(a);
                            bool? v = true;
                            if (temp < 0)
                            {
                                v = false;
                                temp = 0 - temp;
                            }
                            clause.Add(temp, v);
                        }
                        r.Add(clause);
                    }
                }
                return r;
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

        static void singleClause(ref List<Dictionary<int, bool>> clauses)
        {

        }
    }
}
