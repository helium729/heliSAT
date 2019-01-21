/*
 * Copyright (c) 2019 Xuanyu Hu
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace heliSAT
{

    class Program
    {
        static int v_size, c_size;
        static bool unbranch, unsingle;

        static void Main(string[] args)
        {
            bool unsat = false;
            unsingle = false;
            unbranch = false;
            Console.WriteLine("Hello World!");
            if (args.Length == 0)
            {
                Console.WriteLine("Please specify the cnf file");
                return;
            }            
            var path = args[0];
            var clauses = readFile(path);

            Dictionary<int, bool> result = new Dictionary<int, bool>();
            var sus = findSuspecious(clauses).Keys.ToArray();
            var suppose = new bool?[sus.Length];
            for (int i = 0; i < suppose.Length; i++)
                suppose[i] = null;

            bool sing = true;
            var bkResult = new Dictionary<int, bool>();
            var bkClauses = new List<Dictionary<int, bool>>();
            bkClauses = clauses;
            bkResult = result;

            while (sing)
            {
                sing = singleClause(ref clauses, ref result);
            }
            sing = true;
            if (unsingle)
                unsat = true;

            bkResult = result;

            while (true)
            {
                if (unsat)
                {
                    Console.WriteLine("unsat");
                    return;
                }
                clauses = bkClauses;
                result = bkResult;
                nextHypothes(ref clauses, sus, ref suppose);
                if (unbranch)
                {
                    unsat = true;
                    continue;
                }
                while (sing)
                {
                    sing = singleClause(ref clauses, ref result);
                }
                sing = true;
                if (unsingle)
                {
                    for (int i = suppose.Length - 1; i >= 0; i--)
                    {
                        if (suppose[i] == null)
                            suppose[i] = false;
                        else
                            break;
                    }
                    unsingle = false;
                }
                if (clauses.Count == 0)
                    break;

            }

            return;
        }

        static List<Dictionary<int, bool>> readFile(string path)
        {
            try
            {
                List<Dictionary<int, bool>> r = new List<Dictionary<int, bool>>();
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
                        Dictionary<int, bool> clause = new Dictionary<int, bool>();
                        foreach (var a in splited)
                        {
                            if (a == "0")
                                break;
                            int temp = Convert.ToInt32(a);
                            bool v = true;
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

        static bool singleClause(ref List<Dictionary<int, bool>> clauses, ref Dictionary<int, bool> result)
        {
            var singles = from a in clauses
                          where a.Count == 1
                          select a;
            if (singles.Count() == 0)
                return false;
            foreach (var a in singles)
            {
                var t = a.Single();
                var re = result.TryAdd(t.Key, t.Value);
                if (!re)
                {
                    bool legacy = false;
                    result.TryGetValue(t.Key, out legacy);
                    unsingle = legacy != t.Value;
                    if (unsingle)
                        break;
                }
                for (int i = 0; i < clauses.Count; i++)
                {
                    bool temp = true;
                    var b = clauses[i];
                    var inside = b.TryGetValue(t.Key, out temp);
                    if (!inside)
                        continue;
                    else if (temp == t.Value)
                    {
                        clauses.RemoveAt(i);
                        i--;
                        continue;
                    }
                    else
                    {
                        clauses[i].Remove(t.Key);
                    }
                }
            }
            return true;
        }

        static Dictionary<int, int> findSuspecious(List<Dictionary<int, bool>> clauses)
        {
            Dictionary<int, int> suspicious = new Dictionary<int, int>();
            foreach (var a in clauses)
            {
                foreach (var b in a)
                {
                    if (suspicious.ContainsKey(b.Key))
                    {
                        int temp = 0;
                        suspicious.TryGetValue(b.Key, out temp);
                        temp++;
                        suspicious.Remove(b.Key);
                        suspicious.TryAdd(b.Key, temp);
                    }
                    else
                        suspicious.TryAdd(b.Key, 1);
                }
            }
            var re = from a in suspicious
                     orderby a.Value descending
                     select a;
            Dictionary<int, int> r = new Dictionary<int, int>();
            foreach (var a in re)
            {
                r.Add(a.Key, a.Value);
            }
            return r;
        }

        static void nextHypothes(ref List<Dictionary<int, bool>> clauses_original, int[] suspects, ref bool?[] susValue)
        {
            bool add = susValue[susValue.Length - 1] == null;
            if (add)
            {
                int i = susValue.Length;
                while (i > 0 && susValue[i - 1] == null)
                    i--;
                susValue[i] = true;
            }
            else
            { 
                for (int i = susValue.Length - 1; i >= 0; i--)
                {
                    if (susValue[i] == true)
                    {
                        susValue[i] = false;
                        break;
                    }
                    else
                    {
                        if (i == 0)
                        {
                            unbranch = true;
                            break;
                        }
                        susValue[i] = null;
                    }
                }
            }
            if (unbranch)
                return;
            for (int i = 0; i < susValue.Length; i++)
            {
                if (susValue == null)
                    break;
                Dictionary<int, bool> re = new Dictionary<int, bool>();
                re.Add(suspects[i], Convert.ToBoolean(susValue[i]));
                clauses_original.Add(re);
            }
        }
    }
}
