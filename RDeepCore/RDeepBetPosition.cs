using System;
using System.Collections.Generic;

namespace RDeepCore
{
    public class RDeepBetPosition
    {
        public BetTypes betType { get; set; }
        public int[] betPositions;
        public int[] winningPositions;
        public int IdealProbabilityPct;
        public int ID;

        public RDeepBetPosition(BetTypes bet, int[] positions)
        {
            betPositions = positions;
            betType = bet;
            ID = ++RDeepBetPositions.Total;
        }

        public static string BetPositionsSummary(int[] Positions)
        {
            return RDeepPositions.PositionsSummary(Positions);
        }
    }

    public class RDeepBetPositions
    {
        public static List<RDeepBetPosition> betPositionList;
        public static int Total = 0;

        public RDeepBetPositions()
        {
            betPositionList = new List<RDeepBetPosition>();
            foreach (BetTypes bet in Enum.GetValues(typeof(BetTypes)))
            {
                if (bet == BetTypes.StraightUp)
                {
                    for (int i = 0; i < 38; i++)
                        LoadRDeepBetPositions(bet, i);
                }
                else
                {
                    LoadRDeepBetPositions(bet);
                }
            }
        }

        public static List<int[]> GetBetPositions(BetTypes bet)
        {
            List<int[]> betPositions = new List<int[]>();
            foreach(RDeepBetPosition p in betPositionList)
            {
                if (p.betType == bet)
                    betPositions.Add(p.betPositions);
            }
            return betPositions;
        }

        internal static RDeepBetPosition GetRDeepBetPositionByID(int ID)
        {
            RDeepBetPosition result = null;

            foreach (RDeepBetPosition betPosition in RDeepBetPositions.betPositionList)
            {
                if (betPosition.ID == ID)
                    result = betPosition;
            }
            return result;
        }

        internal static RDeepBetPosition GetRDeepBetPositionByPositionIDs(int[] positions)
        {
            RDeepBetPosition result = null;

            foreach (RDeepBetPosition betPosition in RDeepBetPositions.betPositionList)
            {
                bool hasMismatches = false;
                foreach (int pos in positions)
                {
                    if (Utilities.Generic.IsNumberInArray(pos, betPosition.betPositions) == false)
                    {
                        hasMismatches = true;
                        break;
                    }
                }
                if (hasMismatches == false)
                    return betPosition;
            }
            return result;
        }

        public int GetFactor(int BetPosID)
        {
            return RDeepWiningNumbersList.BetPayOutFactorByBet(GetRDeepBetPositionByID(BetPosID).betType);
        }

        public static bool ValidateBet(int BetPositionID, int CurrentNumber)
        {
            RDeepBetPosition RDeepBetPosition = RDeepBetPositions.GetRDeepBetPositionByID(BetPositionID);
            return ValidateBet(RDeepBetPosition, CurrentNumber);
        }

        public static bool ValidateBet(RDeepBetPosition RDeepBetPosition, int CurrentNumber)
        {
            bool result = false;
            
            int[] betWinPositions = RDeepWiningNumbersList.GetBetWinNumbersByBetPosition(RDeepBetPosition);

            //Is Current number in the Winning positions of the bet
            if (Utilities.Generic.IsNumberInArray(
                CurrentNumber,
                betWinPositions
                )) result = true;

            return result;
        }

