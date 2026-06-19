using System.Linq.Expressions;

namespace BestApp.Core.Repositories;

public interface IRepository<T> where T : class
{
    // Okuma İşlemleri
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    // LINQ sorguları yazabilmek için (Örn: Sadece IsActive == true olanları getir)
    Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate); 

    // Yazma İşlemleri
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}