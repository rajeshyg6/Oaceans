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

        string DisplayProbability(string sortOrder);

        void UpdateProbabilities(List<RDeepPosition> LastNumbers);
    }

    public class BetByTenFifteenStrategy : IRDeepStrategy
    {
        public Dictionary<RDeepPosition, float> probabilities;
        Dictionary<PositionTypeCategory, List<int>> probabilityUpgradeFactorsOnHit;
        Dictionary<PositionTypeCategory, List<UpgradeProbabilityOnFewerHits>> probabilityUpgradeFactorsOnFewerHits;

        public Dictionary<PositionType, Dictionary<RDeepPosition, float>> GroupsUpgradeProbability;

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

            SetProbabilityUpgradeFactorsOnFewerHits();
        }

        public IEnumerable<RDeepBet> GoForBet(RDeepPlayer player, List<RDeepPosition> LastNumbers)
        {
            if (player.coins.Count < 1)
                throw new Exception("Running out of coins!");

            List<RDeepBet> result = new List<RDeepBet>();
            
            RDeepPosition maxProbableNumber = RDeepPosition.Six;
            float maxProbability = maxProbableNumber.defaultProbability;

            if (probabilities.Count < 1)
                UpdateProbabilities(LastNumbers);

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
                randomTotalCoins = Generic.GetRandomNumber(1, 2);

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

        private void UpdateProbabilitiesFromGroups()
        {
            foreach(RDeepPosition number in wheelNumbers)
            {
                probabilities[number] = 0;
                foreach(PositionType positionType in Enum.GetValues(typeof(PositionType)))
                {
                    if (positionType == PositionType.Straight)
                        probabilities[number] += GroupsUpgradeProbability[positionType][number];
                }
            }
        }

        public string DisplayProbability(string sortOrder)
        {
            string result = "";

            IEnumerable<KeyValuePair<RDeepPosition, float>> probabilitySorted;

            if (sortOrder.ToUpper() == "D")
                probabilitySorted = probabilities.OrderByDescending(pair => pair.Key.Dozen).Take(probabilities.Count);
            else if (sortOrder.ToUpper() == "C")
                probabilitySorted = probabilities.OrderByDescending(pair => pair.Key.Column).Take(probabilities.Count);
            else if (sortOrder.ToUpper() == "E" || sortOrder.ToUpper() == "O")
                probabilitySorted = probabilities.OrderByDescending(pair => pair.Key.OddEven).Take(probabilities.Count);
            else if (sortOrder.ToUpper() == "R" || sortOrder.ToUpper() == "B")
                probabilitySorted = probabilities.OrderByDescending(pair => pair.Key.Color).Take(probabilities.Count);
            else if (sortOrder.ToUpper() == "L" || sortOrder.ToUpper() == "H")
                probabilitySorted = probabilities.OrderByDescending(pair => pair.Key.LowHigh).Take(probabilities.Count);
            else if (sortOrder.ToUpper() == "P" || sortOrder.ToUpper() == "R")
                probabilitySorted = probabilities.OrderByDescending(pair => pair.Value).Take(probabilities.Count);
            else
                probabilitySorted = probabilities.OrderByDescending(pair => pair.Key.Name).Take(probabilities.Count);

            foreach (var probability in probabilitySorted)
            {
                result += DisplayGroupProbability(probability.Key.Name, PositionType.Straight, probability.Key);

                result += DisplayGroupProbability("[RD]", PositionType.Red, probability.Key);
                result += DisplayGroupProbability("[BL]", PositionType.Black, probability.Key);
                result += DisplayGroupProbability("[LO]", PositionType.Low, probability.Key);
                result += DisplayGroupProbability("[HI]", PositionType.High, probability.Key);
                result += DisplayGroupProbability("[OD]", PositionType.Odd, probability.Key);
                result += DisplayGroupProbability("[EV]", PositionType.Even, probability.Key);
                result += DisplayGroupProbability("[1D]", PositionType.FirstDozen, probability.Key);
                result += DisplayGroupProbability("[2D]", PositionType.SecondDozen, probability.Key);
                result += DisplayGroupProbability("[3D]", PositionType.ThirdDozen, probability.Key);
                result += DisplayGroupProbability("[1C]", PositionType.FirstColumn, probability.Key);
                result += DisplayGroupProbability("[2C]", PositionType.SecondColumn, probability.Key);
                result += DisplayGroupProbability("[3C]", PositionType.ThirdColumn, probability.Key);

                result += string.Format("   Total: {0:+0.00000;-0.00000}", probability.Value + "\n");
                //result += string.Format("\tDefault Probability = {0:0.0000}\n", probability.Key.defaultProbability);
            }
            return result;
        }

        private string DisplayGroupProbability(string posTypeShortName, PositionType positionType, RDeepPosition number)
        {
            string probability = string.Format("{0:+0.00000;-0.00000}", GroupsUpgradeProbability[positionType][number]);
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
                    new UpgradeProbabilityOnFewerHits(10, 1, 1),
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
                        new UpgradeProbabilityOnFewerHits(15, 1, 1),
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
                        new UpgradeProbabilityOnFewerHits(90, 0, 1)
                    }
                );
        }

        public int GetProbabilityUpgradeFactorsOnFewerHits(List<RDeepPosition> LastPositions, PositionTypeCategory category, PositionType positionType)
        {
            int factor = 0;

            List<UpgradeProbabilityOnFewerHits> upgradeProbabilityOnFewerHits = probabilityUpgradeFactorsOnFewerHits[category];
            foreach(UpgradeProbabilityOnFewerHits factorData in upgradeProbabilityOnFewerHits)
            {
                if (LastPositions.Count >= factorData.SpinCount)
                {
                    IEnumerable<RDeepPosition> Last_N_Numbers = LastPositions.Skip(Math.Max(0, LastPositions.Count() - factorData.SpinCount));

                    if (Last_N_Numbers.Count(number => number.Column == positionType || number.Dozen == positionType || number.OddEven == positionType || number.LowHigh == positionType || number.Color == positionType)
                        <= factorData.HitCount)
                    {
                        factor += factorData.UpgradeFactor;
                    }
                }
            }
            return factor;
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
            GroupsUpgradeProbability = new Dictionary<PositionType, Dictionary<RDeepPosition, float>>();

            foreach (PositionType positionType in Enum.GetValues(typeof(PositionType)))
            {
                GroupsUpgradeProbability.Add(positionType, GetBlankGroupUpgradeProbabilities(positionType));
            }
        }

        private Dictionary<RDeepPosition, float> GetBlankGroupUpgradeProbabilities(PositionType positionType)
        {
            Dictionary<RDeepPosition, float> blankGroupUpgradeProbabilities = new Dictionary<RDeepPosition, float>();

            foreach(RDeepPosition number in wheelNumbers)
            {
                if (positionType == PositionType.Straight)
                    blankGroupUpgradeProbabilities.Add(number, number.defaultProbability);
                else
                    blankGroupUpgradeProbabilities.Add(number, 0);
            }

            return blankGroupUpgradeProbabilities;
        }

        public void UpdateProbabilities(List<RDeepPosition> LastNumbers)
        {
            SetDefaultProbabilities();

            if (LastNumbers.Count == 0) return;

            RDeepPosition currentPos = LastNumbers[LastNumbers.Count - 1];

            foreach(PositionType positionType in Enum.GetValues(typeof(PositionType)))
            {
                if (positionType != PositionType.Green)
                    UpdateProbabilitiesByPosType(positionType, LastNumbers, currentPos);
            }
        }

        private void UpdateProbabilitiesByPosType(PositionType positionType, List<RDeepPosition> LastNumbers, RDeepPosition currentPos)
        {
            int HitCount = 1;
            int Factor = 0;
            float UpgradeRate = 0;

            PositionTypeSubCategory subCategory = GetPositonSubCategoryByPositionType(positionType);
            PositionTypeCategory category = GetPositonTypeCategory(subCategory);

            PositionType hitPositionType = GetPositionType(subCategory, currentPos);

            List<RDeepPosition> fromNumbers = new List<RDeepPosition>();
            if (hitPositionType == PositionType.Straight)
                fromNumbers.Add(currentPos);
            else
                fromNumbers = RDeepPositions.NumbersByPositionType(positionType).ToList();

            List<RDeepPosition> toNumbers = wheelNumbers.Except(fromNumbers).ToList();

            if (positionType == hitPositionType)
            {
                // *** On Hit ***
                if (positionType != PositionType.Straight)
                    HitCount = GetPositionTypeHitCount(subCategory, LastNumbers);

                Factor = GetProbabilityUpgradeFactorsOnHit(category, HitCount);
            }
            // *** On Miss ***

            Factor += GetProbabilityUpgradeFactorsOnFewerHits(LastNumbers, category, positionType);

            float Rate = GetProbabilityUpgradeRate(fromNumbers.Count());

            UpgradeRate = Factor * Rate;

            ShiftProbability(fromNumbers, positionType, UpgradeRate);
            ShiftProbability(toNumbers, positionType, UpgradeRate * -1);
        }

        private void ShiftProbability(IEnumerable<RDeepPosition> toWheelNumbers, PositionType positionType, float probability)
        {
            Dictionary<RDeepPosition, float> groupUpgradeProbability = GroupsUpgradeProbability[positionType];

            float probabilityToAdd = probability / toWheelNumbers.Count();
            foreach (RDeepPosition number in toWheelNumbers)
            {
                probabilities[number] += probabilityToAdd;
                if (positionType == PositionType.Straight)
                    groupUpgradeProbability[number] += probabilityToAdd;
                else
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

        private PositionTypeSubCategory GetPositonSubCategoryByPositionType(PositionType posType)
        {
            if (posType == PositionType.Black || posType == PositionType.Red)
            {
                return PositionTypeSubCategory.Color;
            }
            if (posType == PositionType.Odd || posType == PositionType.Even)
            {
                return PositionTypeSubCategory.OddEven;
            }
            else if (posType == PositionType.Low || posType == PositionType.High)
            {
                return PositionTypeSubCategory.LowHigh;
            }
            else if (posType == PositionType.FirstColumn || posType == PositionType.SecondColumn || posType == PositionType.ThirdColumn)
            {
                return PositionTypeSubCategory.Column;
            }
            else if (posType == PositionType.FirstDozen || posType == PositionType.SecondDozen || posType == PositionType.ThirdDozen)
            {
                return PositionTypeSubCategory.Dozen;
            }
            else
                return PositionTypeSubCategory.Straight;
        }
    }

    class RandomRDeepStrategy : IRDeepStrategy
    {
        public string DisplayProbability(string sortOrder)
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
                randomTotalCoins = Generic.GetRandomNumber(1, 2);

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
                    randomTotalCoins = Generic.GetRandomNumber(1, 2);

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

        public void UpdateProbabilities(List<RDeepPosition> LastNumbers)
        {

        }
    }
}
