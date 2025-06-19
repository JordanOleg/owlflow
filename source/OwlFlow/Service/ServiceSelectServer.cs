using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwlFlow.Models;

namespace OwlFlow.Service
{
    public class ServiceSelectServer
    {
        private ServiceRepository serviceRepository; 
        public ServiceSelectServer(ServiceRepository serviceRepository)
        {
            this.serviceRepository = serviceRepository;
        }
        public Server GetOptimalServer()
        {
            List<Server> servers = serviceRepository.Servers;
            if (servers != null && servers.Count > 0)
            {
                List<Server> connect = servers.Where(x => x.IsConnected == true).ToList();
                if (connect.Count > 0)
                {
                    if (connect.Count == 1)
                    {
                        return connect[0];
                    }
                    return connect[Random.Shared.Next(0, connect.Count)];
                }
                else return null;
            }
            else return null;
            /* return servers
                        .Where(x => x.UseCPU <= 85)
                        .Where(x => x.UseMemory <= 85)
                        .Where(x => x.MaxCapacityClient >= x.CountClient)
                        .First(); */

        }
    }
}