namespace BestApp.Core.Repositories;

public interface IUnitOfWork
{
    Task CommitAsync(); // Değişiklikleri kaydet
    void Commit();
}