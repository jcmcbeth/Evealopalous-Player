using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using McBeth.EvealopalousPlayer.Models;
using System.Text.RegularExpressions;
using System.Threading;
using McBeth.EvealopalousPlayer.Grid;

namespace McBeth.EvealopalousPlayer
{
    public class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static CookieContainer cookies;

        private static int rolls = 0;
        private static int wins = 0;
        private static int losses = 0;

        private static string username;
        private static string password;

        private static List<int> activeRaffles;
        private static List<int> playerRaffles;

        public static void Main(string[] args)
        {
            username = args[0].Replace(" ", "+");
            password = args[1];

            PlayGrids();
        }

        public static void PlayGrids()
        {
            EvealopalousClient client = new EvealopalousClient(username, password);
            GridRepository repository = new GridRepository(client);

            client.Timeout = 4000; // 4 seconds bro

            while (true)
            {
                bool success = false;
                try
                {
                    success = PlayGrid(GridType.TheSlums, repository);
                } catch (TimeoutException ex)
                {
                    logger.ErrorException(ex.Message, ex);
                } catch (WebException ex)
                {
                    logger.ErrorException(ex.Message, ex);
                }

                Thread.Sleep(2000);
            }
        }


        public static int lastUnknown = -1;

        public static bool PlayGrid(GridType type, GridRepository repository)
        {
            //logger.Trace("Downloading grid {0}", type);
            GridBoard grid = repository.GetGridByType(type);            

            int awards = grid.GetCellTypeCount(CellState.Award);
            int unknown = grid.GetCellTypeCount(CellState.Hidden);
            int empty = grid.GetCellTypeCount(CellState.Empty);
            int items = grid.GetCellTypeCount(CellState.Item);

            int awardsLeft = 22 - awards;

            double odds = awardsLeft / (double)unknown;
            double itemOdds = (8 - items) / (double)unknown;

            if (lastUnknown != unknown)
            {
                logger.Info("{0} shows {1} awards, {2} empty, {3} unknown, {4} items, {5:0.00000%} item odds, {6:0.00000%} odds", type, awards, empty, unknown, items, itemOdds, odds);
                lastUnknown = unknown;
            }

            //if (odds >= .315)
            //{
            //    Cell cell = grid.GetFirstHiddenCell();

            //    logger.Trace("Buying ticket {0}", cell.TicketId);
            //    grid = repository.PurchaseGridTicket(grid, cell.TicketId);

            //    Cell newCell = grid.GetCellByTicketId(cell.TicketId);
            //    if (newCell.State == CellState.Award)
            //    {
            //        wins++;
            //        logger.Info("Ticket {0} won!", cell.TicketId);
            //    } else if (newCell.State == CellState.Empty)
            //    {
            //        losses++;
            //        logger.Info("Ticket {0} was empty.", cell.TicketId);
            //    } else
            //    {
            //        logger.Warn("Cell state for ticket {0} did not change on purchase", cell.TicketId);
            //    }
            //    rolls++;

            //    double winOdds = wins / (double)(wins + losses);
            //    logger.Info("{1} wins, {2} losses, {3} rolls, winning percentage {4:0.00%}", type, wins, losses, rolls, winOdds);

            //    return true;
            //} else
            //{
            //    logger.Info("Skipping because of bad odds");
            //}

            return false;
        }

        public static void PlayAllRaffles()
        {
            logger.Info("Logging in");
            cookies = GetLoginCookie(username, password);
            WaitRandom(5, 10);

            logger.Info("Getting active player games");
            playerRaffles = GetActivePlayerGameIds();
            logger.Info("{0} active player games found", playerRaffles.Count);
            WaitRandom(5, 10);

            while (true)
            {
                bool failure = false;

                logger.Info("Getting active ISK games");
                activeRaffles = GetAllRaffles();         

                // Get the raffles that are not a player raffle
                List<int> newRaffles = activeRaffles.Except(playerRaffles).ToList();
                logger.Info("{0} out of {1} active games are new", newRaffles.Count, activeRaffles.Count);                

                foreach (int raffleId in newRaffles)
                {
                    WaitRandom(3, 6);

                    logger.Info("Buying ticket for game {0}", raffleId);
                    if (BuyTickets(raffleId, null, 1))
                    {
                        logger.Info("Ticket purchased");
                        playerRaffles.Add(raffleId);
                    } else
                    {
                        logger.Error("Ticket purchase failed");
                        failure = true;                        
                    }                    
                }

                if (!failure)
                {
                    WaitRandom(30, 60);
                }
            }
        }

