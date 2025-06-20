using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OwlFlow.Models;
using OwlFlow.Service;
using OwlFlow.Tools;

namespace MyApp.Namespace
{
    [IgnoreAntiforgeryToken]
    public class AddServerModel : PageModel
    {
        public class RequestIPAddress
        {
            public string IP { get; set; }
        }
        private readonly ServiceRepository _repository;

        [BindProperty]
        public bool IsChecked { get; set; }

        [BindProperty]
        public Server AddServer { get; set; }
        public AddServerModel(ServiceRepository repository)
        {
            _repository = repository;
        }
        public IActionResult OnGet()
        {
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            AddServer.Id = Guid.NewGuid();
            ModelState.Remove("AddServer.URI");
            if (!ModelState.IsValid)
            {
                return Page();
            }
            AddServer.IsConnected = IsChecked;
            _repository.Servers.Add(AddServer);
            await _repository.UpdateServers();
            return RedirectToPage("Index");
        }

        [HttpPost]
        public async Task<IActionResult> OnPostTryConnection([FromBody] RequestIPAddress requestIPAddress)
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
