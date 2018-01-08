using System;
using System.Collections.Generic;

namespace Participants
{
    public class Player : Participant
    {
        public Player(string name, int cash) : base(name, cash)
        {
        }

        public static string ShowPlayerCoins(IEnumerable<Participant> players)
        {
            string result = "\n***** Summary of players coin balance *****";

            foreach (Participant player in players)
            {
                result += "\nPlayer: " + player.Name + "\t";

                result += player.DisplayCoins();
            }
            return result;
        }
    }
/*
    public class PlayerList
    {
        public static List<Player> players = new List<Player>();
        public static int Total = 0;

        public PlayerList()
        {

        }

        internal static Player AddPlayer(string Name)
        {
            Player player = new Player(Name);
            players.Add(player);
            return player;
        }
    }
    */
}