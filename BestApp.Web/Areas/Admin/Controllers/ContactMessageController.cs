using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BestApp.Core.Entities;
using BestApp.Core.Services;

namespace BestApp.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize] // Sadece giriş yapmış yetkililer görebilir
public class ContactMessageController : Controller
{
    private readonly IService<ContactMessage> _contactMessageService;

    public ContactMessageController(IService<ContactMessage> contactMessageService)
    {
        _contactMessageService = contactMessageService;
    }

    // 1. Gelen Kutusu (Liste)
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var messages = await _contactMessageService.GetAllAsync();
        
        // Önce okunmamış mesajlar üstte görünsün, ardından en yeniler sıralansın
        var sortedMessages = messages
            .OrderBy(m => m.IsRead)
            .ThenByDescending(m => m.SentDate)
            .ToList();
            
        return View(sortedMessages);
    }

    // 2. Mesaj Okuma Sayfası
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var message = await _contactMessageService.GetByIdAsync(id);
        if (message == null) return NotFound();

        // Eğer mesaj ilk defa açılıyorsa, "Okundu" olarak işaretle ve veritabanını güncelle
        if (!message.IsRead)
        {
            message.IsRead = true;
            await _contactMessageService.UpdateAsync(message);
        }

        return View(message);
    }

    // 3. Mesaj Silme İşlemi
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var message = await _contactMessageService.GetByIdAsync(id);
        if (message != null)
        {
            await _contactMessageService.RemoveAsync(message);
        }
        return RedirectToAction(nameof(Index));
    }
}