using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwlFlow.Models;

namespace OwlFlow.Service
{
    public class ServiceRepository
    {
        public List<Server> Servers { get; set; }
        public ServiceJsonSerializerServers Repository { get; set; }
        public ServiceRepository(ServiceJsonSerializerServers serviceServerRepository){
            Repository = serviceServerRepository;
            Servers = this.GetServers();
        }
        public async Task UpdateServers(){
            await Task.Run(() => {  
                Repository.Update(this.Servers);
            });
        }
        public List<Server> GetServers(){
            bool result = Repository.GetServers(out List<Server> server);
            return result ? server : new List<Server>();
        }
    }
}