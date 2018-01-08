using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Participants;
using Utilities;
using Monitory;

namespace RDeepCore
{
    public class RDeepPlayer : Player
    {
        IRDeepStrategy RDeepStrategy;

        public RDeepPlayer(string name, int Cash, string strategy) : base(name, Cash)
        {
            if (strategy == "Random")
                RDeepStrategy = new RandomRDeepStrategy();
            else if (strategy == "TenFifteen")
                RDeepStrategy = new BetByTenFifteenStrategy();
        }

        public IEnumerable<RDeepBet> GoForBet(List<int> LastNumbers)
        {
            return RDeepStrategy.GoForBet(this, LastNumbers);
        }
    }
}
