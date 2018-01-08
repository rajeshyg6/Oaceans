using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitory
{
    public class Coin
    {

        public int Value { get; set; }
        public int ID { get; set; }
        public CoinType CT { get; set; }
        public bool isOnBet { get; set; }

        public Coin(CoinType C)
        {
            CT = C;
            Value = (int)C;
            ID = ++Coins.Total;
        }
    }

    public class Coins
    {
        public static int Total = 0;
        public static List<Coin> coins = null;

        public static Coin GetCoinByID(int ID)
        {
            Coin result = null;
            foreach (Coin c in coins)
            {
                if (c.ID == ID)
                {
                    result = c;
                    break;
                }
            }

            return result;
        }

        public static void SetBetStatus(IEnumerable<Coin> coins, bool onBet)
        {
            foreach(Coin coin in coins)
            {
                coin.isOnBet = onBet;
            }
        }

        public static List<Coin> ConvertCashToCoins(int cash, int Thousands = 0, int FiveHundreds = 0, int Hundreds = 0, int Fifties = 0, int Tens = 0, int Fives = 0, int Ones = 0)
        {
            List<Coin> result = new List<Coin>();

            if (cash != Thousands * 1000 + FiveHundreds * 500 + Hundreds * 100 + Fifties * 50 + Tens * 10 + Fives * 5 + Ones * 1)
                throw new Exception("Incorrect denominations to convert");

            if (Thousands > 0)
                result.AddRange(ConvertCashToCoins(Thousands * 1000, CoinType.Thousand));
            if (FiveHundreds > 0)
                result.AddRange(ConvertCashToCoins(FiveHundreds * 500, CoinType.FiveHundred));
            if (Hundreds > 0)
                result.AddRange(ConvertCashToCoins(Hundreds * 100, CoinType.Hundred));
            if (Fifties > 0)
                result.AddRange(ConvertCashToCoins(Fifties * 50, CoinType.Fifty));
            if (Tens > 0)
                result.AddRange(ConvertCashToCoins(Tens * 10, CoinType.Ten));
            if (Fives > 0)
                result.AddRange(ConvertCashToCoins(Fives * 5, CoinType.Five));
            if (Ones > 0)
                result.AddRange(ConvertCashToCoins(Ones * 1, CoinType.One));

            return result;
        }

        public static List<Coin> ConvertCashToCoins(int cash, CoinType C, bool canAcceptReminderInOnes = false)
        {
            List<Coin> result = new List<Coin>();

            if (!canAcceptReminderInOnes && cash % (int)C != 0)
                throw new Exception("Given amoubt can't be split in to specified coin types");
            else
            {
                int reminder = cash - (cash / (int)C);
                for (int i = 1; i <= cash - reminder; i++)
                {
                    result.Add(new Coin(C));
                }
                if (reminder > 0)
                    ConvertCashToCoins(reminder, CoinType.One);
            }

            return result;
        }

        public static string DisplayCoinsSummary(IEnumerable<Coin> coins)
        {
            string result = "Coins:\t";

            foreach(CoinType CT in Enum.GetValues(typeof(CoinType)))
            {
                IEnumerable<Coin> C = coins.Where(coin => coin.CT == CT);

                if (C.Count() > 0)
                    result += DisplayCoinsSummary(C, CT);
            }

            return result;
        }

        public static string DisplayCoinsSummary(IEnumerable<Coin> coins, CoinType CT)
        {
            return "[" + CT.ToString() + "s] - " + coins.Count().ToString() + "\t";
        }

        public static int CoinsValue(IEnumerable<Coin> coins)
        {
            int Value = 0;

            foreach (Coin C in coins)
                Value += C.Value;

            return Value;
        }
    }

    public enum CoinType : int { One = 1, Five = 5, Ten = 10, Fifty = 50, Hundred = 100, FiveHundred = 500, Thousand = 1000 }
}
