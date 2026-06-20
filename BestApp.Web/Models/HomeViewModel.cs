using BestApp.Core.Entities;
using EntityService = BestApp.Core.Entities.Service;

namespace BestApp.Web.Models;

public class HomeViewModel
{
    // Ana sayfada hem projeleri hem de hizmetleri göstermek için listeler tanımlıyoruz
    public IEnumerable<Project> Projects { get; set; } = new List<Project>();
    public IEnumerable<EntityService> Services { get; set; } = new List<EntityService>();
}