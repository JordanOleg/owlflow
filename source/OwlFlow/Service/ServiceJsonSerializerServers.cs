using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using OwlFlow.Model;

namespace OwlFlow.Service
{
    public class ServiceJsonSerializerServers
    {
        public FileInfo PathRepository { get; set; }
        public bool GetServers(out List<Server> servers){
            try
            {
                if (!File.Exists(PathRepository.FullName))
                {
                    servers = null;
                    return false;
                }
                string text = File.ReadAllText(PathRepository.FullName);
                List<Server> result = JsonSerializer.Deserialize<List<Server>>(text);
                if (result != null){
                    servers = result;
                    return true;
                }
                servers = null;
                return false;
            }
            catch{
                servers = null;
                return false;
            }
        }
        /// <summary>
        /// Update data 
        /// </summary>
        /// <param name="servers"></param>
        /// <returns></returns>
        public bool Update(List<Server> servers)
        {
            try
            {
                var json = JsonSerializer.Serialize(servers, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(PathRepository.FullName, json);
                return true;
            }
            catch { return false; }
        }
    }
}