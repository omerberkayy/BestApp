using BestApp.Core.Entities;

namespace BestApp.Core.Services;

// IService'in yeteneklerini miras alıyoruz ve ekstra metotlar ekliyoruz
public interface IProjectService : IService<Project>
{
    // Sadece aktif (yayında olan) peyzaj projelerini getirecek özel metot
    Task<IEnumerable<Project>> GetActiveApplicationsAsync();
}