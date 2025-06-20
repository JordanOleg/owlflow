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
        [HttpPost]
        public async Task<IActionResult> OnPostTryConnection([FromBody] AddServerModel.RequestIPAddress requestIPAddress)
        {
            try
            {
                bool result = await Task.Run(async () =>
                {
                    Uri.TryCreate($"http://{requestIPAddress.IP}/", UriKind.RelativeOrAbsolute, out var result);
                    HttpClient httpClient = new HttpClient();
                    HttpResponseMessage response = await httpClient.GetAsync(result);
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else return false;
                });
                return new JsonResult(new { success = result })
                {
                    ContentType = "application/json"
                };
            }
            catch
            {
                return new JsonResult(new { success = false })
                {
                    ContentType = "application/json"
                };
            }
        }
    }
}
