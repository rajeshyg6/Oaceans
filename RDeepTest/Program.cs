﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDeepCore;
using Participants;
using Utilities;
using System.Threading;
using System.IO;

namespace RDeepTest
{
    class Program
    {
        static RDeepBoard board;

        static void Main(string[] args)
        {
            //WriteRandomNumbersToFile();
            //return;
            board = new RDeepBoard();
            /*
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n****Game Starts*****\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(board.DisplayBoardInfo());
            Console.ResetColor();
            */
            //ShowAllPositions();

            Spin();

            Console.ReadKey();
        }

        static void Spin()
        {
            try
            {
                //Console.BackgroundColor = ConsoleColor.White;
                //Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("\nHit S or 0-37 to spin again.");

                string line = Console.ReadLine();
                Console.Clear();

                board.CallForBets();

                string s = "";
                s = board.DisplayBoardPlayersCoinsValue();
                Console.WriteLine(s);
                //s += board.DisplayBetAndWinningNumbers();
                //Console.WriteLine(s);

                /*
                s = board.DisplayBoardInfo();
                if (s.Contains("MISMATCH"))
                    Console.ForegroundColor = ConsoleColor.Red;
                else
                    Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(s);
                Console.ResetColor();
                */

                //System.Threading.Thread.Sleep(100);

                int value = -1;
                int.TryParse(line, out value);

                if (line.ToUpper() == "S" || line == "" || (value >= 1 && value <= 37) || line == "0")
                {
                    board.Spin(value);
                    board.ClearBets();
                }
                Console.WriteLine("\n" + board.DisplayLastNumbers());
                Console.WriteLine("\n" + board.DisplayPlayersProbabilities(line));

                if (line.ToUpper() != "X")
                    Spin();
            }
            catch (Exception e)
            {
                Console.WriteLine("\n Exception: " + e.Message.ToString() + "\nStack Trace: " + e.StackTrace.ToString());
            }
        }


        private static void WriteRandomNumbersToFile()
        {
            int Total = 38000;
            RandomNumLoad random = new RandomNumLoad(Total);

            ThreadStart childThreadRef = new ThreadStart(RandomNumLoad.LoadRandomNumbers);

            Thread childThread = new Thread(childThreadRef);
            childThread.Start();

            string filePath = "C:\\Users\\Rajesh Y\\Documents\\Visual Studio 2017\\Projects\\Oaceans\\RDeepTest\\Data\\RandomNumbers38000.txt";

            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                int IndexOfLastNumberSaved = -1;
                while (RandomNumLoad.randomeNumbers.Count < Total)
                {
                    int Index = IndexOfLastNumberSaved;
                    for (IndexOfLastNumberSaved = RandomNumLoad.randomeNumbers.Count() - 1; Index < IndexOfLastNumberSaved; Index++)
                    {
                        if (IndexOfLastNumberSaved >= 0 && Index >= 0)
                            writer.Write(RandomNumLoad.randomeNumbers[Index].ToString() + ",\n");
                    }

                    if (Console.ReadKey().KeyChar.ToString().ToLower() == "r")
                    {
                        Console.Clear();
                        Console.WriteLine("Total random numbers loaded " + RandomNumLoad.randomeNumbers.Count.ToString() + "\n");
                        List<int> randNums = new List<int>();
                        try
                        {
                            for (int i = 1; i < 38; i++)
                            {
                                int TotalRandomNumbers = RandomNumLoad.randomeNumbers.Count();
                                int groupActualTotal = RandomNumLoad.randomeNumbers.Count(num => num == i);
                                int groupEstToal = TotalRandomNumbers / 38;
                                int ActualPct = 0;
                                if (groupEstToal > 0)
                                    ActualPct = groupActualTotal * 100 / groupEstToal;

                                Console.WriteLine("\t" + i.ToString() + "\tActualTotal: " + groupActualTotal + "\tEstimated Total: " + groupEstToal + "\tActual%: " + ActualPct.ToString() + "%");
                            }
                        }
                        catch (IOException ieo)
                        {
                            Console.WriteLine(ieo.Message.ToString() + "\n" + ieo.StackTrace.ToString());
                        }
                        catch
                        {

                        }
                    }
                }
                writer.Flush();
                writer.Close();
            }
            Console.ReadLine();
        }

        static void ShowAllPositions()
        {
            int counter = 1;
            foreach (RDeepPosition betPos in RDeepPositions.rDeepPositions)
            {
                Console.WriteLine("\n" + counter.ToString() + "- Position: " + betPos.ID + "\n");
                //Console.WriteLine(RDeepPositions.PositionsSummary(
                Console.WriteLine("\n" + betPos.Dozen.ToString());
                Console.ReadKey();
                counter++;
            }
        }
    }
}
