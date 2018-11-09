using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;

namespace McBeth.EvealopalousPlayer.Grid
{
    public class EvealopalousClient
    {
        private CookieContainer cookies;        

        public const string GridUrl = "http://evealopalous.com/phpfile.php?ticket={0}&grid_id={1}&grid_base_id={2}&grid_square={3}";
        public const int DefaultTimeout = 100000;

        public EvealopalousClient(string username, string password)
        {
            this.LoggedIn = false;

            this.UserName = username;
            this.Password = password;

            this.Timeout = EvealopalousClient.DefaultTimeout;
        }

        public int Timeout
        {
            get;
            set;
        }

        public bool LoggedIn
        {
            get;
            private set;
        }

        public string UserName
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public string PlayGridTicket(int ticket, int gridId, GridType type)
        {
            return DownloadPage(
                EvealopalousClient.GridUrl,
                ticket, gridId, (int)type, (int)type);
        }

        public string DownloadGrid(GridType type)
        {
            return DownloadPage(
                EvealopalousClient.GridUrl,
                650, 1, (int)type, (int)type);
        }

        public string DownloadGrid(int ticket, int gridId, int gridBase, int gridSquare)
        {
            return DownloadPage(
                EvealopalousClient.GridUrl,
                ticket, gridId, gridBase, gridSquare);
        }

        public string DownloadPage(string formattedUrl, params object[] args)
        {
            string url = string.Format(formattedUrl, args);

            using (CookieAwareWebClient client = this.GetLoggedInWebClient())
            {
                string response = client.DownloadString(url);

                return response;
            }
        }

        protected virtual CookieAwareWebClient GetLoggedInWebClient()
        {
            if (!this.LoggedIn)
            {
                this.cookies = GetLoginCookie(this.UserName, this.Password);
                this.LoggedIn = true;
            }                        

            var client = new CookieAwareWebClient(cookies);
            client.Timeout = this.Timeout;
            client.Headers.Add("Referer", "http://evealopalous.com/grid.php");

            string cookiesText = client.Cookies.GetCookieHeader(new Uri("http://evealopalous.com"));

            return client;
        }

        private MatchCollection GetPageMatches(string url, Regex regex)
        {
            string response = this.DownloadPage(url);

            return regex.Matches(response);            
        }

        private static CookieContainer GetLoginCookie(string userName, string password)
        {
            CookieContainer cookies = new CookieContainer();

            using (CookieAwareWebClient client = new CookieAwareWebClient(cookies))
            {
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                string postText = string.Format("loginname={0}&loginpass={1}&Login=Login&width=1920&height=1080&colorDepth=24&pixelDepth=24&platform=Win32&avail_plugins=27", userName, password);
                byte[] postData = Encoding.ASCII.GetBytes(postText);

                byte[] response = client.UploadData("http://evealopalous.com/login.php", "POST", postData);
            }

            cookies.Add(
                new Uri("http://evealopalous.com"),
                new Cookie("javascript", "1"));

            return cookies;
        }
    }
}
