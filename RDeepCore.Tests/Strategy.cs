using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace RDeepCore.Tests
{
    [TestClass]
    public class Last45FirstColumnLowOddReds
    {
        BetByTenFifteenStrategy strategy;
        List<RDeepPosition> LastNumbers;

        private void instantiate()
        {

            strategy = new BetByTenFifteenStrategy();
            LastNumbers = new List<RDeepPosition>();
            for (int i = 0; i < 45; i++)
                LastNumbers.Add(RDeepPosition.One);
        }

        [TestMethod]
        public void OddOnEvenBets()
        {
            instantiate();
            int result;
            result = strategy.GetProbabilityUpgradeFactorsOnFewerHits(LastNumbers, PositionTypeCategory.Even, PositionType.Odd);
            Assert.Equals(result, 0);
        }
        [TestMethod]
        public void EvenOnEvenBets()
        {
            instantiate();
            int result;
            result = strategy.GetProbabilityUpgradeFactorsOnFewerHits(LastNumbers, PositionTypeCategory.Even, PositionType.Even);
            Assert.Equals(result, 6);
        }
        [TestMethod]
        public void LowOnEvenBets()
        {
            instantiate();
            int result;
            result = strategy.GetProbabilityUpgradeFactorsOnFewerHits(LastNumbers, PositionTypeCategory.Even, PositionType.Low);
            Assert.Equals(result, 0);
        }
        [TestMethod]
        public void HighOnEvenBets()
        {
            instantiate();
            int result;
            result = strategy.GetProbabilityUpgradeFactorsOnFewerHits(LastNumbers, PositionTypeCategory.Even, PositionType.High);
            Assert.Equals(result, 6);
        }
        [TestMethod]
        public void RedOnEvenBets()
        {
            instantiate();
            int result;
            result = strategy.GetProbabilityUpgradeFactorsOnFewerHits(LastNumbers, PositionTypeCategory.Even, PositionType.Red);
            Assert.Equals(result, 0);
        }
        [TestMethod]
        public void BlackOnEvenBets()
        {
            instantiate();
            int result;
            result = strategy.GetProbabilityUpgradeFactorsOnFewerHits(LastNumbers, PositionTypeCategory.Even, PositionType.Black);
            Assert.Equals(result, 6);
        }
        [TestMethod]
        public void FirstColumnOnThirdBets()
        {
            instantiate();
            int result;
            result = strategy.GetProbabilityUpgradeFactorsOnFewerHits(LastNumbers, PositionTypeCategory.Third, PositionType.FirstColumn);
            Assert.Equals(result, 0);
        }
        [TestMethod]
        public void SecondColumnOnThirdBets()
        {
            instantiate();
            int result;
            result = strategy.GetProbabilityUpgradeFactorsOnFewerHits(LastNumbers, PositionTypeCategory.Third, PositionType.SecondColumn);
            Assert.Equals(result, 4);
        }
        [TestMethod]
        public void ThirdColumnOnThirdBets()
        {
            instantiate();
            int result;
            result = strategy.GetProbabilityUpgradeFactorsOnFewerHits(LastNumbers, PositionTypeCategory.Third, PositionType.ThirdColumn);
            Assert.Equals(result, 4);
        }
        [TestMethod]
        public void FirstDozenOnThirdBets()
        {
            instantiate();
            int result;
            result = strategy.GetProbabilityUpgradeFactorsOnFewerHits(LastNumbers, PositionTypeCategory.Third, PositionType.FirstDozen);
            Assert.Equals(result, 0);
        }
        [TestMethod]
        public void SecondDozenOnThirdBets()
        {
            instantiate();
            int result;
            result = strategy.GetProbabilityUpgradeFactorsOnFewerHits(LastNumbers, PositionTypeCategory.Third, PositionType.SecondDozen);
            Assert.Equals(result, 4);
        }
        [TestMethod]
        public void ThirdDozenOnThirdBets()
        {
            instantiate();
            int result;
            result = strategy.GetProbabilityUpgradeFactorsOnFewerHits(LastNumbers, PositionTypeCategory.Third, PositionType.ThirdDozen);
            Assert.Equals(result, 4);
        }
    }
}
