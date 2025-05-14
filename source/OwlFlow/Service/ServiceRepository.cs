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
        public ServiceServerRepository Repository { get; set; }
        public ServiceRepository(ServiceServerRepository serviceServerRepository){
            Repository = serviceServerRepository;
            Servers = this.GetServers();
        }

        public void AddServer(Server server){
            Servers.Add(server);
            Task.Run(() => this.UpdateServers());
        }

        public void RemoveServer(Server server) {
            Servers.Add(server);
            Task.Run(() => this.UpdateServers());
        }
        public async Task UpdateServers(){
            await new Task(() => {  
                Repository.SaveServersAll(this.Servers);
            });
        }
        private List<Server> GetServers(){
            bool result = Repository.GetServers(out List<Server> server);
            return result ? server : new List<Server>();
        }
    }
}