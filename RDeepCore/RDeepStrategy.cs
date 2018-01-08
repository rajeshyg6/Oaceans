using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Participants;
using Monitory;

namespace RDeepCore
{
    interface IRDeepPlayStrategy
    {
        int BetPercentageMin { get; set; }
        int BetPercentageMax { get; set; }
        List<BetTypeCategory> betTypeCategories { get; set; }
        int StopAtLossPct { get; set; }
        int WalkAwayAtProfitPct { get; set; }

        int EvaluateBetAmount(RDeepBoard board, RDeepPlayer player);
    }

    interface IRDeepStrategy
    {
        IEnumerable<RDeepBet> GoForBet(RDeepPlayer player, List<int> LastNumbers);
    }

    class BetByTenFifteenStrategy : IRDeepStrategy
    {
        Dictionary<int, float> probabilities;
        IEnumerable<RDeepPosition> wheelNumbers;

        public BetByTenFifteenStrategy()
        {
            probabilities = new Dictionary<int, float>();

            wheelNumbers = RDeepPositions.rDeepPositions.Where(num => num.isWheelNumber == true);

            foreach (RDeepPosition number in wheelNumbers)
            {
                probabilities.Add(number.ID, number.defaultProbability);
            }
        }

        public IEnumerable<RDeepBet> GoForBet(RDeepPlayer player, List<int> LastNumbers)
        {
            List<RDeepBet> result = new List<RDeepBet>();

            UpdateProbabilities(LastNumbers);

            return result;
        }

        private void UpdateProbabilities(List<int> LastNumbers)
        {
            RDeepPosition currentPos = RDeepPositions.GetPositionByID(LastNumbers[LastNumbers.Count - 1]);
            foreach(RDeepPosition number in wheelNumbers)
            {
                //probabilities[number.ID] = 
            }
        }
    }

    class RandomRDeepStrategy : IRDeepStrategy
    {
        public IEnumerable<RDeepBet> GoForBet(RDeepPlayer player, List<int> LastNumbers)
        {
            List<RDeepBet> result = new List<RDeepBet>();

            if (player.coins.Count < 1)
                throw new Exception("Running out of coins!");

            int randomNoOfBets = Generic.GetRandomNumber(1, 4);

            for (int b = 0; b < randomNoOfBets; b++)
            {
                int BetPosition = Generic.GetRandomNumber(1, RDeepBetPositions.Total + 1);
                List<Coin> betCoins = new List<Coin>();

                int randomTotalCoins;

                if (player.coins.Count < 4)
                    randomTotalCoins = 1;
                else
                    randomTotalCoins = Generic.GetRandomNumber(1, 5);

                for (int i = 0; i < randomTotalCoins; i++)
                {
                    List<Coin> activeCoins = player.coins.Where(coin => coin.isOnBet == false).ToList();
                    int randomCoin = Generic.GetRandomNumber(0, activeCoins.Count);
                    activeCoins[randomCoin].isOnBet = true;
                    betCoins.Add(activeCoins[randomCoin]);
                    System.Threading.Thread.Sleep(100);
                }

                result.Add(new RDeepBet(
                    player,
                    RDeepBetPositions.GetRDeepBetPositionByID(BetPosition),
                    betCoins));

                if (player.coins.Count == 0)
                    break;

                System.Threading.Thread.Sleep(150);
            }

            return result;
        }
    }
}
