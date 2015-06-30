using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;
using System.Threading.Tasks;

namespace PrayerTimes.Utils
{
    class Network
    {
        public async static Task<bool> IsInternetConnected()
        {
            WebRequest webReq = null;
            WebResponse resp = null;
            Uri url = null;
            url = new Uri("http://www.google.com");
            webReq = (HttpWebRequest)HttpWebRequest.Create(url);
            try
            {
                resp = (HttpWebResponse)await webReq.GetResponseAsync();
                webReq.Abort();
                webReq = null;
                resp = null;
                return true;
            }
            catch
            {
                webReq.Abort();
                webReq = null;
            }
            finally
            {
                if (resp != null)
                {
                }
                url = null;
                resp = null;
            }
            return false;
        }

    }
}
