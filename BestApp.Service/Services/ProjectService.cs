using BestApp.Core.Entities;
using BestApp.Core.Repositories;
using BestApp.Core.Services;

namespace BestApp.Service.Services;

public class ProjectService : Service<Project>, IProjectService
{
    private readonly IRepository<Project> _repository;

    public ProjectService(IRepository<Project> repository, IUnitOfWork unitOfWork) 
        : base(repository, unitOfWork)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Project>> GetActiveApplicationsAsync()
    {
        // İş kuralı: IsActive özelliği true olanları getir
        return await _repository.GetWhereAsync(x => x.IsActive == true);
    }
}