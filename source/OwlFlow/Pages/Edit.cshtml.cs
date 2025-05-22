using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OwlFlow.Models;
using OwlFlow.Service;

namespace MyApp.Namespace
{
    public class EditModel : PageModel
    {
        [BindProperty]
        public Server EditServer{ get; set; }
        private ServiceRepository _serviceServerRepository;
        public EditModel(ServiceRepository serviceServerRepository)
        {
            this._serviceServerRepository = serviceServerRepository;
        }
        public IActionResult OnGet(Guid id, string name)
        {
            EditServer = _serviceServerRepository.Servers.First(s => s.Id == id && s.Name == name)!;
            if (EditServer == null)
            {
                return NotFound();
            }
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            Server record = _serviceServerRepository.Servers.First(s => s.Id == EditServer.Id);
            _serviceServerRepository.Servers.Remove(record);
            _serviceServerRepository.Servers.Add(EditServer);
            await _serviceServerRepository.UpdateServers();
            return RedirectToPage("/InfoServers");
        }
    }
}
