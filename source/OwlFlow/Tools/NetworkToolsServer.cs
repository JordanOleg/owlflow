using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace OwlFlow.Tools
{
    public static class NetworkToolsServer
    {
        public static async Task<bool> PingIp(string ip)
        {
            PingReply result = await new Ping().SendPingAsync(ip);
            if (result.Status == IPStatus.Success)
                return true;
            else return false;
        }
    }
}