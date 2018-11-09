using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McBeth.EvealopalousPlayer.Models
{
    public class Raffle : Event
    {
        /// <summary>
        /// Gets and sets the name of the prize.
        /// </summary>
        public string Prize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets the ISK or buyout value of the prize.
        /// </summary>
        public double Value
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets the cost of all of the tickets.
        /// </summary>
        public double TotalCost
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets the cost for each ticket.
        /// </summary>
        public double TicketPrice
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets the total number of tickets.
        /// </summary>
        public int TotalTickets
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets the number of unclaimed tickets remaining.
        /// </summary>
        public int TicketsRemaining
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            Raffle raffle = obj as Raffle;
            if (raffle != null)
            {
                return this.Id == raffle.Id;
            }

            return false;
        }
    }
}
