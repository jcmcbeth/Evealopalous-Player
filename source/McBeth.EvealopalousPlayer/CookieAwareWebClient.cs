using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace McBeth.EvealopalousPlayer
{
    public class CookieAwareWebClient : WebClient
    {
        public CookieAwareWebClient()
            : this(new CookieContainer())
        {
        }

        public CookieAwareWebClient(CookieContainer cookies)
        {
            this.Cookies = cookies;
        }

        public int Timeout
        {
            get;
            set;
        }

        public CookieContainer Cookies
        {
            get;
            set;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);

            if (this.Timeout > 0)
            {
                request.Timeout = this.Timeout;
            }

            if (request is HttpWebRequest)
            {
                ((HttpWebRequest)request).CookieContainer = this.Cookies;
            }

            return request;
        }
    }
}
