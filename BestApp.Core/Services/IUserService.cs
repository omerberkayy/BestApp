using BestApp.Core.Entities;

namespace BestApp.Core.Services;

public interface IUserService : IService<AppUser>
{
    Task<AppUser?> ValidateUserAsync(string username, string password);
}