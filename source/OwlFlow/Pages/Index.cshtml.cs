using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OwlFlow.Models;
using OwlFlow.Service;

namespace OwlFlow.Pages;

public class IndexModel : PageModel
{
    public List<Server> Servers { get; set; }
    public ServiceRepository Repository { get; set; }

    public IndexModel(ServiceRepository serviceRepository)
    {
        Repository = serviceRepository;
        Servers = serviceRepository.Servers;
    }

    public void OnGet()
    {
        
    }
    public List<Server> Update()
    {
        return Servers = Repository.GetServers();
    }
}
