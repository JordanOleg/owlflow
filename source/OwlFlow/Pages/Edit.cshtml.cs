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
        private Server _editObjectOfRef;
        private ServiceRepository _serviceServerRepository;
        public EditModel(ServiceRepository serviceServerRepository)
        {
            this._serviceServerRepository = serviceServerRepository;
        }
        public IActionResult OnGet(Guid id, string name)
        {
            EditServer = _serviceServerRepository.Servers.First(s => s.Id == id && s.Name == name)!;
            _editObjectOfRef = EditServer;
            if (_editObjectOfRef == null)
            {
                return NotFound();
            }
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            _serviceServerRepository.Servers.Remove(_editObjectOfRef);
            _serviceServerRepository.Servers.Add(EditServer);
            await _serviceServerRepository.UpdateServers();
            return RedirectToPage("/InfoServers");
        }
    }
}
