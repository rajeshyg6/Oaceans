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
        Dictionary<PositionTypeCategory, List<UpgradeProbabilityOnFewerHits>> probabilityUpgradeFactorsOnFewerHits;

        Dictionary<PositionTypeSubCategory, Dictionary<RDeepPosition, float>> GroupsUpgradeProbability;

        class UpgradeProbabilityOnFewerHits
        {
            public UpgradeProbabilityOnFewerHits(int SpinCount, int HitCount, int UpgradeFactor)
            {
                this.SpinCount = SpinCount;
                this.HitCount = HitCount;
                this.UpgradeFactor = UpgradeFactor;
            }

            public int SpinCount { get; set; }
            public int HitCount { get; set; }
            public int UpgradeFactor { get; set; }
        }

        IEnumerable<RDeepPosition> wheelNumbers;

        public BetByTenFifteenStrategy()
        {
            probabilities = new Dictionary<RDeepPosition, float>();

            wheelNumbers = RDeepPositions.rDeepPositions.Where(num => num.isWheelNumber == true);
            
            SetBlankGroupUpgradeProbabilities();

            SetProbabilityUpgradeFactorsOnHit();
        }

        public IEnumerable<RDeepBet> GoForBet(RDeepPlayer player, List<RDeepPosition> LastNumbers)
        {
            if (player.coins.Count < 1)
                throw new Exception("Running out of coins!");

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

            if (player.coins.Count(coin => coin.isOnBet == false && coin.Value <= 25) < 4)
                randomTotalCoins = 1;
            else
                randomTotalCoins = Generic.GetRandomNumber(1, 3);

            for (int i = 0; i < randomTotalCoins; i++)
            {
                List<Coin> activeCoins = player.coins.Where(coin => coin.isOnBet == false && coin.Value <= 25).ToList();
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

            var probabilitySorted = probabilities.OrderByDescending(pair => pair.Value).Take(probabilities.Count);
            
            foreach (var probability in probabilitySorted)
            {
                result += DisplayGroupProbability(probability.Key.Name, PositionTypeSubCategory.Straight, probability.Key);

                if (probability.Key.isGreen)
                    result += DisplayGroupProbability("[ G]", PositionTypeSubCategory.Color, probability.Key);
                else if (probability.Key.isRed)
                    result += DisplayGroupProbability("[ R]", PositionTypeSubCategory.Color, probability.Key);
                else
                    result += DisplayGroupProbability("[ B]", PositionTypeSubCategory.Color, probability.Key);

                if (probability.Key.isLow)
                    result += DisplayGroupProbability("[LO]", PositionTypeSubCategory.LowHigh, probability.Key);
                else if (probability.Key.isHigh)
                    result += DisplayGroupProbability("[HI]", PositionTypeSubCategory.LowHigh, probability.Key);
                else
                    result += DisplayGroupProbability("[ G]", PositionTypeSubCategory.Column, probability.Key);

                if (probability.Key.isOdd)
                    result += DisplayGroupProbability("[OD]", PositionTypeSubCategory.OddEven, probability.Key);
                else if (probability.Key.isEven)
                    result += DisplayGroupProbability("[EV]", PositionTypeSubCategory.OddEven, probability.Key);
                else
                    result += DisplayGroupProbability("[ G]", PositionTypeSubCategory.Column, probability.Key);

                if (probability.Key.isFirstDozen)
                    result += DisplayGroupProbability("[1D]", PositionTypeSubCategory.Dozen, probability.Key);
                else if (probability.Key.isSecondDozen)
                    result += DisplayGroupProbability("[2D]", PositionTypeSubCategory.Dozen, probability.Key);
                else if (probability.Key.isThirdDozen)
                    result += DisplayGroupProbability("[3D]", PositionTypeSubCategory.Dozen, probability.Key);
                else
                    result += DisplayGroupProbability("[ G]", PositionTypeSubCategory.Column, probability.Key);
                
                if (probability.Key.isFirstColumn)
                    result += DisplayGroupProbability("[1C]", PositionTypeSubCategory.Column, probability.Key);
                else if (probability.Key.isSecondColumn)
                    result += DisplayGroupProbability("[2C]", PositionTypeSubCategory.Column, probability.Key);
                else if (probability.Key.isThirdColumn)
                    result += DisplayGroupProbability("[3C]", PositionTypeSubCategory.Column, probability.Key);
                else
                    result += DisplayGroupProbability("[ G]", PositionTypeSubCategory.Column, probability.Key);

                result += string.Format("\tProb: {0:+0.0000;-0.0000}", probability.Value + "\n");
                //result += string.Format("\tDefault Probability = {0:0.0000}\n", probability.Key.defaultProbability);
            }
            return result;
        }

        private string DisplayGroupProbability(string posTypeShortName, PositionTypeSubCategory subCategory, RDeepPosition number)
        {
            string probability = string.Format("{0:+0.0000;-0.0000}", GroupsUpgradeProbability[subCategory][number]);
            return string.Concat(posTypeShortName, ": " , probability + "; ");
        }

        private void SetProbabilityUpgradeFactorsOnHit()
        {
            probabilityUpgradeFactorsOnHit = new Dictionary<PositionTypeCategory, List<int>>();
            probabilityUpgradeFactorsOnHit.Add(PositionTypeCategory.Even, new List<int> { 100, 5, 5, -3, -5, -5, -6, -7, -8, -9, -10 });
            probabilityUpgradeFactorsOnHit.Add(PositionTypeCategory.Third, new List<int> { 100, -2, -2, -2, -3, -4, -6, -6, -8, -9, -10, -11, -12, -13, -14, -15 });
            probabilityUpgradeFactorsOnHit.Add(PositionTypeCategory.Straight, new List<int> { 100, -1 }
            );
        }

        private void SetProbabilityUpgradeFactorsOnFewerHits()
        {
            probabilityUpgradeFactorsOnFewerHits = new Dictionary<PositionTypeCategory, List<UpgradeProbabilityOnFewerHits>>();

            //Even
            probabilityUpgradeFactorsOnFewerHits.Add(PositionTypeCategory.Even,
                new List<UpgradeProbabilityOnFewerHits>
                {
                    new UpgradeProbabilityOnFewerHits(10, 1, 2),
                    new UpgradeProbabilityOnFewerHits(10, 2, 1),
                    new UpgradeProbabilityOnFewerHits(20, 1, 1),
                    new UpgradeProbabilityOnFewerHits(20, 2, 1),
                    new UpgradeProbabilityOnFewerHits(30, 3, 1),
                }
                );

            //Third
            probabilityUpgradeFactorsOnFewerHits.Add(PositionTypeCategory.Third,
                    new List<UpgradeProbabilityOnFewerHits>
                    {
                        new UpgradeProbabilityOnFewerHits(15, 1, 2),
                        new UpgradeProbabilityOnFewerHits(15, 2, 1),
                        new UpgradeProbabilityOnFewerHits(30, 1, 1),
                        new UpgradeProbabilityOnFewerHits(30, 2, 1),
                        new UpgradeProbabilityOnFewerHits(45, 3, 1),
                    }
                );

            //Straght
            probabilityUpgradeFactorsOnFewerHits.Add(PositionTypeCategory.Straight,
                    new List<UpgradeProbabilityOnFewerHits>
                    {
                        new UpgradeProbabilityOnFewerHits(100, 0, 1)
                    }
                );
        }

        private int GetProbabilityUpgradeFactorsOnHit(PositionTypeCategory category, int hitCount)
        {
            List<int> ProbabilityUpgradeFactorOnHit = new List<int>();
            ProbabilityUpgradeFactorOnHit = probabilityUpgradeFactorsOnHit[category];

            if (category == PositionTypeCategory.Straight)
                hitCount = 1;

            if (ProbabilityUpgradeFactorOnHit.Count <= hitCount)
                return ProbabilityUpgradeFactorOnHit.Last();

            return ProbabilityUpgradeFactorOnHit[hitCount];
        }

        private float GetProbabilityUpgradeRate(int totalNumbersOfType)
        {
            int totalWheenNumber = wheelNumbers.Count();

            return (float)RDeepPosition.One.defaultProbability / (totalWheenNumber - totalNumbersOfType);
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

        private void SetBlankGroupUpgradeProbabilities()
        {
            GroupsUpgradeProbability = new Dictionary<PositionTypeSubCategory, Dictionary<RDeepPosition, float>>();

            foreach (PositionTypeSubCategory category in Enum.GetValues(typeof(PositionTypeSubCategory)))
            {
                GroupsUpgradeProbability.Add(category, GetBlackGroupUpgradeProbabilities());
            }
        }

        private Dictionary<RDeepPosition, float> GetBlackGroupUpgradeProbabilities()
        {
            Dictionary<RDeepPosition, float> BlackGroupUpgradeProbabilities = new Dictionary<RDeepPosition, float>();

            foreach(RDeepPosition number in wheelNumbers)
            {
                BlackGroupUpgradeProbabilities.Add(number, 0);
            }

            return BlackGroupUpgradeProbabilities;
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

            List<RDeepPosition> numbersOfHitType = new List<RDeepPosition>();
            if (hitType == PositionType.Straight)
                numbersOfHitType.Add(currentPos);
            else
                numbersOfHitType = RDeepPositions.NumbersByPositionType(hitType).ToList();

            IEnumerable<RDeepPosition> numbersOfNonHitType = wheelNumbers.Except(numbersOfHitType);
            float Rate = GetProbabilityUpgradeRate(numbersOfHitType.Count());

            ShiftProbability(numbersOfHitType, GroupsUpgradeProbability[subCategory], Factor * Rate);
            ShiftProbability(numbersOfNonHitType, GroupsUpgradeProbability[subCategory], -1 * Factor * Rate);
        }

        private void ShiftProbability(IEnumerable<RDeepPosition> toWheelNumbers, Dictionary<RDeepPosition, float> groupUpgradeProbability, float probability)
        {

            float probabilityToAdd = probability / toWheelNumbers.Count();
            foreach (RDeepPosition number in toWheelNumbers)
            {
                probabilities[number] += probabilityToAdd;
                groupUpgradeProbability[number] = probabilityToAdd;
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
                else if (LastPositionType != PositionType.Straight)
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

            int randomWheelNumber = 7;

            randomWheelNumber = Generic.GetRandomNumber(0, 38);

            if (player.coins.Count(coin => coin.isOnBet == false && coin.Value <=25) < 1)
                throw new Exception("Running out of coins!");

            List<Coin> betCoins = new List<Coin>();

            int randomTotalCoins;

            if (player.coins.Count < 4)
                randomTotalCoins = 1;
            else
                randomTotalCoins = Generic.GetRandomNumber(1, 3);

            for (int i = 0; i < randomTotalCoins; i++)
            {
                List<Coin> activeCoins = player.coins.Where(coin => coin.isOnBet == false && coin.Value <= 25).ToList();
                int randomCoin = Generic.GetRandomNumber(0, activeCoins.Count);
                activeCoins[randomCoin].isOnBet = true;
                betCoins.Add(activeCoins[randomCoin]);
                System.Threading.Thread.Sleep(100);
            }

            result.Add(new RDeepBet(
                player,
                RDeepBetPositions.GetRDeepBetPositionByPositionIDs(new int[] { randomWheelNumber }),
                betCoins));

            return result;
        }

        public IEnumerable<RDeepBet> GoForBetRandomBetTypes(RDeepPlayer player, List<RDeepPosition> LastNumbers)
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