        public static List<int> GetActivePlayerGameIds()
        {
            Regex raffleEntriesRegex = new Regex("<a\\s+href=\\\"/raffles\\.php\\?raffletype=(?<raffleType>[A-Z]+)&raffleid=(?<id>\\d+)\\\">Add entries</a>");
            string url = "http://evealopalous.com/account.php?a=activeentries";
            List<int> ids = new List<int>();

            using (CookieAwareWebClient client = new CookieAwareWebClient(cookies))
            {
                string response = client.DownloadString(url);

                foreach (Match match in raffleEntriesRegex.Matches(response))
                {
                    string id = match.Groups["id"].Value;
                    int idValue = int.Parse(id);

                    ids.Add(idValue);
                }
            }

            return ids;
        }

        public static bool BuyTickets(int raffleId, string type, int count)
        {
            string urlFormat = "http://evealopalous.com/raffles.php?raffletype={0}&raffleid={1}";
            string url = string.Format(urlFormat, type, raffleId);

            if (string.IsNullOrWhiteSpace(type))
            {
                type = "all";
            }

            using (CookieAwareWebClient client = new CookieAwareWebClient(cookies))
            {
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                string postText = string.Format("raffle_add_tickets_field={0}&buy_raffles=Buy+Raffles", count);
                byte[] postData = Encoding.ASCII.GetBytes(postText);

                byte[] response = client.UploadData(url, "POST", postData);

                string text = Encoding.ASCII.GetString(response);

                if (text.Contains("Ticket(s) purchased sucessfully!"))
                {
                    return true;
                }                
            }

            return false;
        }

        public static List<int> GetActiveIskGameIds()
        {
            Regex gameLink = new Regex("<a\\s+href=\\\"/raffles\\.php\\?raffletype=ISK&raffleid=(?<id>\\d+)\\\">");
            string url = "http://evealopalous.com/raffles.php?raffletype=ISK";
            List<int> raffles = new List<int>();

            using (CookieAwareWebClient client = new CookieAwareWebClient(cookies))
            {
                string response = client.DownloadString(url);

                foreach (Match match in gameLink.Matches(response))
                {
                    string id = match.Groups["id"].Value;

                    int idValue = int.Parse(id);

                    raffles.Add(idValue);
                }
            }

            return raffles;
        }

        public static List<int> GetAllRaffles()
        {
            string url = "http://evealopalous.com/raffles.php?raffletype=all";
            int page = 1;
            int pages = 1;
            List<int> raffles = new List<int>();

            raffles = GetAllRafflesWithUrl(url, out pages);
            page++;

            while (page <= pages)
            {
                url = string.Format("http://evealopalous.com/raffles.php?raffletype=all&page={0}", page);
                
                int whatever;
                List<int> otherPageRaffles = GetAllRafflesWithUrl(url, out whatever);

                raffles = raffles.Union(otherPageRaffles).ToList();

                page++;
            }

            return raffles;
        }

        public static List<int> GetAllRafflesWithUrl(string url, out int pages)
        {
            Regex gameLink = new Regex("<a\\s+href=\\\"/raffles\\.php\\?raffletype=(?<raffleType>[A-Za-z]+)&raffleid=(?<id>\\d+)\\\">");            
            Regex pageCountRegex = new Regex("<div class=\\\"paginationHeader\\\">\\s+Pages<br />\\s+\\((?<pages>\\d+)\\)\\s+</div>");
            List<int> raffles = new List<int>();

            using (CookieAwareWebClient client = new CookieAwareWebClient(cookies))
            {
                string response = client.DownloadString(url);

                pages = int.Parse(pageCountRegex.Match(response).Groups["pages"].Value);

                foreach (Match match in gameLink.Matches(response))
                {
                    string id = match.Groups["id"].Value;

                    int idValue = int.Parse(id);

                    raffles.Add(idValue);
                }
            }

            return raffles;
        }

        public static CookieContainer GetLoginCookie(string userName, string password)
        {
            CookieContainer cookies = new CookieContainer();

            using (CookieAwareWebClient client = new CookieAwareWebClient(cookies))
            {
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                string postText = string.Format("loginname={0}&loginpass={1}&Login=Login&width=1920&height=1080&colorDepth=24&pixelDepth=24&platform=Win32&avail_plugins=27", userName, password);
                byte[] postData = Encoding.ASCII.GetBytes(postText);

                byte[] response = client.UploadData("http://evealopalous.com/login.php", "POST", postData);

                string text = Encoding.ASCII.GetString(response);
            }

            return cookies;
        }

        private static void WaitRandom(int minSeconds, int maxSeconds)
        {
            Random random = new Random();

            int wait = random.Next(minSeconds * 1000, maxSeconds * 1000);

            logger.Trace("Waiting {0:0.0} seconds", wait / 1000.0);
            Thread.Sleep(wait);
        }
    }
}
