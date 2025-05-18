using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwlFlow.Model;

namespace OwlFlow.Service
{
    public class ServiceRepository
    {
        public List<Server> Servers { get; private set; }
        public ServiceJsonSerializerServers Repository { get; set; }
        public ServiceRepository(ServiceJsonSerializerServers serviceServerRepository){
            Repository = serviceServerRepository;
            Servers = this.GetServers();
        }

        public async Task AddServer(Server server){
            Servers.Add(server);
            await UpdateServers();
        }

        public async Task RemoveServer(Server server) {
            Servers.Remove(server);
            await UpdateServers();
        }
        public async Task UpdateServers(){
            await new Task(() => {  
                Repository.SaveServersAll(this.Servers);
            });
        }
        public List<Server> GetServers(){
            bool result = Repository.GetServers(out List<Server> server);
            return result ? server : new List<Server>();
        }
    }
}