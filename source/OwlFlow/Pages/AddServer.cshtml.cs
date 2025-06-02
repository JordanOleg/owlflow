using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OwlFlow.Models;
using OwlFlow.Service;
using OwlFlow.Tools;

namespace MyApp.Namespace
{
    public class AddServerModel : PageModel
    {
        private readonly ServiceRepository _repository;

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
            _repository.Servers.Add(AddServer);
            await _repository.UpdateServers();
            return RedirectToPage("Index");
        }
        public async Task<IActionResult> OnGetTryConnection([FromBody] EditModel.RequestIPAddress requestIP)
        {
            bool result = await NetworkToolsServer.PingIp(requestIP.IP);
            return new JsonResult(new { success = result });
        }
    }
}
