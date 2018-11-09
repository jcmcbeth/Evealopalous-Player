using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McBeth.EvealopalousPlayer.Models
{
    public class Payout : Event
    {
        public double TotalPayout
        {
            get;
            set;
        }

        public int Tokens
        {
            get;
            set;
        }

        public double PayoutPerToken
        {
            get;
            set;
        }

        public TimeSpan TimeRemaining
        {
            get;
            set;
        }
    }
}
