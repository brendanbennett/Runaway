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
            while (true)
            { 
            int maxInsLength = 4;
            string username = "brendanbennett";
            string password = "youwontguessthis";

            //Data
            string html = GetData(username, password);
            string rawMap = html.Substring(html.IndexOf("=.") + 1, html.IndexOf("&FVinsMax") - html.IndexOf("=.") - 1);
            int maxIns = Int32.Parse(html.Substring(html.IndexOf("Max=") + 4, html.IndexOf("&FVinsMin") - html.IndexOf("Max=") - 4));
            int minIns = Int32.Parse(html.Substring(html.IndexOf("Min=") + 4, html.IndexOf("&FVboardX") - html.IndexOf("Min=") - 4));
            int boardX = Int32.Parse(html.Substring(html.IndexOf("boardX=") + 7, html.IndexOf("&FVboardY") - html.IndexOf("boardX=") - 7));
            int boardY = Int32.Parse(html.Substring(html.IndexOf("boardY=") + 7, html.IndexOf("&FVlevel") - html.IndexOf("boardY=") - 7));
            int level = Int32.Parse(html.Substring(html.IndexOf("level=") + 6, html.Substring(html.IndexOf("level=") + 6, 4).IndexOf('"')));



            //map generation
                var map = GenerateMap(rawMap, boardX, boardY);
                //PrintMap(map, boardX, boardY);
                //Console.WriteLine(rawMap);
                Console.WriteLine("Max Instruxtions: " + maxIns);
                Console.WriteLine("Min Instruxtions: " + minIns);
                Console.WriteLine("Board X, Y: " + boardX + ", " + boardY);
                Console.WriteLine("Level: " + level);

                SolveAll(minIns, maxIns, maxInsLength, map);
                Console.Write(Environment.NewLine);
            }
            


            //PrintInstructions(RunInstructions(GenerateInstructions(2), map, 1, 2));//test





            //Console.ReadKey();
        }


        static void SolveAll(int minIns, int maxIns, int maxInsLength, List<List<char>> map)
        {
            List<Instruct> possibleIns = new List<Instruct>();
            for (int i = minIns; i <= maxIns; i++)
            {
                possibleIns.AddRange(GetFullInstructions(maxInsLength, i, map));
            }
            foreach (Instruct i in possibleIns)
            {
                if (RunInstrucionToEnd(i, map) == true)
                {
                    PrintInstructions(i);
                    Solve(i, "brendanbennett", "youwontguessthis");
                    break;
                }
            }
        }


        static List<Instruct> GetFullInstructions(int maxInsLength, int maxIns, List<List<char>> map)
        {
            List<int> lengths = GetInstructionLengths(maxInsLength, maxIns);
            List<Instruct> outIns = new List<Instruct>();
            List<Instruct> workingIns = new List<Instruct>();
            /*foreach (int len in lengths)
            {
                Console.Write(len + ", ");
            }*/
            int iter = 0;
            foreach (int length in lengths)
            {
                iter++;
                //Console.Write(Environment.NewLine);
                if (workingIns.Count != 0)
                {
                    //PrintInstructions(workingIns);
                    List<Instruct> newInsTemp2 = new List<Instruct>();
                    for (int startInsIndex = 0; startInsIndex < workingIns.Count; startInsIndex++)
                    {
                        Instruct currentInstruct = new Instruct();
                        currentInstruct = workingIns[startInsIndex];
                        List<Instruct> newInsTempBase = new List<Instruct>();
                        List<Instruct> newInsList = new List<Instruct>();
                        newInsList = RunInstructions(GenerateInstructions(length), map, currentInstruct.EndPos()[1], currentInstruct.EndPos()[0]);
                        //Console.WriteLine(length);
                        //Console.WriteLine("From: ");
                        //PrintInstructions(currentInstruct);
                        //Console.WriteLine("y = " + currentInstruct.EndPos()[0] + ", x = " + currentInstruct.EndPos()[1]);
                        //PrintInstructions(newInsList);
                        //Console.Write(Environment.NewLine);

                        for (int i = 0; i < newInsList.Count; i++)
                        {
                            newInsTempBase.Add(currentInstruct);
                        }
                        //PrintInstructions(newInsTempBase);
                        newInsTemp2 = new List<Instruct>();
                        int k = 0;
                        foreach (Instruct baseIns in newInsTempBase)
                        {
                            Instruct combinedInstruct = new Instruct();
                            combinedInstruct.AddRange(baseIns);
                            combinedInstruct.AddRange(newInsList[k]);
                            newInsTemp2.Add(combinedInstruct);
                            k++;
                        }
                        k = 0;
                        //PrintInstructions(newInsTemp2);
                        outIns.AddRange(newInsTemp2);
                    }
                    ///PrintInstructions(outIns);

                    if (iter == lengths.Count)
                     
                    {
                        return outIns;
                    }

                    workingIns.RemoveRange(0, workingIns.Count);
                    workingIns.AddRange(outIns);
                    outIns.RemoveRange(0, outIns.Count);
                    //workingIns.RemoveRange(0, workingIns.Count);


                }
                else
                {
                    foreach (Instruct ins in RunInstructions(GenerateInstructions(length), map, 0, 0))
                    {
                        workingIns.Add(ins);

                    }
                    if (GetInstructionLengths(maxInsLength, maxIns).Count == 1)
                    {
                        return workingIns;
                    }
                    
                    //PrintInstructions(workingIns);
                }

            }
            return null;

            
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
                    mapRow[j] = rawMap[j + (i * boardX)];
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

        static void PrintInstructions(List<Instruct> ins)
        {
            foreach (Instruct instruction in ins)
            {
                foreach (int subInstruction in instruction)
                {
                    Console.Write(string.Format("{0} ", subInstruction));
                }
                Console.Write(Environment.NewLine);
                Console.Write(Environment.NewLine);
            }
        }
        static void PrintInstructions(Instruct ins)
        {
            foreach (int subInstruction in ins)
            {
                Console.Write(string.Format("{0} ", subInstruction));
            }
            Console.Write(Environment.NewLine);
            Console.Write(Environment.NewLine);
        }

        public static List<T> CreateList<T>(int capacity)
        {
            return Enumerable.Repeat(default(T), capacity).ToList();
        }

        static List<Instruct> GenerateInstructions(int length)
        {
            string binaryString;
            List<Instruct> instructionSet = new List<Instruct>();

            for (int i = 0; i < (Math.Pow(2,length)); i++)
            {
                binaryString = Convert.ToString(i, 2);
                //Console.WriteLine(binaryString);
                //Instruction is in form {0,0,0,0}
                Instruct instruction = new Instruct();
                instruction.AddEnd(CreateList<short>(length));

                for (int j = binaryString.Length - 1; j >= 0; j--)
                {
                    instruction[j + (length - binaryString.Length)] = Int16.Parse(binaryString[j].ToString());
                }
                instructionSet.Add(instruction);
            }

            //Console.WriteLine("Number of instructions: " + instructionSet.Count + " Number of commands per instruction: " + instructionSet[0].Count);

            return instructionSet;

        }

        static List<Instruct> RunInstructions(List<Instruct> ins, List<List<char>> map, int x, int y) //Input instructions, output instructions that don't fail. Only output 1 if it has won.
        {
            int xPos = x;
            int yPos = y;
            List<int> badIns = new List<int>();
            List<int> wonIns = new List<int>();

            

            List<Instruct> workingIns = new List<Instruct>(ins);

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
                        //Console.WriteLine(insGroup);
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
                workingIns[0].Won = true;
                return workingIns;
            }

            for (int i = badIns.Count - 1; i >= 0; i--)
            {
                workingIns.RemoveAt(badIns[i]);//Remove instructions that don't work
            }


            return workingIns;

        }

        static bool RunInstrucionToEnd(Instruct ins,List<List<char>> map)
        {
            int xPos = 0;
            int yPos = 0;
            while (true)
            {
                for (int i = 0; i < ins.Count; i++)
                {
                    if (ins[i] == 0)
                    {
                        yPos++;
                    }
                    else
                    {
                        xPos++;
                    }

                    if (map[yPos][xPos] == 'X')
                    {
                        //Console.WriteLine(insGroup);
                        return false;
                    }
                    else if (map[yPos][xPos] == 'G')
                    {
                        return true;
                    }
                }
            }

        }


        static void Solve(Instruct insRaw, string username, string password)
        {
            List<string> insString = new List<string>();
            foreach (short i in insRaw)
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

        static List<int> GetInstructionLengths(int maxIns, int initInsLength)
        {
            List<int> insLengths = new List<int>();

            for (int i = 0; i < initInsLength/maxIns; i++)
            {
                insLengths.Add(maxIns);
            }
            if (initInsLength % maxIns != 0)
            {
                insLengths.Add(initInsLength - (maxIns * insLengths.Count));
            }

            return insLengths;
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
    class Instruct : List<Int16>
    {
        public Instruct AddEnd(Instruct instruct2)
        {
            foreach (short i in instruct2)
            {
                this.Add(i);
            }
            return this;
        }

        public Instruct AddEnd(List<short> instruct2)
        {
            foreach (short i in instruct2)
            {
                this.Add(i);
            }
            return this;
        }

        public List<int> EndPos(int yStart = 0, int xStart = 0)
        {
            List<int> pos = new List<int>();
            pos.Add(yStart);
            pos.Add(xStart);
            foreach (short i in this)
            {
                if (i == 0)
                {
                    pos[0]++;
                }
                else
                {
                    pos[1]++;
                }
            }
            return pos;
        }
        public bool Won { get; set; }
    }
}