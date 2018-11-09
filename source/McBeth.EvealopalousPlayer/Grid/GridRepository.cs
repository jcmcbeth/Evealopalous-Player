using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;

namespace McBeth.EvealopalousPlayer.Grid
{
    public class GridRepository
    {
        public static readonly Regex GridRegex = new Regex(
            "<a\\s+ (?:style=\"background-color:(?<color>\\#?[a-fA-F0-9]+" +
            ")\"\\s+)?href=\\\"javascript:void\\(0\\)\"\\s+onclick=\"load" +
            "XMLDoc\\((?<ticket>\\d+),\\s+(?<grid_id>\\d+),\\s+(?<grid_ba" +
            "se_id>\\d+),\\s?(?<grid_square>\\d+)\\)\"></a>",
            RegexOptions.IgnoreCase
            | RegexOptions.Multiline
            | RegexOptions.CultureInvariant
            | RegexOptions.IgnorePatternWhitespace
            | RegexOptions.Compiled
        );   

        public GridRepository(EvealopalousClient client)
        {
            this.Client = client;
        }

        public EvealopalousClient Client
        {
            get;
            private set;
        }

        public GridBoard PurchaseGridTicket(GridBoard grid, int ticketId)
        {
            string response = this.Client.PlayGridTicket(ticketId, grid.GameId, grid.Type);

            GridBoard newGrid = GetGridFromResponse(response, grid.Type);

            return newGrid;
        }

        public List<GridBoard> GetActiveGrids()
        {
            List<GridBoard> grids = new List<GridBoard>();

            grids.Add(GetGridByType(GridType.TheSlums));
            grids.Add(GetGridByType(GridType.SpaceWalk));
            grids.Add(GetGridByType(GridType.Polarity));
            grids.Add(GetGridByType(GridType.VectorX));

            return grids;
        }

        public GridBoard GetGridByType(GridType type)
        {
            string response = this.Client.DownloadGrid(type);

            return GetGridFromResponse(response, type);
        }

        private GridBoard GetGridFromResponse(string response, GridType type)
        {            
            GridBoard grid = null;

            MatchCollection matches = GridRegex.Matches(response);
            foreach (Match match in matches)
            {
                // Only need to get this once
                if (grid == null)
                {
                    int gameId = int.Parse(match.Groups["grid_id"].Value);
                    grid = new GridBoard(gameId, type);
                }

                int id = int.Parse(match.Groups["ticket"].Value); ;
                Cell cell = new Cell(id);

                string color = match.Groups["color"].Value;

                if (type == GridType.TheSlums)
                {
                    if (color == "#F7BB09")
                    {
                        cell.State = CellState.Award;
                    }
                    else if (color == "#730000")
                    {
                        cell.State = CellState.Empty;
                    }
                    else if (color == "#644B76")
                    {
                        cell.State = CellState.Item;
                    }
                    else
                    {
                        cell.State = CellState.Hidden;
                    }
                }

                grid.AddCell(cell);
            }

            return grid;
        }
    }
}
