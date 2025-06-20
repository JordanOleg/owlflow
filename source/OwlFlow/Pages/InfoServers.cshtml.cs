using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OwlFlow.Models;
using OwlFlow.Service;

namespace MyApp.Namespace
{
    [IgnoreAntiforgeryToken]
    public class InfoServersModel : PageModel
    {
        public List<Server> Servers { get; set; }
        private ServiceRepository _serverRepository { get; set; }
        public InfoServersModel(ServiceRepository serviceServerRepository)
        {
            this._serverRepository = serviceServerRepository;
            this.Servers = this._serverRepository.Servers;
        }
        public void OnGet()
        {
            Servers = _serverRepository.GetServers();
        }
        public async Task<IActionResult> OnPostDelete(Guid id, string name)
        {
            Server removeServer = _serverRepository.Servers.First(s => id == s.Id && name == s.Name);
            Servers.Remove(removeServer);
            await _serverRepository.UpdateServers();
            return Page();
        }
    }
}
