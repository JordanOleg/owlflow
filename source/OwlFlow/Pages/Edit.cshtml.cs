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
        public ServiceRepository serviceServerRepository{ get; set; }
        public EditModel(ServiceRepository serviceServerRepository){
            this.serviceServerRepository = serviceServerRepository;
            this.Servers = serviceServerRepository.Servers.ToList();
        }
        public IActionResult OnGet(Guid id, string name){
            EditServer = Servers.FirstOrDefault(s => s.Id == id && s.Name == name)!;
            Servers.Remove(EditServer);
            if (EditServer == null){
                return NotFound();
            }
            return Page();
        }
        public IActionResult OnPost()
        {
            Servers.Add(EditServer);
            return RedirectToPage("/InfoServers");
        }
    }
}
