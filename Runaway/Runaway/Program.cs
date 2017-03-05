using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Runaway
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //Console.WriteLine(html);
            //0 = D
            //1 = R
            int maxInsLength = 4;
            string username = "brendanbennett";
            string password = "youwontguessthis";

            //Data
            string html = GetData(username,password);
            string rawMap = html.Substring(html.IndexOf("=.") + 1, html.IndexOf("&FVinsMax") - html.IndexOf("=.") - 1);
            int maxIns = Int32.Parse(html.Substring(html.IndexOf("Max=") + 4, html.IndexOf("&FVinsMin") - html.IndexOf("Max=") - 4));
            int minIns = Int32.Parse(html.Substring(html.IndexOf("Min=") + 4, html.IndexOf("&FVboardX") - html.IndexOf("Min=") - 4));
            int boardX = Int32.Parse(html.Substring(html.IndexOf("boardX=") + 7, html.IndexOf("&FVboardY") - html.IndexOf("boardX=") - 7));
            int boardY = Int32.Parse(html.Substring(html.IndexOf("boardY=") + 7, html.IndexOf("&FVlevel") - html.IndexOf("boardY=") - 7));
            int level = Int32.Parse(html.Substring(html.IndexOf("level=") + 6, html.Substring(html.IndexOf("level=") + 6, 4).IndexOf('"')));

            //map generation
            var map = GenerateMap(rawMap, boardX, boardY);
            PrintMap(map, boardX, boardY);

            for (int i = minIns; i <= maxIns; i++)
            {
                var startingIns = GenerateInstructions(i);

                var workingIns = RunInstructions(startingIns, map, 0, 0);
                PrintInstructions(startingIns);
                Console.Write(Environment.NewLine);
                PrintInstructions(workingIns);
            }

            /*
            foreach (List<int> instruction in workingIns)
            {

                foreach (int subIns in instruction)
                {
                    if (subIns == 0)
                    {
                        yPos++;
                    }
                }
            }*/

            Console.ReadKey();
        }


        static string GetData(string username, string password)
        {

            using (WebClient client = new WebClient())
            {
                string htmlCode = client.DownloadString("http://www.hacker.org/runaway/index.php?name=" + username + "&password=" + password);
                return htmlCode;
            }
        }

        static List<List<char>> GenerateMap(string rawMap, int boardX, int boardY)
        {
            List<List<char>> map = new List<List<char>>();
            for (int i = 0; i < boardY; i++)
            {
                var mapRow = CreateList<char>(boardX);
                for (int j = 0; j < boardX; j++)
                {
                    mapRow[j] = rawMap[j * (i + 1)];
                }
                mapRow.Add('G');
                map.Add(mapRow);
            }
            map.Add(CreateList<char>(boardX + 1));
            for (int i = 0; i <= boardX; i++)
            {
                map[boardY][i] = 'G';
            }

            return map;
        }

        static void PrintMap(List<List<char>> map, int y, int x)
        {
            for (int i = 0; i <= y; i++)
            {
                for (int j = 0; j <= x; j++)
                {
                    Console.Write(string.Format("{0} ", map[i][j]));
                }
                Console.Write(Environment.NewLine);
            }
        }

        static void PrintInstructions(List<List<int>> ins)
        {
            foreach (List<int> instruction in ins)
            {
                foreach (int subInstruction in instruction)
                {
                    Console.Write(string.Format("{0} ", subInstruction));
                }
                Console.Write(Environment.NewLine);
            }
        }

        private static List<T> CreateList<T>(int capacity)
        {
            return Enumerable.Repeat(default(T), capacity).ToList();
        }

        static List<List<int>> GenerateInstructions(int length)
        {
            string binaryString;
            List<List<int>> instructionSet = new List<List<int>>();

            for (int i = 0; i < (Math.Pow(2,length)); i++)
            {
                binaryString = Convert.ToString(i, 2);
                //Console.WriteLine(binaryString);
                //Instruction is in form {0,0,0,0}
                var instruction = CreateList<int>(length);

                for (int j = binaryString.Length - 1; j >= 0; j--)
                {
                    instruction[j + (length - binaryString.Length)] = Int16.Parse(binaryString[j].ToString());
                }
                instructionSet.Add(instruction);
            }

            Console.WriteLine("Number of instructions: " + instructionSet.Count + " Number of commands per instruction: " + instructionSet[0].Count);

            return instructionSet;

        }

        static List<List<int>> RunInstructions(List<List<int>> ins, List<List<char>> map, int x, int y) //Input instructions, output instructions that don't fail. Only output 1 if it has won.
        {
            int xPos = x;
            int yPos = y;
            List<int> badIns = new List<int>();
            List<int> wonIns = new List<int>();

            

            List<List<int>> workingIns = new List<List<int>>(ins);

            for (int insGroup = 0; insGroup < ins.Count; insGroup++)
            {

                yPos = y;
                xPos = x;
                for (int i = 0; i < ins[0].Count; i++)
                {
                    if (ins[insGroup][i] == 0)
                    {
                        yPos++;
                    }
                    else
                    {
                        xPos++;
                    }

                    if (map[yPos][xPos] == 'X')
                    {
                        Console.WriteLine(insGroup);
                        badIns.Add(insGroup);
                        break;
                    }
                    else if (map[yPos][xPos] == 'G')
                    {
                        wonIns.Add(insGroup);
                        break;
                    }
                }
                if (wonIns.Count > 0)
                {
                    break;
                }
            }

            if (wonIns.Count > 0)
            {
                workingIns.RemoveRange(0, workingIns.Count);
                workingIns.Add(ins[wonIns[0]]);
                return workingIns;
            }

            for (int i = badIns.Count - 1; i >= 0; i--)
            {
                workingIns.RemoveAt(badIns[i]);//Remove instructions that don't work
            }


            return workingIns;

        }

        static void Solve(List<int> insRaw, string username, string password)
        {
            List<string> insString = new List<string>();
            foreach (int i in insRaw)
            {
                if (i == 0)
                {
                    insString.Add("D");
                }
                else if (i == 1)
                {
                    insString.Add("R");
                }
            }
            string joined = string.Join<string>("", insString);


            using (WebClient client = new WebClient())
            {
                client.DownloadData("http://www.hacker.org/runaway/index.php?name=" + username + "&password=" + password + "&path=" + joined);
            }
        }

        //static void ExtractData(string html)
        //{
        //    string rawMap = html.Substring(html.IndexOf("=.") + 1, html.IndexOf("&FVinsMax") - html.IndexOf("=.") - 1);
        //    string maxIns = html.Substring(html.IndexOf("Max=") + 4, html.IndexOf("&FVinsMin") - html.IndexOf("Max=") - 4);
        //    string minIns = html.Substring(html.IndexOf("Min=") + 4, html.IndexOf("&FVboardX") - html.IndexOf("Min=") - 4);
        //    string boardX = html.Substring(html.IndexOf("boardX=") + 7, html.IndexOf("&FVboardY") - html.IndexOf("boardX=") - 7);
        //    string boardY = html.Substring(html.IndexOf("boardY=") + 7, html.IndexOf("&FVlevel") - html.IndexOf("boardY=") - 7);
        //    string level = html.Substring(html.IndexOf("level=") + 6, html.Substring(html.IndexOf("level=") + 6, 4).IndexOf('"'));

        //    Console.WriteLine(level);
        //} 
    }
}