using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace TinyRadiusServer
{
    public class ClientSets : Dictionary<IPAddress, string>
    {
        public static ClientSets Instance = new ClientSets();

        public void Init(string ips, string shareKey)
        {
            if (String.IsNullOrEmpty(ips))
                return;
            var ipset = ips.Split(',');
            var shareKeySet = shareKey.Split(',');
            int i = 0;
            foreach (string ip in ipset)
            {
                this.Add(IPAddress.Parse(ip), shareKeySet[i]);
                i++;
            }
        }



    }
}
