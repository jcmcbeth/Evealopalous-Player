using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McBeth.EvealopalousPlayer.Grid
{
    public enum CellState
    {
        Hidden,
        Empty,
        Award,
        Item
    }

    public class Cell
    {
        public Cell(int ticketId)
        {
            this.TicketId = ticketId;
        }

        public int TicketId
        {
            get;
            private set;
        }

        public CellState State
        {
            get;
            set;
        }
    }
}
