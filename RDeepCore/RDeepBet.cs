using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monitory;

namespace RDeepCore
{
    public enum BetTypes
    {
        StraightUp,
        Row,
        Split,
        Street,
        Corner,
        TopLineOrBasket,
        SixLine,
        FirstDozen,
        SecondDozen,
        ThirdDozen,
        FirstColumn,
        SecondColumn,
        ThirdColumn,
        Odd,
        Even,
        Red,
        Black,
        OneToEighteen,
        NinteenToThirtyEight
    }

    public enum BetTypeCategory
    {
        StraightUp,
        Split,
        Corner,
        SixLine,
        Dozens,
        EvenOdd,
        EvenRedBlack,
        HighLow,
        Green,
        TopOfTheLine
    }

    public enum BetStatus
    {
        Placed,
        Lost,
        Won
    }

    public class RDeepBet
    {
        public RDeepPlayer Player { get; set; }
        public RDeepBetPosition BetPosition { get; set; }
        public List<Coin> BetCoins { get; set; }
        public BetStatus status { get; set; }

        public RDeepBet(RDeepPlayer player, RDeepBetPosition betPosition, List<Coin> betCoins)
        {
            try
            {
                Player = player;
                BetPosition = betPosition;
                BetCoins = betCoins;
                Coins.SetBetStatus(betCoins, true);
                status = BetStatus.Placed;
            }
            catch (Exception e)
            {
                Coins.SetBetStatus(betCoins, false);
                throw e;
            }
        }

        public string DisplayBet()
        {
            string result = "\nBet:\t";
            result += "Player:\t" + Player.Name + ";\t";
            result += "Position:\t" + BetPosition.betType.ToString() + ";\t" + RDeepPositions.PositionsSummary(BetPosition.betPositions) + "\t";
            result += Coins.DisplayCoinsSummary(BetCoins);
            result += "Bet Value: " + Coins.CoinsValue(BetCoins);
            return result;
        }
    }
}
