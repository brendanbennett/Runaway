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
            string html = GetData();
            //Console.WriteLine(html);
            //0 = D
            //1 = R
            string rawMap = html.Substring(html.IndexOf("=.") + 1, html.IndexOf("&FVinsMax") - html.IndexOf("=.") - 1);
            int maxIns = Int32.Parse(html.Substring(html.IndexOf("Max=") + 4, html.IndexOf("&FVinsMin") - html.IndexOf("Max=") - 4));
            int minIns = Int32.Parse(html.Substring(html.IndexOf("Min=") + 4, html.IndexOf("&FVboardX") - html.IndexOf("Min=") - 4));
            int boardX = Int32.Parse(html.Substring(html.IndexOf("boardX=") + 7, html.IndexOf("&FVboardY") - html.IndexOf("boardX=") - 7));
            int boardY = Int32.Parse(html.Substring(html.IndexOf("boardY=") + 7, html.IndexOf("&FVlevel") - html.IndexOf("boardY=") - 7));
            int level = Int32.Parse(html.Substring(html.IndexOf("level=") + 6, html.Substring(html.IndexOf("level=") + 6, 4).IndexOf('"')));

            //map generation
            var map = GenerateMap(rawMap, boardX, boardY);
            PrintMap(map, boardX, boardY);

            PrintInstructions(GenerateInstructions(4), 16, 4);


            Console.ReadKey();
        }


        static string GetData()
        {
            string username = "brendanbennett";
            string password = "youwontguessthis";

            using (WebClient client = new WebClient())
            {
                string htmlCode = client.DownloadString("http://www.hacker.org/runaway/index.php?name=" + username + "&password=" + password);
                return htmlCode;
            }
        }

        static char[,] GenerateMap(string rawMap, int boardX, int boardY)
        {
            char[,] map = new char[boardY, boardX];
            for (int i = 0; i < boardY; i++)
            {
                for (int j = 0; j < boardX; j++)
                {
                    map[i, j] = rawMap[j * (i + 1)];
                }
            }
            return map;
        }

        static void PrintMap(char[,] map,int y, int x)
        {
            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    Console.Write(string.Format("{0} ", map[i, j]));
                }
                Console.Write(Environment.NewLine);
            }
        }

        static void PrintInstructions(int[,] ins, int y, int x)
        {
            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    Console.Write(string.Format("{0} ", ins[i, j]));
                }
                Console.Write(Environment.NewLine);
            }
        }

        static int[,] GenerateInstructions(int length)
        {
            string binaryString;
            int[,] instructionSet = new int[length*length,length];

            for (int i = 0; i < (length*length); i++)
            {
                binaryString = Convert.ToString(i, 2);
                //Console.WriteLine(binaryString);
                for (int j = binaryString.Length-1; j >= 0; j--)
                {
                    instructionSet[i, j+ (length- binaryString.Length)] = Int16.Parse(binaryString[j].ToString());
                }
            }

            Console.WriteLine("Number of instructions: " + instructionSet.GetLength(0) + " Number of commands per instruction: " + instructionSet.GetLength(1));

            return instructionSet;

        }

        static int[,] RunInstructions(int[,] ins, char[,] map, int x, int y)
        {
            int xPos = x;
            int yPos = y;
            int[,] workingInstructions = new int[ins.GetLength(0), ins.GetLength(1)];
            for (int insGroup = 0; insGroup < ins.GetLength(0); insGroup++)
            {
                for (int i = 0; i < ins.GetLength(1); i++)
                {
                    if (ins[insGroup,i] == 0)
                    {
                        yPos++;
                    }
                    else
                    {
                        xPos++;
                    }

                    if (map[y,x] == 'X')
                    {
                        break;
                    }
                }
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
