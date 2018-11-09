using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace McBeth.EvealopalousPlayer.Grid
{
    public enum GridType
    {
        TheSlums = 1,
        Polarity = 2,
        VectorX = 3,
        SpaceWalk = 4
    }

    public class GridBoard
    {
        private List<Cell> cells;        

        public GridBoard(int gameId, GridType type)
        {
            this.cells = new List<Cell>();
            this.GameId = gameId;
            this.Type = type;
        }

        public ReadOnlyCollection<Cell> Cells
        {
            get
            {
                return new ReadOnlyCollection<Cell>(cells);
            }
        }

        public int GameId
        {
            get;
            private set;
        }

        public GridType Type
        {
            get;
            private set;
        }

        public void AddCell(Cell cell)
        {
            this.cells.Add(cell);
        }

        public int GetCellTypeCount(CellState state)
        {
            return this.cells.Count(c => c.State == state);
        }

        public Cell GetFirstHiddenCell()
        {
            return this.cells.First(c => c.State == CellState.Hidden);
        }

        public Cell GetCellByTicketId(int ticketId)
        {
            return this.cells.Single(c => c.TicketId == ticketId);
        }
    }
}
