using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OwlFlow.Models;
using OwlFlow.Service;
using OwlFlow.Tools;

namespace MyApp.Namespace
{
    [IgnoreAntiforgeryToken]
    public class EditModel : PageModel
    {
        public class RequestIPAddress
        {
            public string IP { get; set; }
        }

        [BindProperty]
        public Server EditServer { get; set; }
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
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Server record = _serviceServerRepository.Servers.First(s => s.Id == EditServer.Id);
            _serviceServerRepository.Servers.Remove(record);
            _serviceServerRepository.Servers.Add(EditServer);
            await _serviceServerRepository.UpdateServers();
            return RedirectToPage("/InfoServers");
        }
        public async Task<IActionResult> OnGetTryConnection([FromBody] RequestIPAddress requestIP)
        {
            bool result = await NetworkToolsServer.PingIp(requestIP.IP);
            return new JsonResult(new { success = result });
        }
    }
}
