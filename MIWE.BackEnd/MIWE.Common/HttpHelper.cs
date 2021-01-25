using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace MIWE.Common
{
    public class HttpHelper
    {
        public static string GetCurrentExternalIP()
        {
            try
            {
                string externalIP;
                externalIP = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
                externalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"))
                             .Matches(externalIP)[0].ToString();
                
                if (externalIP.Contains("85.204.6.250")) 
                    return "https://localhost:8008";

                return externalIP;
            }
            catch
            {
                return "https://localhost:8008";
            }
        }
    }
}
