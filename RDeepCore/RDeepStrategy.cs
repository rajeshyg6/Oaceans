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
        IEnumerable<RDeepBet> GoForBet(RDeepPlayer player, List<RDeepPosition> LastNumbers);
    }

    class BetByTenFifteenStrategy : IRDeepStrategy
    {
        Dictionary<RDeepPosition, float> probabilities;
        Dictionary<PositonTypeCategory, List<int>> probabilityUpgradeFactorsOnHit;

        IEnumerable<RDeepPosition> wheelNumbers;

        public BetByTenFifteenStrategy()
        {
            probabilities = new Dictionary<RDeepPosition, float>();

            wheelNumbers = RDeepPositions.rDeepPositions.Where(num => num.isWheelNumber == true);

            SetProbabilityUpgradeFactorsOnHit();
        }

        public IEnumerable<RDeepBet> GoForBet(RDeepPlayer player, List<RDeepPosition> LastNumbers)
        {
            List<RDeepBet> result = new List<RDeepBet>();

            UpdateProbabilities(LastNumbers);

            return result;
        }

        private void SetProbabilityUpgradeFactorsOnHit()
        {
            probabilityUpgradeFactorsOnHit = new Dictionary<PositonTypeCategory, List<int>>();
            probabilityUpgradeFactorsOnHit.Add(PositonTypeCategory.Even, new List<int> { 10, 5, 0, 0, -8, -2, -1, -1, -1, -1, -1 });
            probabilityUpgradeFactorsOnHit.Add(PositonTypeCategory.Third, new List<int> { 15, 2, -2, 0, 0, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 });
            probabilityUpgradeFactorsOnHit.Add(PositonTypeCategory.Straight, new List<int> { 220, -1 });
        }

        private void SetDefaultProbabilities()
        {
            foreach (RDeepPosition number in wheelNumbers)
            {
                probabilities.Add(number, number.defaultProbability);
            }
        }

        private void UpdateProbabilities(List<RDeepPosition> LastNumbers)
        {
            SetDefaultProbabilities();

            RDeepPosition currentPos = LastNumbers[LastNumbers.Count - 1];
            foreach(PositionTypeSubCategory category in Enum.GetValues(typeof(PositionTypeSubCategory)))
            {
                UpdateProbabilitiesByPosType(category, LastNumbers);
            }
        }

        private void UpdateProbabilitiesByPosType(PositionTypeSubCategory subCategory, List<RDeepPosition> LastNumbers)
        {
            int HitCount = 1;
            if (subCategory != PositionTypeSubCategory.Straight)
                HitCount = GetPositionTypeHitCount(subCategory, LastNumbers);

            PositonTypeCategory Category = GetPositonTypeCategory(subCategory);

            //probabilityUpgradeFactorsOnHit[Category].[HitCount];
        }

        private int GetPositionTypeHitCount(PositionTypeSubCategory subCategory, List<RDeepPosition> LastNumbers)
        {
            int result = 1;

            PositonType LastPositionType = PositonType.Straight;
            PositonType CurrentPositionType = PositonType.Straight;

            for (int index = LastNumbers.Count - 1; index >= 0; index--)
            {
                CurrentPositionType = GetPositionType(subCategory, LastNumbers[index]);

                if (LastPositionType == CurrentPositionType)
                    result++;
                else
                    break;

                LastPositionType = CurrentPositionType;
            }

            return result;
        }

        private PositonType GetPositionType(PositionTypeSubCategory subCategory, RDeepPosition pos)
        {
            if (subCategory == PositionTypeSubCategory.Color)
            {
                if (pos.isRed)
                    return PositonType.Red;
                else
                    return PositonType.Black;
            }
            else if (subCategory == PositionTypeSubCategory.OddEven)
            {
                if (pos.isEven)
                    return PositonType.Even;
                else
                    return PositonType.Odd;
            }
            else if (subCategory == PositionTypeSubCategory.LowHigh)
            {
                if (pos.isLow)
                    return PositonType.Low;
                else
                    return PositonType.High;
            }
            else if (subCategory == PositionTypeSubCategory.Dozen)
            {
                if (pos.isFirstDozen)
                    return PositonType.FirstDozen;
                else if (pos.isSecondDozen)
                    return PositonType.SecondDozen;
                else
                    return PositonType.ThirdDozen;
            }
            else if (subCategory == PositionTypeSubCategory.Column)
            {
                if (pos.isFirstColumn)
                    return PositonType.FirstColumn;
                else if (pos.isSecondColumn)
                    return PositonType.SecondColumn;
                else
                    return PositonType.ThirdColumn;
            }
            else
                return PositonType.Straight;
        }

        private PositonTypeCategory GetPositonTypeCategory(PositionTypeSubCategory subCategory)
        {
            if (subCategory == PositionTypeSubCategory.Color || subCategory == PositionTypeSubCategory.OddEven || subCategory == PositionTypeSubCategory.LowHigh)
            {
                return PositonTypeCategory.Even;
            }
            else if (subCategory == PositionTypeSubCategory.Column || subCategory == PositionTypeSubCategory.Dozen)
            {
                return PositonTypeCategory.Third;
            }
            else
                return PositonTypeCategory.Straight;
        }

        private PositonTypeCategory GetPositonTypeCategory(PositonType posType)
        {
            if (posType == PositonType.Black || posType == PositonType.Red || posType == PositonType.Odd || posType == PositonType.Even || posType == PositonType.Low || posType == PositonType.High)
            {
                return PositonTypeCategory.Even;
            }
            else if (posType == PositonType.FirstColumn || posType == PositonType.SecondColumn || posType == PositonType.ThirdColumn || posType == PositonType.FirstDozen || posType == PositonType.SecondDozen || posType == PositonType.ThirdDozen)
            {
                return PositonTypeCategory.Third;
            }
            else
                return PositonTypeCategory.Straight;
        }
    }

    class RandomRDeepStrategy : IRDeepStrategy
    {
        public IEnumerable<RDeepBet> GoForBet(RDeepPlayer player, List<RDeepPosition> LastNumbers)
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
