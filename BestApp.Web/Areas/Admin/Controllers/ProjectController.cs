using Microsoft.AspNetCore.Mvc;
using BestApp.Core.Entities;
using BestApp.Core.Services;
using Microsoft.AspNetCore.Authorization;

namespace BestApp.Web.Areas.Admin.Controllers;

// Bu Controller'ın Admin bölgesine ait olduğunu belirtmek ZORUNLUDUR
[Area("Admin")] 
[Authorize]
public class ProjectController : Controller
{
    private readonly IProjectService _projectService;
    private readonly IWebHostEnvironment _webHostEnvironment; // Resim yükleme dizinini bulmak için

    public ProjectController(IProjectService projectService, IWebHostEnvironment webHostEnvironment)
    {
        _projectService = projectService;
        _webHostEnvironment = webHostEnvironment;
    }

    // 1. LİSTELEME (READ)
    public async Task<IActionResult> Index()
    {
        // Tüm projeleri getir (Aktif/Pasif fark etmeksizin admin hepsini görmeli)
        var projects = await _projectService.GetAllAsync();
        return View(projects);
    }

    // 2. YENİ EKLEME SAYFASINI GETİRME (CREATE - GET)
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // 3. YENİ EKLENEN VERİYİ KAYDETME VE RESİM YÜKLEME (CREATE - POST)
    [HttpPost]
    [ValidateAntiForgeryToken] // Güvenlik için CSRF koruması
    public async Task<IActionResult> Create(Project project, IFormFile? imageFile)
    {
        if (ModelState.IsValid)
        {
            // Eğer kullanıcı bir resim yüklediyse
            if (imageFile != null && imageFile.Length > 0)
            {
                // wwwroot/uploads/projects klasörünün yolunu al
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "projects");
                
                // Klasör yoksa oluştur
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Benzersiz bir dosya adı oluştur (Çakışmaları önlemek için)
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Dosyayı sunucuya asenkron olarak kopyala
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                // Veritabanına sadece dosyanın adını/yolunu kaydet
                project.ImageUrl = "/uploads/projects/" + uniqueFileName;
            }
            else
            {
                // Resim yüklenmediyse varsayılan bir görsel ata (İsteğe bağlı)
                project.ImageUrl = "/images/default-landscape.jpg";
            }

            await _projectService.AddAsync(project);
            return RedirectToAction(nameof(Index)); // İşlem bitince listeye geri dön
        }
        
        return View(project); // Validasyon hatası varsa aynı sayfaya geri dön
    }

    // 4. SİLME İŞLEMİ (DELETE - POST)
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _projectService.GetByIdAsync(id);
        if (project != null)
        {
            // Sunucudan resmi fiziksel olarak da silebilirsin (Opsiyonel)
            if (!string.IsNullOrEmpty(project.ImageUrl) && !project.ImageUrl.Contains("default"))
            {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, project.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            await _projectService.RemoveAsync(project);
        }
        return RedirectToAction(nameof(Index));
    }
}