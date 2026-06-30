using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BestApp.Core.Entities;
using BestApp.Core.Services;

namespace BestApp.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class ProjectController : Controller
{
    private readonly IService<Project> _projectService;
    private readonly IWebHostEnvironment _webHostEnvironment; // Sunucu klasörlerine erişim için

    public ProjectController(IService<Project> projectService, IWebHostEnvironment webHostEnvironment)
    {
        _projectService = projectService;
        _webHostEnvironment = webHostEnvironment;
    }

    // 1. Projeleri Listeleme Sayfası (Index)
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var projects = await _projectService.GetAllAsync();
        return View(projects);
    }

    // 2. Yeni Proje Ekleme Sayfası (Get)
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // 3. Yeni Proje Ekleme ve Dosya Yükleme (Post)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Project project, IFormFile imageFile)
    {
        // Not: "imageFile" parametre adı, HTML'deki <input type="file" name="imageFile"> ile birebir aynı olmalıdır.
        
        if (imageFile != null && imageFile.Length > 0)
        {
            // A. wwwroot/images/applications klasörünün yolunu belirle
            string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "projects");
            
            // B. Klasör bilgisayarda yoksa otomatik oluştur
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            // C. Aynı isimde iki resim yüklenirse çakışmasın diye benzersiz bir isim üret (Guid)
            string extension = Path.GetExtension(imageFile.FileName); // .jpg, .png vb.
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            
            // D. Dosyanın sunucuda kaydedileceği tam fiziksel yol
            string filePath = Path.Combine(uploadFolder, uniqueFileName);

            // E. Dosyayı fiziksel olarak klasöre kopyala
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            // F. Veritabanına kaydedilecek web adresini entity'ye ata
            project.ImageUrl = $"/images/projects/{uniqueFileName}";
        }
        else
        {
            // Eğer resim seçilmediyse kullanıcıya hata göster
            ModelState.AddModelError("imageFile", "Lütfen proje için bir görsel seçin.");
            return View(project);
        }
        ModelState.Remove("ImageUrl");

        // G. Geri kalan verileri (Başlık, Açıklama vb.) doğrula ve veritabanına kaydet
        if (ModelState.IsValid)
        {
            await _projectService.AddAsync(project);
            return RedirectToAction(nameof(Index)); // Başarılıysa listeleme sayfasına dön
        }

        return View(project);
    }

    // 4. Proje Silme İşlemi (Post)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _projectService.GetByIdAsync(id);
        
        if (project != null)
        {
            // Opsiyonel: Proje silindiğinde sunucudaki fiziksel resim dosyasını da silerek yer açalım
            if (!string.IsNullOrEmpty(project.ImageUrl))
            {
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, project.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            await _projectService.RemoveAsync(project);
        }

        return RedirectToAction(nameof(Index));
    }
}