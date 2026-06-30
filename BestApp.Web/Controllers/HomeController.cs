using Microsoft.AspNetCore.Mvc;
using BestApp.Core.Entities;
using BestApp.Core.Services;
using BestApp.Web.Models;

namespace BestApp.Web.Controllers;

public class HomeController : Controller
{
    private readonly IService<Project> _projectService;
    private readonly IService<About> _aboutService;
    private readonly IService<Core.Entities.Service> _serviceService;
    private readonly IService<ContactMessage> _contactMessageService;

    // Tüm servisleri Constructor (Yapıcı Metot) üzerinden içeri alıyoruz
    public HomeController(
        IService<Project> projectService,
        IService<About> aboutService,
        IService<Core.Entities.Service> serviceService,
        IService<ContactMessage> contactMessageService)
    {
        _projectService = projectService;
        _aboutService = aboutService;
        _serviceService = serviceService;
        _contactMessageService = contactMessageService;
    }

    public async Task<IActionResult> Index()
    {
        var abouts = await _aboutService.GetAllAsync();
        var services = await _serviceService.GetWhereAsync(x => x.IsActive);
        var projects = await _projectService.GetWhereAsync(x => x.IsActive);

        var viewModel = new HomeViewModel
        {
            AboutInfo = abouts.FirstOrDefault(), // İlk kaydı Hakkımızda olarak al
            Services = services.ToList(),
            Projects = projects.OrderByDescending(x => x.Id).ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendMessage(HomeViewModel model)
    {
        var message = model.ContactForm;
        
        if (ModelState.IsValid)
        {
            message.SentDate = DateTime.UtcNow;
            message.IsRead = false;
            
            await _contactMessageService.AddAsync(message);
            TempData["SuccessMessage"] = "Mesajınız başarıyla iletildi. En kısa sürede size dönüş yapacağız.";
        }
        else
        {
            TempData["ErrorMessage"] = "Lütfen tüm alanları eksiksiz doldurun.";
        }
        
        // Form gönderildikten sonra sayfanın direkt "İletişim" bölümüne kayarak açılmasını sağlar
        return RedirectToAction(nameof(Index), "Home", null, "iletisim"); 
    }
}