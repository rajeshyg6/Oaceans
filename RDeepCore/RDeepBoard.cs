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
    public class RDeepBoard
    {
        internal RDeepDealer boardDealer;
        internal List<RDeepPlayer> boardPlayers;
        public List<RDeepPosition> LastNumbers;
        public RDeepPosition CurrentNumber;
        public List<RDeepBet> bets;
        
        public List<RDeepPosition> BoardPositions;// = new List<RDeepPosition>();

        public RDeepBoard()
        {
            boardPlayers = new List<RDeepPlayer>();
            LastNumbers = new List<RDeepPosition>();
            bets = new List<RDeepBet>();

            LoadBoardBetPositions();
            CallForPlayers();
        }

        private void LoadBoardBetPositions()
        {
            RDeepPositions RDeepPositions = new RDeepPositions();
            RDeepBetPositions betPos = new RDeepBetPositions();
            RDeepWiningNumbersList betWinPos = new RDeepWiningNumbersList();
        }

        private void CallForPlayers()
        {
            RDeepPlayer RDeepPlayer = new RDeepPlayer("Mr.A", 5000, "Random");
            boardPlayers.Add(RDeepPlayer);

            RDeepPlayer RDeepPlayer2 = new RDeepPlayer("Mr.B", 5000, "TenFifteen");
            boardPlayers.Add(RDeepPlayer2);

            /*
            RDeepPlayer RDeepPlayer3 = new RDeepPlayer("Mr.C", 5000);
            boardPlayers.Add(RDeepPlayer3);

            RDeepPlayer RDeepPlayer4 = new RDeepPlayer("Mr.D", 5000);
            boardPlayers.Add(RDeepPlayer4);*/

            boardDealer = new RDeepDealer("Mr. Dealer", 100000);

            //Participant.TransferCoins(RDeepPlayer, dealer, RDeepPlayer.TakeCoins(CoinType.One, 2).ToList());
        }

        private void AssignDealer(RDeepDealer dealer)
        {
            boardDealer = dealer;
        }

        private void JoinPlayer(RDeepPlayer player)
        {
            boardPlayers.Add(player);
        }

        public void CallForBets()
        {
            if (boardPlayers.Count < 1)
                throw new Exception("No players");

            foreach (RDeepPlayer player in boardPlayers)
            {
                bets.AddRange(player.GoForBet(LastNumbers).ToList());
                System.Threading.Thread.Sleep(100);
            }
        }

        public void Spin()
        {
            if (bets.Count < 1)
                throw new Exception("No bets");

            CurrentNumber = RDeepPositions.GetPositionByID(Generic.GetRandomNumber(0, 38));

            LastNumbers.Add(CurrentNumber);

            ValidateBetsForPlayers();

            SettleBets();
        }

        private void SettleBets()
        {
            foreach (RDeepPlayer player in boardPlayers)
            {
                foreach (RDeepBet bet in bets.Where(bet => bet.Player.ID == player.ID))
                {
                    Coins.SetBetStatus(bet.BetCoins, false);

                    int Factor = 1;

                    List<Coin> CoinsToDealer = new List<Coin>();
                    List<Coin> CoinsToPlayer = new List<Coin>();

                    if (bet.status == BetStatus.Won)
                    {
                        Factor = RDeepWiningNumbersList.BetPayOutFactorByBet(bet.BetPosition.betType);
                        foreach (IEnumerable<Coin> CoinsOfType in bet.BetCoins.GroupBy(coin => coin.CT))
                        {
                            CoinsToPlayer.AddRange(RDeepPlayer.TakeCoins(
                                boardDealer,
                                CoinsOfType.First().CT,
                                CoinsOfType.Count() * Factor));
                        }
                    }
                    else
                    {
                        CoinsToDealer.AddRange(bet.BetCoins);
                    }

                    if (CoinsToDealer.Count > 0)
                    {
                        RDeepPlayer.TransferCoins(bet.Player, boardDealer, CoinsToDealer);
                    }

                    if (CoinsToPlayer.Count > 0)
                        RDeepPlayer.TransferCoins(boardDealer, bet.Player, CoinsToPlayer);
                }
            }
        }

        private void ValidateBetsForPlayers()
        {
            foreach (RDeepBet bet in bets)
            {
                if (RDeepBetPositions.ValidateBet(bet.BetPosition, CurrentNumber.ID))
                    bet.status = BetStatus.Won;
                else
                    bet.status = BetStatus.Lost;
            }
        }

        public string DisplayBoardInfo()
        {
            string Result = "";
            Result += RDeepPlayer.ShowPlayerCoins(boardPlayers);
            Result += boardDealer.ShowDealerCoins();
            return Result;
        }

        public string DisplayBetAndWinningNumbers()
        {
            string result = "***** Summary of roll *****\n";

            result += "\n\t** Current number is " + CurrentNumber.Name;

            foreach (RDeepPlayer player in boardPlayers)
            {
                result += "\nPlayer: " + player.Name;
                result += "\n\t -> Bet details: ";

                foreach (RDeepBet bet in bets.Where(bet => bet.Player.ID == player.ID))
                {
                    result += bet.DisplayBet();
                    int[] WinningNumbers = RDeepWiningNumbersList.GetBetWinNumbersByBetPosition(bet.BetPosition);
                    result += "\n\tPlayer bet winning numbers: " + RDeepPositions.PositionsSummary(WinningNumbers);
                    int Factor = 1;

                    if (bet.status == BetStatus.Won)
                        Factor = RDeepWiningNumbersList.BetPayOutFactorByBet(bet.BetPosition.betType);

                    result += "\n\tStatus: ";
                    if (bet.status == BetStatus.Lost) result += " :( ";
                    if (bet.status == BetStatus.Won) result += " :) ";
                    result += bet.status.ToString() + " $" + Factor * Coins.CoinsValue(bet.BetCoins);
                }
            }
            return result;
        }

        public string DisplayLastNumbers()
        {
            string result = "";

            foreach (RDeepPosition number in LastNumbers)
                result += number.Name + ", ";

            return result;
        }

        public string DisplayPlayersProbabilities()
        {
            string result = "";

            foreach (RDeepPlayer player in boardPlayers)
            {
                result += "\nPlayer: " + player.Name + "\n";
                result += player.DisplayProbability();
            }
            return result;
        }

        public string DisplayBoardPlayersCoinsValue()
        {
            string result = "";
            int TotalValue = 0;
            int InitialValue = 0;

            foreach (RDeepPlayer player in boardPlayers)
            {
                result += "\nPlayer: " + player.Name;
                result += player.DisplayPlayerCoinsValue();
                TotalValue += player.PlayerCoinsValue();
                InitialValue += player.InitialCash;
            }

            result += "\n" + boardDealer.Name;
            result += boardDealer.DisplayPlayerCoinsValue();
            TotalValue += boardDealer.PlayerCoinsValue();
            InitialValue += boardDealer.InitialCash;

            if (TotalValue != InitialValue)
                result += "\nXXXXXX Mismatch XXXXXX:\tIntialCash " + InitialValue.ToString() + "\tTotalCash " + TotalValue.ToString();

            return result;
        }

        public void ClearBets()
        {
            bets.Clear();
        }

        #region Factors

        private int StraightUpBetFactor(string Position)
        {
            int Result = 0;

            if (Convert.ToInt32(Position) >= 0 && Convert.ToInt32(Position) <= 36)
                return Result = 36;

            return Result;
        }

        private int RowBetFactor(string Position)
        {
            int Result = 0;

            if (Position == "0, 00")
                return Result = 18;

            return Result;
        }
        
        private int SplitBetFactor(string Position)
        {
            int Result = 0;

            if (Position.Split(',').Length == 2)
            {
                int FirstNumber = -1, SecondNumber = -1;
                foreach (string pos in Position.Split(','))
                {
                    int Num = Convert.ToInt32(pos.Trim());
                    
                    if (FirstNumber == 0)
                        FirstNumber = Num;
                    else
                        SecondNumber = Num;
                }

                if (FirstNumber > 0 && FirstNumber < 37 && SecondNumber > 0 && SecondNumber < 37)
                    Result = 18;
            }

            return Result;
        }

        private int StreetBetFactor(string Position)
        {
            int Result = 0;

            if (Position == "1" || Position == "4" || Position == "7" || Position == "10" ||
                Position == "13" || Position == "16" || Position == "19" || Position == "22" ||
                Position == "25" || Position == "28" || Position == "31" || Position == "34")
                Result = 12;

            return Result;
        }

        private int CornerBetFactor(string Position)
        {
            int Result = 0;

            if (Position == "1, 2, 4, 5" || Position == "2, 3, 5, 6" || Position == "4, 5, 7, 8" || Position == "5, 6, 8, 9" || Position == "7, 8, 10, 11" || Position == "8, 9, 11, 12" ||
                Position == "10, 11, 13, 14" || Position == "11, 12, 14, 15" ||
                Position == "13, 14, 16, 17" || Position == "14, 15, 17, 18" || Position == "16, 17, 19, 20" || Position == "17, 18, 20, 21" || Position == "19, 20, 22, 23" || Position == "20, 21, 23, 24" ||
                Position == "22, 23, 25, 26" || Position == "23, 24, 26, 27" ||
                Position == "25, 26, 28, 29" || Position == "26, 27, 29, 30" || Position == "28, 29, 31, 32" || Position == "29, 30, 32, 33" || Position == "31, 32, 34, 35" || Position == "32, 33, 35, 36"
                )
                Result = 9;

            return Result;
        }

        private int TopLineOrBasketBetFactor(string Position)
        {
            int Result = 0;

            if (Position == "0, 00, 1, 2, 3")
                Result = 7;

            return Result;
        }

        private int SixLineBetFactor(string Position)
        {
            int Result = 0;

            if (Position == "1, 2, 3, 4, 5, 6" || Position == "4, 5, 6, 7, 8, 9" || Position == "7, 8, 9, 10, 11, 12" || Position == "10, 11, 12, 13, 14, 15" || Position == "13, 14, 15, 16, 17, 18" || Position == "16, 17, 18, 19, 20, 21" || Position == "19, 20, 21, 22, 23, 24" || Position == "22, 23, 24, 25, 26, 27" || Position == "25, 26, 27, 28, 29, 30" || Position == "28, 29, 30, 31, 32, 33" || Position == "31, 32, 33, 34, 35, 36")
                Result = 6;

            return Result;
        }

        private int FirstColumnBetFactor(string Position)
        {
            int Result = 0;

            if (Position == "1, 4, 7, 10, 13, 16, 19, 22, 25, 28, 31, 34")
                Result = 2;

            return Result;
        }

        private int SecondColumnBetFactor(string Position)
        {
            int Result = 0;

            if (Position == "2, 5, 8, 11, 14, 17, 20, 23, 26, 29, 32, 35")
                Result = 2;

            return Result;
        }

        private int ThirdColumnBetFactor(string Position)
        {
            int Result = 0;

            if (Position == "3, 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36")
                Result = 2;

            return Result;
        }

        private int FirstDozenBetFactor(string Position)
        {
            int Result = 0;

            foreach (string pos in Position.Split(','))
            {
                int Num;
                Num = Convert.ToInt32(pos.Trim());
                if (Num > 0 && Num < 13)
                    Result = 3;
            }

            return Result;
        }

        private int SecondDozenBetFactor(string Position)
        {
            int Result = 0;

            foreach (string pos in Position.Split(','))
            {
                int Num;
                Num = Convert.ToInt32(pos.Trim());
                if (Num > 12 && Num < 25)
                    Result = 3;
            }

            return Result;
        }

        private int ThirdDozenBetFactor(string Position)
        {
            int Result = 0;

            foreach (string pos in Position.Split(','))
            {
                int Num;
                Num = Convert.ToInt32(pos.Trim());
                if (Num > 24 && Num < 37)
                    Result = 3;
            }

            return Result;
        }

        private int OddBetFactor(string Position)
        {
            int Result = 2;

            return Result;
        }

        private int EvenBetFactor(string Position)
        {
            int Result = 2;

            return Result;
        }

        private int RedBetFactor(string Position)
        {
            int Result = 2;

            return Result;
        }

        private int BlackBetFactor(string Position)
        {
            int Result = 2;

            return Result;
        }

        private int OneToEighteenBetFactor(string Position)
        {
            int Result = 2;

            return Result;
        }

        private int NinteenToThirteeSixBetFactor(string Position)
        {
            int Result = 2;

            return Result;
        }
        #endregion
    }
}
