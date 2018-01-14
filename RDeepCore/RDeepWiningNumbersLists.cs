using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDeepCore
{
    public class RDeepBetTypeWiningNumbersList
    {
        public int Factor { get; set; }
        public BetTypes betType { get; set; }
        public List<int[]> betWiningNumbersList;
        public int ID;

        public RDeepBetTypeWiningNumbersList(int factor, BetTypes bet, List<int[]> betWinningNumbersList)
        {
            betWiningNumbersList = betWinningNumbersList;
            Factor = factor;
            betType = bet;
            ID = ++RDeepWiningNumbersList.Total;
        }
    }

    public class RDeepWiningNumbersList
    {
        public static List<RDeepBetTypeWiningNumbersList> betTypeWinningNumbersLists;
        public static int Total = 0;

        public RDeepWiningNumbersList()
        {
            betTypeWinningNumbersLists = new List<RDeepBetTypeWiningNumbersList>();
            foreach (BetTypes bet in Enum.GetValues(typeof(BetTypes)))
            {
                betTypeWinningNumbersLists.Add(LoadBetTypeWiningNumbersList(bet));
            }
        }

        internal static RDeepBetTypeWiningNumbersList GetBetTypeWinningNumbersListByID(int ID)
        {
            RDeepBetTypeWiningNumbersList result = null;

            foreach (RDeepBetTypeWiningNumbersList winningNumbersList in betTypeWinningNumbersLists)
            {
                if (winningNumbersList.ID == ID)
                    result = winningNumbersList;
            }

            return result;
        }

        internal static int BetPayOutFactorByBet(BetTypes bet)
        {
            int result = 0;

            foreach (RDeepBetTypeWiningNumbersList betTypeWinnningNumberList in betTypeWinningNumbersLists)
            {
                if (betTypeWinnningNumberList.betType == bet)
                    result = betTypeWinnningNumberList.Factor;
            }

            return result;
        }
        
        internal static int[] GetBetWinNumbersByBetPosition(RDeepBetPosition betPosition)
        {
            int[] result = null;

            RDeepBetTypeWiningNumbersList betTypeWinnningNumberList = GetWiningNumbersListByBetType(betPosition.betType);
            result = GetWinningNumbersForBet(betPosition.betPositions, betTypeWinnningNumberList.betWiningNumbersList);

            return result;
        }

        private static RDeepBetTypeWiningNumbersList GetWiningNumbersListByBetType(BetTypes bet)
        {
            RDeepBetTypeWiningNumbersList result = null;

            foreach (RDeepBetTypeWiningNumbersList betTypeWinnningNumbersList in RDeepWiningNumbersList.betTypeWinningNumbersLists)
            {
                if (betTypeWinnningNumbersList.betType == bet)
                    result = betTypeWinnningNumbersList;
            }

            return result;
        }

        private static int[] GetWinningNumbersForBet(int[] betPositions, List<int[]> betTypeWinnningNumbersList)
        {
            int[] result = { };

            //Straight up bet
            if(betPositions.Count() == 1 && betPositions[0] <= 37)
            {
                return betPositions;
            }

            foreach (int[] winnningNumbers in betTypeWinnningNumbersList)
            {
                if (IsBetPositionMatchingWinningNumbers(betPositions, winnningNumbers))
                {
                    result = winnningNumbers;
                    break;
                }
            }
            return result;
        }

        private static bool IsBetPositionMatchingWinningNumbers(int[] betPositions, int[] betWinningNumbers)
        {
            bool result = true;
            foreach (int pos in betPositions)
            {
                if (RDeepPositions.GetPositionByID(pos).isWheelNumber)
                {
                    //Winning numbers doesn't belong to bet positions even if one of the wheel number in bet position is NOT in the Winning Numbers
                    if (Utilities.Generic.IsNumberInArray(pos, betWinningNumbers) == false)
                        return false;
                    //Outside Bet: If the bet positions doesn't have wheel numbers, it will never return false.
                }
            }
            return result;
        }

        private static RDeepBetTypeWiningNumbersList LoadBetTypeWiningNumbersList(BetTypes bet)
        {
            List<int[]> BetTypeWiningNumbersList = new List<int[]>();
            int Factor = 0;

            if (bet == BetTypes.StraightUp)
            {
                BetTypeWiningNumbersList = new List<int[]>
                {
                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37 }
                };
                Factor = 35;
            }
            else if (bet == BetTypes.Row || bet == BetTypes.Split)
            {
                BetTypeWiningNumbersList = RDeepBetPositions.GetBetPositions(bet);
                Factor = 17;
            }
            else if (bet == BetTypes.Street)
            {
                BetTypeWiningNumbersList = new List<int[]>
                {
                    new int[] { 0, 1, 2, },     //0, 1, 2
                    new int[] { 37, 2, 3, },    //00, 2, 3
                    new int[] { 1, 2, 3, },
                    new int[] { 4, 5, 6, },
                    new int[] { 7, 8, 9 },
                    new int[] { 10, 11, 12 },
                    new int[] { 13, 14, 15 },
                    new int[] { 16, 17, 18 },
                    new int[] { 19, 20, 21 },
                    new int[] { 22, 23, 24 },
                    new int[] { 28, 29, 30 },
                    new int[] { 31, 32, 33 },
                    new int[] { 34, 35, 36 }
                };

                Factor = 11;
            }
            else if (bet == BetTypes.Corner)
            {
                BetTypeWiningNumbersList = RDeepBetPositions.GetBetPositions(bet);
                Factor = 8;
            }
            else if (bet == BetTypes.TopLineOrBasket)
            {
                BetTypeWiningNumbersList = new List<int[]>
                {
                    new int[] { 0, 37, 2, 3 }, //"0, 00, 1, 2, 3"
                };
                Factor = 6;
            }
            else if (bet == BetTypes.SixLine)
            {
                BetTypeWiningNumbersList = new List<int[]>
                {
                    new int[] { 0, 37, 1, 2, 3 }, //0, 00, 1, 2, 3
                    new int[] { 1, 2, 3, 4, 5, 6 },
                    new int[] { 4, 5, 6, 7, 8, 9 },
                    new int[] { 7, 8, 9, 10, 11, 12  },
                    new int[] { 10, 11, 12, 13, 14, 15 },
                    new int[] { 13, 14, 15, 16, 17, 18 },
                    new int[] { 16, 17, 18, 19, 20, 21 },
                    new int[] { 19, 20, 21, 22, 23, 24 },
                    new int[] { 22, 23, 24, 25, 26, 27 },
                    new int[] { 25, 26, 27, 28, 29, 30 },
                    new int[] { 28, 29, 30, 31, 32, 33 },
                    new int[] { 31, 32, 33, 34, 35, 36 }
                };
                Factor = 6;
            }
            else if (bet == BetTypes.FirstDozen)
            {

                BetTypeWiningNumbersList = new List<int[]>
                {
                    new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }    // 1 to 12
                };
                Factor = 2;
            }
            else if (bet == BetTypes.SecondDozen)
            {

                BetTypeWiningNumbersList = new List<int[]>
                {
                    new int[] { 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 }    // 13 to 24
                };
                Factor = 2;
            }
            else if (bet == BetTypes.ThirdDozen)
            {

                BetTypeWiningNumbersList = new List<int[]>
                {
                    new int[] { 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36 }    // 25 to 36
                };
                Factor = 2;
            }
            else if (bet == BetTypes.FirstColumn)
            {
                BetTypeWiningNumbersList = new List<int[]>
                {
                    new int[] { 1, 4, 7, 10, 13, 16, 19, 22, 25, 28, 31, 34 }    //
                };
                Factor = 2;
            }
            else if (bet == BetTypes.SecondColumn)
            {
                BetTypeWiningNumbersList = new List<int[]>
                {
                    new int[] { 2, 5, 8, 11, 14, 17, 20, 23, 26, 29, 32, 35 }    //
                };
                Factor = 2;
            }
            else if (bet == BetTypes.ThirdColumn)
            {
                BetTypeWiningNumbersList = new List<int[]>
                {
                    new int[] { 3, 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36 }    //
                };
                Factor = 2;
            }
            else if (bet == BetTypes.Odd)
            {
                BetTypeWiningNumbersList = new List<int[]>
                {
                    new int[] { 1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25, 27, 29, 31, 33, 35 }    //Odd numbers 1, 3, 5 .... 35
                };
                Factor = 1;
            }
            else if (bet == BetTypes.Even)
            {
                BetTypeWiningNumbersList = new List<int[]>
                {
                    new int[] { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36 }    //Even numbers 2, 4, 6 .... 36
                };
                Factor = 1;
            }
            else if (bet == BetTypes.Red)
            {
                BetTypeWiningNumbersList = new List<int[]>
                {
                    new int[] { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 }
                };
                Factor = 1;
            }
            else if (bet == BetTypes.Black)
            {
                BetTypeWiningNumbersList = new List<int[]>
                {
                    new int[] { 2, 4, 6, 8, 10, 11, 13, 15, 17, 20, 22, 24, 26, 28, 29, 31, 33, 35 }
                };
                Factor = 1;
            }
            else if (bet == BetTypes.OneToEighteen)
            {
                BetTypeWiningNumbersList = new List<int[]>
                {
                    new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 }    // 1 to 18
                };
                Factor = 1;
            }
            else if (bet == BetTypes.NinteenToThirtyEight)
            {
                BetTypeWiningNumbersList = new List<int[]>
                {
                    new int[] { 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36 }    // 19 to 36
                };
                Factor = 1;
            }
            return new RDeepBetTypeWiningNumbersList(Factor, bet, BetTypeWiningNumbersList);
        }
    }
}