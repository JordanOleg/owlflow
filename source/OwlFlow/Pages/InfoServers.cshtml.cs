using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OwlFlow.Model;
using OwlFlow.Service;

namespace MyApp.Namespace
{
    public class InfoServersModel : PageModel
    {
        public List<Server> Servers{ get; set; }
        public ServiceServerRepository ServerRepository{ get; set; }
        public InfoServersModel(ServiceServerRepository serviceServerRepository){
            this.ServerRepository = serviceServerRepository;
            this.ServerRepository.GetServers(out List<Server> servers);
            this.Servers = servers;
        }
        public void OnGet()
        {
            ServerRepository.GetServers(out List<Server> servers);
            this.Servers = servers;
        }
    }
}
