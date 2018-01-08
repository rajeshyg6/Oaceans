namespace Participants
{
    public class Dealer : Participant
    {
        public Dealer(string name, int Cash) : base(name, Cash)
        {
        }

        public string ShowDealerCoins()
        {
            string result = "\n***** Dealer coin balance *****\n";
            
            result += DisplayCoins();

            return result;
        }
    }
}