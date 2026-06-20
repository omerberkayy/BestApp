using Microsoft.AspNetCore.Mvc;
using BestApp.Core.Services;
using BestApp.Web.Models;

namespace BestApp.Web.Controllers;

public class HomeController : Controller
{
    // Service katmanındaki arayüzlerimizi (Interface) tanımlıyoruz
    private readonly IProjectService _projectService;
    private readonly IService<Core.Entities.Service> _serviceManager; // Kendi Service entity'miz ile karışmaması için tam yol verdik

    // Dependency Injection (DI) ile servisleri Controller'a enjekte ediyoruz
    public HomeController(IProjectService projectService, IService<Core.Entities.Service> serviceManager)
    {
        _projectService = projectService;
        _serviceManager = serviceManager;
    }

    public async Task<IActionResult> Index()
    {
        // 1. Sadece aktif (yayında olan) peyzaj uygulamalarını Service katmanından çekiyoruz
        var activeProjects = await _projectService.GetActiveProjectsAsync();

        // 2. Sistemdeki aktif hizmetleri çekiyoruz (Burada generic getwhere metodunu kullanıyoruz)
        var activeServices = await _serviceManager.GetWhereAsync(s => s.IsActive == true);

        // 3. Verileri ViewModel'e dolduruyoruz
        var viewModel = new HomeViewModel
        {
            Projects = activeProjects,
            Services = activeServices
        };

        // 4. Dolu paketi (viewModel) View'a gönderiyoruz
        return View(viewModel);
    }

    // Hakkımızda ve İletişim gibi diğer statik/yarı-dinamik sayfalar için metotlar:
    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }
}