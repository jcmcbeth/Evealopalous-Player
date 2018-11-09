using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McBeth.EvealopalousPlayer.Models
{
    public class Event
    {
        /// <summary>
        /// Gets the ID of the raffle.
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets an estimated time the raffle started.
        /// </summary>
        public DateTime Start
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets an estimated time the raffle ended.
        /// </summary>
        public DateTime End
        {
            get;
            set;
        }
    }
}
