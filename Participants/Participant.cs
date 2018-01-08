using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monitory;

namespace Participants
{
    public class Participant
    {
        public int InitialCash { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public List<Coin> coins;

        public Participant(string name, int cash)
        {
            Name = name;
            ID = ++ParticipantList.Total;
            InitialCash = cash;
            BuyCoins(cash);
        }

        public void BuyCoinsStandard()
        {

        }

        public void BuyCoins(int Cash)
        {
            if (Cash == 10)
                coins = Coins.ConvertCashToCoins(Cash, 0, 0, 0, 0, 0, 1, 5);
            else if (Cash == 20)
                coins = Coins.ConvertCashToCoins(Cash, 0, 0, 0, 0, 0, 2, 10);
            else if (Cash == 100)
                coins = Coins.ConvertCashToCoins(Cash, 0, 0, 0, 1, 2, 2, 20);
            else if (Cash == 1000)
                coins = Coins.ConvertCashToCoins(Cash, 0, 0, 1, 6, 25, 50, 100);
            else if (Cash == 2000)
                coins = Coins.ConvertCashToCoins(Cash, 0, 0, 2, 12, 50, 120, 100);
            else if (Cash == 5000)
                coins = Coins.ConvertCashToCoins(Cash, 0, 0, 6, 20, 280, 100, 100);
            else if (Cash == 10000)
                coins = Coins.ConvertCashToCoins(Cash, 6, 4, 10, 10, 10, 50, 150);
            else if (Cash == 100000)
                coins = Coins.ConvertCashToCoins(Cash, 60, 40, 100, 100, 100, 500, 1500);
            else if (Cash == 500000)
                coins = Coins.ConvertCashToCoins(Cash, 300, 200, 500, 500, 500, 2500, 7500);
            else if (Cash == 5000000)
                coins = Coins.ConvertCashToCoins(Cash, 3000, 2000, 5000, 5000, 5000, 25000, 75000);
            else if (Cash == 9630)
                coins = Coins.ConvertCashToCoins(Cash, 6, 4, 10, 10, 10, 5, 5);
            else
                throw new Exception("Sorry, not sure about denominations!");
        }

        public static void TransferCoins(Participant fromParticipant, Participant toParticipant, IEnumerable<Coin> coins)
        {
            foreach (Coin coin in coins)
            {
                fromParticipant.coins.Remove(coin);
                toParticipant.coins.Add(coin);
            }
        }

        public static void TransferCoins(Participant fromParticipant, Participant toParticipant, CoinType CT, int factor)
        {
            TransferCoins(
                fromParticipant,
                toParticipant,
                fromParticipant.TakeCoins(CT, factor)
                );
        }

        public static IEnumerable<Coin> TakeCoins(Participant fromParticipant, CoinType CT, int units)
        {
            IEnumerable<Coin> result = new List<Coin>();
            if(fromParticipant.coins.Count(coin => coin.CT == CT) >= units)
            {
                result = fromParticipant.coins.Where(coin => coin.CT == CT);
            }
            else
            {
                throw new Exception("\n" + fromParticipant.Name + " is running out of " + CT.ToString() + "(s)");
            }
            return result.Take(units);
        }

        public IEnumerable<Coin> TakeCoins(CoinType CT, int units)
        {
            return TakeCoins(this, CT, units);
        }

        public string DisplayCoins()
        {
            string result = "";

            result += Coins.DisplayCoinsSummary(coins);

            return result;
        }
        
        public string DisplayPlayerCoinsValue()
        {
            int coinsValue = Coins.CoinsValue(coins);
            int InvStatusPct = (coinsValue * 100/ InitialCash);
            return "\tCoins Value: " + coinsValue.ToString() + "\tInvestment Status: " + InvStatusPct.ToString() + "%";
        }

        public int PlayerCoinsValue()
        {
            return Coins.CoinsValue(coins);
        }
    }

    public class ParticipantList
    {
        public static List<Participant> participants = new List<Participant>();
        public static int Total = 0;

        public ParticipantList()
        {

        }

        internal static Participant AddParticipant(string Name, int cash)
        {
            Participant participant = new Participant(Name, cash);
            participants.Add(participant);
            return participant;
        }
    }
}