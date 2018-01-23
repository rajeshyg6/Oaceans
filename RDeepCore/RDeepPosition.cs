using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDeepCore
{
    public enum PositionTypeCategory
    {
        Straight,
        Even,
        Third
    }

    public enum PositionTypeSubCategory
    {
        Straight,
        Color,
        OddEven,
        LowHigh,
        Dozen,
        Column
    }

    public enum PositionType
    {
        Straight,
        Red,
        Black,
        Odd,
        Even,
        Low,
        High,
        FirstDozen,
        SecondDozen,
        ThirdDozen,
        FirstColumn,
        SecondColumn,
        ThirdColumn,
        Green
    }

    public class RDeepPositions
    {
        public static List<RDeepPosition> rDeepPositions;

        public RDeepPositions()
        {
            AddRDeepPositions();
            AssignDefaultProbabilities();
        }

        private static void AddRDeepPositions()
        {
            rDeepPositions = new List<RDeepPosition>();
            rDeepPositions.Add(RDeepPosition.Zero);
            rDeepPositions.Add(RDeepPosition.DoubleZero);
            rDeepPositions.Add(RDeepPosition.One);
            rDeepPositions.Add(RDeepPosition.Two);
            rDeepPositions.Add(RDeepPosition.Three);
            rDeepPositions.Add(RDeepPosition.Four);
            rDeepPositions.Add(RDeepPosition.Five);
            rDeepPositions.Add(RDeepPosition.Six);
            rDeepPositions.Add(RDeepPosition.Seven);
            rDeepPositions.Add(RDeepPosition.Eight);
            rDeepPositions.Add(RDeepPosition.Nine);
            rDeepPositions.Add(RDeepPosition.Ten);
            rDeepPositions.Add(RDeepPosition.Eleven);
            rDeepPositions.Add(RDeepPosition.Twelve);
            rDeepPositions.Add(RDeepPosition.Thirteen);
            rDeepPositions.Add(RDeepPosition.Forteen);
            rDeepPositions.Add(RDeepPosition.Fifteen);
            rDeepPositions.Add(RDeepPosition.Sixteen);
            rDeepPositions.Add(RDeepPosition.Seventeen);
            rDeepPositions.Add(RDeepPosition.Eighteen);
            rDeepPositions.Add(RDeepPosition.Ninteen);
            rDeepPositions.Add(RDeepPosition.Twenty);
            rDeepPositions.Add(RDeepPosition.TwentyOne);
            rDeepPositions.Add(RDeepPosition.TwentyTwo);
            rDeepPositions.Add(RDeepPosition.TwentyThree);
            rDeepPositions.Add(RDeepPosition.TwentyFour);
            rDeepPositions.Add(RDeepPosition.TwentyFive);
            rDeepPositions.Add(RDeepPosition.TwentySix);
            rDeepPositions.Add(RDeepPosition.TwentySeven);
            rDeepPositions.Add(RDeepPosition.TwentyEight);
            rDeepPositions.Add(RDeepPosition.TwentyNine);
            rDeepPositions.Add(RDeepPosition.Thirty);
            rDeepPositions.Add(RDeepPosition.ThirtyOne);
            rDeepPositions.Add(RDeepPosition.ThirtyTwo);
            rDeepPositions.Add(RDeepPosition.ThirtyThree);
            rDeepPositions.Add(RDeepPosition.ThirtyFour);
            rDeepPositions.Add(RDeepPosition.ThirtyFive);
            rDeepPositions.Add(RDeepPosition.ThirtySix);
            rDeepPositions.Add(RDeepPosition.FirstDozen);
            rDeepPositions.Add(RDeepPosition.SecondDozen);
            rDeepPositions.Add(RDeepPosition.ThirdDozen);
            rDeepPositions.Add(RDeepPosition.OneToEighteen);
            rDeepPositions.Add(RDeepPosition.NineteenToThirtySix);
            rDeepPositions.Add(RDeepPosition.FirstColumn);
            rDeepPositions.Add(RDeepPosition.SecondColumn);
            rDeepPositions.Add(RDeepPosition.ThirdColumn);
            rDeepPositions.Add(RDeepPosition.Odd);
            rDeepPositions.Add(RDeepPosition.Even);
            rDeepPositions.Add(RDeepPosition.Red);
            rDeepPositions.Add(RDeepPosition.Black);
        }

        private static void AssignDefaultProbabilities()
        {
            IEnumerable<RDeepPosition> wheelNumbers = RDeepPositions.rDeepPositions.Where(pos => pos.isWheelNumber == true);

            int wheelNumbersCount = wheelNumbers.Count();
            foreach (RDeepPosition number in wheelNumbers)
            {
                number.defaultProbability = (float)100 / wheelNumbersCount;
            }
        }

        internal static RDeepPosition GetPositionByID(int ID)
        {
            RDeepPosition result = null;
            foreach (RDeepPosition position in rDeepPositions)
            {
                if (position.ID == ID)
                {
                    result = position;
                    break;
                }
            }
            return result;
        }

        internal static IEnumerable<RDeepPosition> PositionNumbers()
        {
            return NumbersByPositionType(PositionType.Straight);
        }

        internal static IEnumerable<RDeepPosition> NumbersByPositionType(PositionType positionType)
        {
            List<RDeepPosition> result = new List<RDeepPosition>();
            
            switch (positionType)
            {
                case PositionType.Straight:
                    foreach (RDeepPosition pos in rDeepPositions)
                    {
                        if (pos.isWheelNumber == true)
                            result.Add(pos);
                    }
                    break;
                case PositionType.Red:
                    foreach (RDeepPosition pos in rDeepPositions)
                    {
                        if (pos.isRed == true)
                            result.Add(pos);
                    }
                    break;
                case PositionType.Black:
                    foreach (RDeepPosition pos in rDeepPositions)
                    {
                        if (pos.isBlack == true)
                            result.Add(pos);
                    }
                    break;
                case PositionType.Even:
                    foreach (RDeepPosition pos in rDeepPositions)
                    {
                        if (pos.isEven == true)
                            result.Add(pos);
                    }
                    break;
                case PositionType.Odd:
                    foreach (RDeepPosition pos in rDeepPositions)
                    {
                        if (pos.isOdd == true)
                            result.Add(pos);
                    }
                    break;
                case PositionType.Low:
                    foreach (RDeepPosition pos in rDeepPositions)
                    {
                        if (pos.isLow == true)
                            result.Add(pos);
                    }
                    break;
                case PositionType.High:
                    foreach (RDeepPosition pos in rDeepPositions)
                    {
                        if (pos.isHigh == true)
                            result.Add(pos);
                    }
                    break;
                case PositionType.FirstDozen:
                    foreach (RDeepPosition pos in rDeepPositions)
                    {
                        if (pos.isFirstDozen == true)
                            result.Add(pos);
                    }
                    break;
                case PositionType.SecondDozen:
                    foreach (RDeepPosition pos in rDeepPositions)
                    {
                        if (pos.isSecondDozen == true)
                            result.Add(pos);
                    }
                    break;
                case PositionType.ThirdDozen:
                    foreach (RDeepPosition pos in rDeepPositions)
                    {
                        if (pos.isThirdDozen == true)
                            result.Add(pos);
                    }
                    break;
                case PositionType.FirstColumn:
                    foreach (RDeepPosition pos in rDeepPositions)
                    {
                        if (pos.isFirstColumn == true)
                            result.Add(pos);
                    }
                    break;
                case PositionType.SecondColumn:
                    foreach (RDeepPosition pos in rDeepPositions)
                    {
                        if (pos.isSecondColumn == true)
                            result.Add(pos);
                    }
                    break;
                case PositionType.ThirdColumn:
                    foreach (RDeepPosition pos in rDeepPositions)
                    {
                        if (pos.isThirdColumn == true)
                            result.Add(pos);
                    }
                    break;
            }

            return result;
        }

        public static string PositionsSummary(int[] Positions)
        {
            string result = "";
            string outSidePos = "";
            string numbers = "";

            foreach (int position in Positions)
            {
                RDeepPosition pos;
                pos = GetPositionByID(position);
                if (pos.isWheelNumber)
                {
                    if (numbers == "")
                        numbers = "Numbers:";

                    numbers += " " + pos.Name + ";";
                }
                else
                {
                    if (outSidePos == "")
                        outSidePos = "Outsides: ";

                    outSidePos += " " + pos.Name + ";";
                }
            }
            if (outSidePos != "")
                result += outSidePos + "\t";
            result += numbers;

            return result;
        }
    }

    public class RDeepPosition
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool isWheelNumber { get; set; }
        
        public bool isRed { get; set; }
        public bool isBlack { get; set; }
        public bool isLow { get; set; }
        public bool isHigh { get; set; }
        public bool isOdd { get; set; }
        public bool isEven { get; set; }
        public bool isFirstDozen { get; set; }
        public bool isSecondDozen { get; set; }
        public bool isThirdDozen { get; set; }
        public bool isFirstColumn { get; set; }
        public bool isSecondColumn { get; set; }
        public bool isThirdColumn { get; set; }
        public bool isGreen { get; set; }
        
        public PositionType Color { get; set; }
        public PositionType OddEven { get; set; }
        public PositionType LowHigh { get; set; }
        public PositionType Dozen { get; set; }
        public PositionType Column { get; set; }

        public float defaultProbability { get; set; }

        public RDeepPosition(int id, string name, bool iswheelnumber)
        {
            ID = id;
            Name = name;

            if (iswheelnumber)
            {
                isWheelNumber = true;
                Color = OddEven = LowHigh = Dozen = Column = PositionType.Straight;
            }
            
            isRed = false;
            isBlack = false;
            isLow = false;
            isHigh = false;
            isOdd = false;
            isEven = false;
            isFirstDozen = false;
            isSecondDozen = false;
            isThirdDozen = false;
            isFirstColumn = false;
            isSecondColumn = false;
            isThirdColumn = false;
            isGreen = false;
            SetFlags();
        }

        private void SetFlags()
        {
            if (ID == 0 || ID == 37)
            {
                isGreen = true;
                Color = OddEven = LowHigh = Dozen = Column = PositionType.Green;
            }
            else
            {
                if (Convert.ToInt32(ID) >= 1 && Convert.ToInt32(ID) <= 18)
                {
                    isLow = true;
                    LowHigh = PositionType.Low;
                }
                else if (Convert.ToInt32(ID) >= 19 && Convert.ToInt32(ID) <= 36)
                {
                    isHigh = true;
                    LowHigh = PositionType.High;
                }

                if (Convert.ToInt32(ID) >= 1 && Convert.ToInt32(ID) <= 12)
                {
                    isFirstDozen = true;
                    Dozen = PositionType.FirstDozen;
                }
                else if (Convert.ToInt32(ID) >= 13 && Convert.ToInt32(ID) <= 24)
                {
                    isSecondDozen = true;
                    Dozen = PositionType.SecondDozen;
                }
                else if (Convert.ToInt32(ID) >= 25 && Convert.ToInt32(ID) <= 36)
                {
                    isThirdDozen = true;
                    Dozen = PositionType.ThirdDozen;
                }

                if (Convert.ToInt32(ID) % 2 == 0 && isWheelNumber)
                {
                    isEven = true;
                    OddEven = PositionType.Even;
                }
                else
                {
                    isOdd = isWheelNumber;
                    OddEven = PositionType.Odd;
                }

                if (Convert.ToInt32(ID) % 3 == 0 && isWheelNumber)
                {
                    isThirdColumn = true;
                    Column = PositionType.ThirdColumn;
                }
                else if (ID == 2 || ID == 5 || ID == 8 || ID == 11 || ID == 14 || ID == 17 || ID == 20 || ID == 23 || ID == 26 || ID == 29 || ID == 32 || ID == 35)
                {
                    isSecondColumn = true;
                    Column = PositionType.SecondColumn;
                }
                else
                {
                    isFirstColumn = isWheelNumber;
                    Column = PositionType.FirstColumn;
                }

                if (ID == 1 || ID == 3 || ID == 5 || ID == 7 || ID == 9 || ID == 12 || ID == 14 || ID == 16 || ID == 18 || ID == 19 || ID == 21 || ID == 23 || ID == 25 || ID == 27 || ID == 30 || ID == 32 || ID == 34 || ID == 36)
                {
                    isRed = true;
                    Color = PositionType.Red;
                }
                else
                {
                    isBlack = isWheelNumber;
                    Color = PositionType.Black;
                }
            }
        }

        public static readonly RDeepPosition Zero =                  new RDeepPosition(0, " 0", true);
        public static readonly RDeepPosition DoubleZero =            new RDeepPosition(37, "00", true);
        public static readonly RDeepPosition One =                   new RDeepPosition(1, " 1", true);
        public static readonly RDeepPosition Two =                   new RDeepPosition(2, " 2", true);
        public static readonly RDeepPosition Three =                 new RDeepPosition(3, " 3", true);
        public static readonly RDeepPosition Four =                  new RDeepPosition(4, " 4", true);
        public static readonly RDeepPosition Five =                  new RDeepPosition(5, " 5", true);
        public static readonly RDeepPosition Six =                   new RDeepPosition(6, " 6", true);
        public static readonly RDeepPosition Seven =                 new RDeepPosition(7, " 7", true);
        public static readonly RDeepPosition Eight =                 new RDeepPosition(8, " 8", true);
        public static readonly RDeepPosition Nine =                  new RDeepPosition(9, " 9", true);
        public static readonly RDeepPosition Ten =                   new RDeepPosition(10, "10", true);
        public static readonly RDeepPosition Eleven =                new RDeepPosition(11, "11", true);
        public static readonly RDeepPosition Twelve =                new RDeepPosition(12, "12", true);
        public static readonly RDeepPosition Thirteen =              new RDeepPosition(13, "13", true);
        public static readonly RDeepPosition Forteen =               new RDeepPosition(14, "14", true);
        public static readonly RDeepPosition Fifteen =               new RDeepPosition(15, "15", true);
        public static readonly RDeepPosition Sixteen =               new RDeepPosition(16, "16", true);
        public static readonly RDeepPosition Seventeen =             new RDeepPosition(17, "17", true);
        public static readonly RDeepPosition Eighteen =              new RDeepPosition(18, "18", true);
        public static readonly RDeepPosition Ninteen =               new RDeepPosition(19, "19", true);
        public static readonly RDeepPosition Twenty =                new RDeepPosition(20, "20", true);
        public static readonly RDeepPosition TwentyOne=              new RDeepPosition(21, "21", true);
        public static readonly RDeepPosition TwentyTwo =             new RDeepPosition(22, "22", true);
        public static readonly RDeepPosition TwentyThree =           new RDeepPosition(23, "23", true);
        public static readonly RDeepPosition TwentyFour =            new RDeepPosition(24, "24", true);
        public static readonly RDeepPosition TwentyFive =            new RDeepPosition(25, "25", true);
        public static readonly RDeepPosition TwentySix =             new RDeepPosition(26, "26", true);
        public static readonly RDeepPosition TwentySeven =           new RDeepPosition(27, "27", true);
        public static readonly RDeepPosition TwentyEight =           new RDeepPosition(28, "28", true);
        public static readonly RDeepPosition TwentyNine =            new RDeepPosition(29, "29", true);
        public static readonly RDeepPosition Thirty =                new RDeepPosition(30, "30", true);
        public static readonly RDeepPosition ThirtyOne =             new RDeepPosition(31, "31", true);
        public static readonly RDeepPosition ThirtyTwo =             new RDeepPosition(32, "32", true);
        public static readonly RDeepPosition ThirtyThree =           new RDeepPosition(33, "33", true);
        public static readonly RDeepPosition ThirtyFour =            new RDeepPosition(34, "34", true);
        public static readonly RDeepPosition ThirtyFive =            new RDeepPosition(35, "35", true);
        public static readonly RDeepPosition ThirtySix =             new RDeepPosition(36, "36", true);
        public static readonly RDeepPosition FirstDozen =            new RDeepPosition(38, "1st12", false);
        public static readonly RDeepPosition SecondDozen =           new RDeepPosition(39, "2nd12", false);
        public static readonly RDeepPosition ThirdDozen =            new RDeepPosition(40, "3rd12", false);
        public static readonly RDeepPosition OneToEighteen =         new RDeepPosition(41, "1-18", false);
        public static readonly RDeepPosition NineteenToThirtySix =   new RDeepPosition(42, "19-36", false);
        public static readonly RDeepPosition FirstColumn =           new RDeepPosition(43, "1stColumn", false);
        public static readonly RDeepPosition SecondColumn =          new RDeepPosition(44, "2ndColumn", false);
        public static readonly RDeepPosition ThirdColumn =           new RDeepPosition(45, "3rdColumn", false);
        public static readonly RDeepPosition Odd =                   new RDeepPosition(46, "Odd", false);
        public static readonly RDeepPosition Even =                  new RDeepPosition(47, "Even", false);
        public static readonly RDeepPosition Red =                   new RDeepPosition(48, "Red", false);
        public static readonly RDeepPosition Black =                 new RDeepPosition(49, "Black", false);
    }
}
