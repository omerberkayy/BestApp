using BestApp.Core.Entities;

namespace BestApp.Web.Models;

public class HomeViewModel
{
    public About? AboutInfo { get; set; }
    public IEnumerable<Core.Entities.Service> Services { get; set; } = new List<Core.Entities.Service>();
    public IEnumerable<Project> Projects { get; set; } = new List<Project>();
    
    // İletişim formu için boş bir nesne
    public ContactMessage ContactForm { get; set; } = new ContactMessage(); 
}