        private static void LoadRDeepBetPositions(BetTypes BetType, int position = -1)
        {
            List<int[]> betPositions;
            betPositions = new List<int[]>();

            switch (BetType)
            {
                case BetTypes.StraightUp:
                    betPositions.Add(new int[] { position });
                    break;
                case BetTypes.Row:
                    betPositions = new List<int[]>
                    {
                        new int[] { 0, 37 },

                        new int[] { 0, 1 },
                        new int[] { 0, 2 },

                        new int[] { 37, 2 },
                        new int[] { 37, 3 }
                    };
                    break;
                case BetTypes.Split:
                    betPositions = new List<int[]>
                    {
                        new int[] { 1, 2 },
                        new int[] { 2, 3 },

                        new int[] { 1,4 },
                        new int[] { 2,5 },
                        new int[] { 3,6 },

                        new int[] { 4, 5 },
                        new int[] { 5, 6 },

                        new int[] { 4, 7 },
                        new int[] { 5, 8 },
                        new int[] { 6, 9 },

                        new int[] { 7, 8 },
                        new int[] { 8, 9 },

                        new int[] { 7, 10 },
                        new int[] { 8, 11 },
                        new int[] { 9, 12 },

                        new int[] { 10, 11 },
                        new int[] { 11, 12 },

                        new int[] { 10, 13 },
                        new int[] { 11, 14 },
                        new int[] { 12, 15 },

                        new int[] { 13, 14 },
                        new int[] { 14, 15 },

                        new int[] { 13, 16 },
                        new int[] { 14, 17 },
                        new int[] { 15, 18 },

                        new int[] { 16, 17 },
                        new int[] { 17, 18 },

                        new int[] { 16, 19 },
                        new int[] { 17, 20 },
                        new int[] { 18, 21 },

                        new int[] { 19, 20 },
                        new int[] { 20, 21 },

                        new int[] { 19, 22 },
                        new int[] { 20, 23 },
                        new int[] { 21, 24 },

                        new int[] { 22, 23 },
                        new int[] { 23, 24 },

                        new int[] { 22, 25 },
                        new int[] { 23, 26 },
                        new int[] { 24, 27 },

                        new int[] { 25, 26 },
                        new int[] { 26, 27 },

                        new int[] { 25, 28 },
                        new int[] { 26, 29 },
                        new int[] { 27, 30 },

                        new int[] { 28, 29 },
                        new int[] { 29, 30 },

                        new int[] { 28, 31 },
                        new int[] { 29, 32 },
                        new int[] { 30, 33 },

                        new int[] { 31, 32 },
                        new int[] { 32, 33 },

                        new int[] { 31, 34 },
                        new int[] { 32, 35 },
                        new int[] { 33, 36 },

                        new int[] { 34, 35 },
                        new int[] { 35, 36 }
                    };
                    break;
                case BetTypes.Street:
                    betPositions.Add(new int[] { 1, RDeepPosition.FirstDozen.ID });
                    betPositions.Add(new int[] { 4, RDeepPosition.FirstDozen.ID });
                    betPositions.Add(new int[] { 7, RDeepPosition.FirstDozen.ID });
                    betPositions.Add(new int[] { 10, RDeepPosition.FirstDozen.ID });
                    betPositions.Add(new int[] { 13, RDeepPosition.SecondDozen.ID });
                    betPositions.Add(new int[] { 16, RDeepPosition.SecondDozen.ID });
                    betPositions.Add(new int[] { 19, RDeepPosition.SecondDozen.ID });
                    betPositions.Add(new int[] { 22, RDeepPosition.SecondDozen.ID });
                    betPositions.Add(new int[] { 25, RDeepPosition.ThirdDozen.ID });
                    betPositions.Add(new int[] { 28, RDeepPosition.ThirdDozen.ID });
                    betPositions.Add(new int[] { 31, RDeepPosition.ThirdDozen.ID });
                    betPositions.Add(new int[] { 34, RDeepPosition.ThirdDozen.ID });
                    break;
                case BetTypes.Corner:

                    betPositions = new List<int[]>
                    {
                        new int[] { 1, 2, 4, 5 },
                        new int[] { 2, 3, 5, 6 },
                        new int[] { 4, 5, 7, 8 },
                        new int[] { 5, 6, 8, 9 },
                        new int[] { 7, 8, 10, 11 },
                        new int[] { 8, 9, 11, 12 },
                        new int[] { 10, 11, 13, 14 },
                        new int[] { 11, 12, 14, 15 },
                        new int[] { 13, 14, 16, 17 },
                        new int[] { 14, 15, 17, 18 },
                        new int[] { 16, 17, 19, 20 },
                        new int[] { 17, 18, 20, 21 },
                        new int[] { 19, 20, 22, 23 },
                        new int[] { 20, 21, 23, 24 },
                        new int[] { 22, 23, 25, 26 },
                        new int[] { 23, 24, 26, 27 },
                        new int[] { 25, 26, 28, 29 },
                        new int[] { 26, 27, 29, 30 },
                        new int[] { 28, 29, 31, 32 },
                        new int[] { 29, 30, 32, 33 },
                        new int[] { 31, 32, 34, 35 },
                        new int[] { 32, 33, 35, 36 }
                    };
                    break;
                case BetTypes.TopLineOrBasket:
                    betPositions.Add(new int[] { 0, 1, RDeepPosition.FirstDozen.ID });
                    break;
                case BetTypes.SixLine:
                    betPositions.Add(new int[] { 1, 4, RDeepPosition.FirstDozen.ID });
                    betPositions.Add(new int[] { 4, 7, RDeepPosition.FirstDozen.ID });
                    betPositions.Add(new int[] { 7, 10, RDeepPosition.FirstDozen.ID });
                    betPositions.Add(new int[] { 10, 13, RDeepPosition.FirstDozen.ID, RDeepPosition.SecondDozen.ID });

                    betPositions.Add(new int[] { 13, 16, RDeepPosition.SecondDozen.ID });
                    betPositions.Add(new int[] { 16, 19, RDeepPosition.SecondDozen.ID });
                    betPositions.Add(new int[] { 19, 22, RDeepPosition.SecondDozen.ID });
                    betPositions.Add(new int[] { 22, 25, RDeepPosition.SecondDozen.ID, RDeepPosition.ThirdDozen.ID });

                    betPositions.Add(new int[] { 25, 28, RDeepPosition.ThirdDozen.ID });
                    betPositions.Add(new int[] { 28, 31, RDeepPosition.ThirdDozen.ID });
                    betPositions.Add(new int[] { 31, 34, RDeepPosition.ThirdDozen.ID });
                    break;
                case BetTypes.FirstDozen:
                    betPositions.Add(new int[] { RDeepPosition.FirstDozen.ID });
                    break;
                case BetTypes.SecondDozen:
                    betPositions.Add(new int[] { RDeepPosition.SecondDozen.ID });
                    break;
                case BetTypes.ThirdDozen:
                    betPositions.Add(new int[] { RDeepPosition.ThirdDozen.ID });
                    break;
                case BetTypes.FirstColumn:
                    betPositions.Add(new int[] { RDeepPosition.FirstColumn.ID });
                    break;
                case BetTypes.SecondColumn:
                    betPositions.Add(new int[] { RDeepPosition.SecondColumn.ID });
                    break;
                case BetTypes.ThirdColumn:
                    betPositions.Add(new int[] { RDeepPosition.ThirdColumn.ID });
                    break;
                case BetTypes.Odd:
                    betPositions.Add(new int[] { RDeepPosition.Odd.ID });
                    break;
                case BetTypes.Even:
                    betPositions.Add(new int[] { RDeepPosition.Even.ID });
                    break;
                case BetTypes.Black:
                    betPositions.Add(new int[] { RDeepPosition.Black.ID });
                    break;
                case BetTypes.Red:
                    betPositions.Add(new int[] { RDeepPosition.Red.ID });
                    break;
                case BetTypes.OneToEighteen:
                    betPositions.Add(new int[] { RDeepPosition.OneToEighteen.ID });
                    break;
                case BetTypes.NinteenToThirtyEight:
                    betPositions.Add(new int[] { RDeepPosition.NineteenToThirtySix.ID });
                    break;
            }

            foreach (int[] betPosition in betPositions)
            {
                betPositionList.Add(new RDeepBetPosition(BetType, betPosition));
            }
        }
    }
}