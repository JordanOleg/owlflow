using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OwlFlow.Model;
using OwlFlow.Service;

namespace MyApp.Namespace
{
    public class EditModel : PageModel
    {
        [BindProperty]
        public Server EditServer{ get; set; }
        public List<Server> Servers{ get; set; }
        public ServiceServerRepository serviceServerRepository{ get; set; }
        public EditModel(ServiceServerRepository serviceServerRepository){
            this.serviceServerRepository = serviceServerRepository;
            this.serviceServerRepository.GetServers(out List<Server> servers);
            this.Servers = servers;
        }
        public IActionResult OnGet(Guid guid, string name){
            EditServer = Servers.FirstOrDefault(s => s.Id == guid && s.Name == name)!;
            Servers.Remove(EditServer);
            if (EditServer == null){
                return NotFound();
            }
            return Page();
        }
        public void OnPost()
        {
            Servers.Add(EditServer);
            serviceServerRepository.SaveServersAll(Servers);
        }
    }
}
