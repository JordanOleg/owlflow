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
            
            return servers
                        .Where(x => x.UseCPU <= 85)
                        .Where(x => x.UseMemory <= 85)
                        .Where(x => x.MaxCapacityPeoples >= x.CountClient)
                        .First();
        }
    }
}