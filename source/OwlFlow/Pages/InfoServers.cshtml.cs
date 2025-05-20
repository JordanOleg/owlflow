using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OwlFlow.Models;
using OwlFlow.Service;

namespace MyApp.Namespace
{
    public class InfoServersModel : PageModel
    {
        public List<Server> Servers{ get; set; }
        public ServiceRepository ServerRepository{ get; set; }
        public InfoServersModel(ServiceRepository serviceServerRepository){
            this.ServerRepository = serviceServerRepository;
            this.Servers = this.ServerRepository.GetServers();
        }
        public void OnGet()
        {
            this.Servers = ServerRepository.GetServers(); 
        }
    }
}
