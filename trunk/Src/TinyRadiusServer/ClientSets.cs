using System;
using System.Collections.Generic;
using System.Net;

namespace TinyRadiusServer
{
    public class ClientSets : Dictionary<string, string>
    {
        public static ClientSets Instance = new ClientSets();

        private ClientSets()
        {
            Init(Client.Default.ClientIPs, Client.Default.ShareKey);
        }

        private void Init(string ips, string shareKey)
        {
            if (String.IsNullOrEmpty(ips))
                return;
            string[] ipset = ips.Split(',');
            string[] shareKeySet = shareKey.Split(',');
            int i = 0;
            foreach (string ip in ipset)
            {
                Add(ip, shareKeySet[i]);
                i++;
            }
        }


        public void Save()
        {
            var ips = new List<string>();
            var shareKeys = new List<string>();
            foreach (var key in Keys)
            {
                ips.Add(key.ToString());
                shareKeys.Add(this[key]);
            }

            Client.Default.ShareKey = String.Join(",", shareKeys.ToArray());
            Client.Default.ClientIPs = String.Join(",", ips.ToArray());
            Client.Default.Save();
        }
    }
}