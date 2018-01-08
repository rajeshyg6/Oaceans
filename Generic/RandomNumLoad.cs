using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class RandomNumLoad
    {
        public static List<int> randomeNumbers;
        public static int TotalNumbers = 380000;

        public RandomNumLoad(int totalNumbers)
        {
            TotalNumbers = totalNumbers;
            randomeNumbers = new List<int>();
        }

        public static void LoadRandomNumbers()
        {
            LoadRandomNumbers(1, 38);
        }

        public static void LoadRandomNumbers(int minNum, int maxNum)
        {
            Random r = new Random();
            while (randomeNumbers.Count <= TotalNumbers)
            {
                System.Threading.Thread.Sleep(200);
                randomeNumbers.Add(r.Next(minNum, maxNum + 1));
            }
        }
    }
}
