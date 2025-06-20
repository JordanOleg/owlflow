using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace OwlFlow.Tools
{
    public static class NetworkToolsServer
    {
        public static async Task<bool> PingIp(string ipUri)
        {
            Uri.TryCreate($"http://{ipUri}/", UriKind.RelativeOrAbsolute, out var result);
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(result);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else return false;
        }
    }
}