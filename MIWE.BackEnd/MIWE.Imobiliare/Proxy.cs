using System;
using System.Collections.Generic;
using System.Text;

namespace MIWE.Imobiliare
{
    public class Proxy
    {

        public int FailCount { get; set; }

        public string Uri
        {
            get
            {
                return $"http://{IP}:{Port}";
            }
        }

        public string IP { get; set; }

        public string Port { get; set; }
    }
}
