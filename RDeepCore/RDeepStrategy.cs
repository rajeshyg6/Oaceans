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
        string DisplayProbability();
    }

    class BetByTenFifteenStrategy : IRDeepStrategy
    {
        internal Dictionary<RDeepPosition, float> probabilities;
        Dictionary<PositionTypeCategory, List<int>> probabilityUpgradeFactorsOnHit;

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

            RDeepPosition maxProbableNumber = RDeepPosition.Six;
            float maxProbability = maxProbableNumber.defaultProbability;

            foreach (RDeepPosition number in wheelNumbers)
            {
                if (maxProbability < probabilities[number])
                {
                    maxProbability = probabilities[number];
                    maxProbableNumber = number;
                }
            }

            List<Coin> betCoins = new List<Coin>();

            int randomTotalCoins;

            if (player.coins.Count < 4)
                randomTotalCoins = 1;
            else
                randomTotalCoins = Generic.GetRandomNumber(1, 3);

            for (int i = 0; i < randomTotalCoins; i++)
            {
                List<Coin> activeCoins = player.coins.Where(coin => coin.isOnBet == false && coin.Value <= 5).ToList();
                int randomCoin = Generic.GetRandomNumber(0, activeCoins.Count);
                activeCoins[randomCoin].isOnBet = true;
                betCoins.Add(activeCoins[randomCoin]);
                System.Threading.Thread.Sleep(100);
            }

            result.Add(new RDeepBet(
                player,
                RDeepBetPositions.GetRDeepBetPositionByPositionIDs(new int[] { maxProbableNumber.ID }),
                betCoins));

            return result;
        }

        public string DisplayProbability()
        {
            string result = "";
            foreach (RDeepPosition number in wheelNumbers)
            {
                result += number.Name;
                if (number.isRed)
                    result += "\t [R] ";
                else
                    result += "\t [B] ";

                result += "\t: Probability = " + probabilities[number].ToString() + "";
                result += "\t: Default Probability = " + number.defaultProbability.ToString() + "\n";
            }
            return result;
        }

        private void SetProbabilityUpgradeFactorsOnHit()
        {
            probabilityUpgradeFactorsOnHit = new Dictionary<PositionTypeCategory, List<int>>();
            probabilityUpgradeFactorsOnHit.Add(PositionTypeCategory.Even, new List<int> { 10, 5, 0, 0, -8, -2, -1, -1, -1, -1, -1 });
            probabilityUpgradeFactorsOnHit.Add(PositionTypeCategory.Third, new List<int> { 15, 2, -2, 0, 0, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 });
            probabilityUpgradeFactorsOnHit.Add(PositionTypeCategory.Straight, new List<int> { 220, -1 });
        }
        
        private int GetProbabilityUpgradeFactorsOnHit(PositionTypeCategory category, int hitCount)
        {
            List<int> ProbabilityUpgradeFactorOnHit = new List<int>();
            ProbabilityUpgradeFactorOnHit = probabilityUpgradeFactorsOnHit[category];

            if (category == PositionTypeCategory.Straight)
                hitCount = 1;

            return ProbabilityUpgradeFactorOnHit[hitCount];
        }

        private float GetProbabilityUpgradeRate(int totalNumbersOfType)
        {
            int totalWheenNumber = wheelNumbers.Count();

            return RDeepPosition.One.defaultProbability / (totalWheenNumber - totalNumbersOfType);
        }

        private void SetDefaultProbabilities()
        {
            if (probabilities.Count == 0)
            {
                foreach (RDeepPosition number in wheelNumbers)
                {
                    probabilities.Add(number, number.defaultProbability);
                }
            }
            else
            {
                foreach (RDeepPosition number in wheelNumbers)
                {
                    probabilities[number] = number.defaultProbability;
                }
            }
        }

        private void UpdateProbabilities(List<RDeepPosition> LastNumbers)
        {
            SetDefaultProbabilities();

            if (LastNumbers.Count == 0) return;

            RDeepPosition currentPos = LastNumbers[LastNumbers.Count - 1];
            foreach(PositionTypeSubCategory category in Enum.GetValues(typeof(PositionTypeSubCategory)))
            {
                UpdateProbabilitiesByPosType(category, LastNumbers, currentPos);
            }
        }

        private void UpdateProbabilitiesByPosType(PositionTypeSubCategory subCategory, List<RDeepPosition> LastNumbers, RDeepPosition currentPos)
        {
            int HitCount = 1;
            if (subCategory != PositionTypeSubCategory.Straight)
                HitCount = GetPositionTypeHitCount(subCategory, LastNumbers);

            PositionTypeCategory Category = GetPositonTypeCategory(subCategory);
            int Factor = GetProbabilityUpgradeFactorsOnHit(Category, HitCount);

            PositionType hitType = GetPositionType(subCategory, currentPos);
            IEnumerable<RDeepPosition> numbersOfHitType = RDeepPositions.PositionNumbersByType(hitType);
            IEnumerable<RDeepPosition> numbersOfNonHitType = wheelNumbers.Except(numbersOfHitType);
            float Rate = GetProbabilityUpgradeRate(numbersOfHitType.Count());

            if (Factor < 0)
            {
                ShiftProbability(numbersOfHitType, numbersOfNonHitType, Factor * Rate);
            }
            else if (Factor > 0)
            {
                ShiftProbability(numbersOfNonHitType, numbersOfHitType, Factor * Rate);
            }
        }

        private void ShiftProbability(IEnumerable<RDeepPosition> fromWheelNumbers, IEnumerable<RDeepPosition> toWheelNumbers, float probability)
        {
            probability = ReduceProbability(fromWheelNumbers, probability);
            AddProbability(toWheelNumbers, probability);
        }

        private float ReduceProbability(IEnumerable<RDeepPosition> fromWheelNumbers, float probability)
        {
            float totalProbabilityReduced = 0;
            float totalCurrentProbability = 0;
            foreach (RDeepPosition number in fromWheelNumbers)
                totalCurrentProbability += probabilities[number];

            foreach (RDeepPosition number in fromWheelNumbers)
            {
                float probabilityToReduce = (probabilities[number] * probability) / totalCurrentProbability;
                probabilities[number] -= probabilityToReduce;
                totalProbabilityReduced += probabilityToReduce;
            }

            return totalProbabilityReduced;
        }

        private void AddProbability(IEnumerable<RDeepPosition> toWheelNumbers, float probability)
        {
            float probabilityToAdd = probability / toWheelNumbers.Count();
            foreach (RDeepPosition number in toWheelNumbers)
            {
                probabilities[number] += probabilityToAdd;
            }
        }

        private int GetPositionTypeHitCount(PositionTypeSubCategory subCategory, List<RDeepPosition> LastNumbers)
        {
            int result = 1;

            PositionType LastPositionType = PositionType.Straight;
            PositionType CurrentPositionType = PositionType.Straight;

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

        private PositionType GetPositionType(PositionTypeSubCategory subCategory, RDeepPosition pos)
        {
            if (subCategory == PositionTypeSubCategory.Color)
            {
                if (pos.isRed)
                    return PositionType.Red;
                else
                    return PositionType.Black;
            }
            else if (subCategory == PositionTypeSubCategory.OddEven)
            {
                if (pos.isEven)
                    return PositionType.Even;
                else
                    return PositionType.Odd;
            }
            else if (subCategory == PositionTypeSubCategory.LowHigh)
            {
                if (pos.isLow)
                    return PositionType.Low;
                else
                    return PositionType.High;
            }
            else if (subCategory == PositionTypeSubCategory.Dozen)
            {
                if (pos.isFirstDozen)
                    return PositionType.FirstDozen;
                else if (pos.isSecondDozen)
                    return PositionType.SecondDozen;
                else
                    return PositionType.ThirdDozen;
            }
            else if (subCategory == PositionTypeSubCategory.Column)
            {
                if (pos.isFirstColumn)
                    return PositionType.FirstColumn;
                else if (pos.isSecondColumn)
                    return PositionType.SecondColumn;
                else
                    return PositionType.ThirdColumn;
            }
            else
                return PositionType.Straight;
        }

        private PositionTypeCategory GetPositonTypeCategory(PositionTypeSubCategory subCategory)
        {
            if (subCategory == PositionTypeSubCategory.Color || subCategory == PositionTypeSubCategory.OddEven || subCategory == PositionTypeSubCategory.LowHigh)
            {
                return PositionTypeCategory.Even;
            }
            else if (subCategory == PositionTypeSubCategory.Column || subCategory == PositionTypeSubCategory.Dozen)
            {
                return PositionTypeCategory.Third;
            }
            else
                return PositionTypeCategory.Straight;
        }

        private PositionTypeCategory GetPositonTypeCategory(PositionType posType)
        {
            if (posType == PositionType.Black || posType == PositionType.Red || posType == PositionType.Odd || posType == PositionType.Even || posType == PositionType.Low || posType == PositionType.High)
            {
                return PositionTypeCategory.Even;
            }
            else if (posType == PositionType.FirstColumn || posType == PositionType.SecondColumn || posType == PositionType.ThirdColumn || posType == PositionType.FirstDozen || posType == PositionType.SecondDozen || posType == PositionType.ThirdDozen)
            {
                return PositionTypeCategory.Third;
            }
            else
                return PositionTypeCategory.Straight;
        }
    }

    class RandomRDeepStrategy : IRDeepStrategy
    {
        public string DisplayProbability()
        {
            return "";// throw new NotImplementedException();
        }

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
