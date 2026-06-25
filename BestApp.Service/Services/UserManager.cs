using BestApp.Core.Entities;
using BestApp.Core.Repositories;
using BestApp.Core.Services;
using BestApp.Core.Utilities;

namespace BestApp.Service.Services;

public class UserManager : Service<AppUser>, IUserService
{
    private readonly IRepository<AppUser> _repository;

    public UserManager(IRepository<AppUser> repository, IUnitOfWork unitOfWork) : base(repository, unitOfWork)
    {
        _repository = repository;
    }

    public async Task<AppUser?> ValidateUserAsync(string username, string password)
    {
        string hashedInput = PasswordHelper.HashPassword(password);
        
        var users = await _repository.GetWhereAsync(x => 
            x.Username == username && 
            x.PasswordHash == hashedInput && 
            x.IsActive == true);

        return users.FirstOrDefault();
    }
}