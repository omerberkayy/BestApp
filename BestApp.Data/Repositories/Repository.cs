using Microsoft.EntityFrameworkCore;
using BestApp.Core.Repositories;
using System.Linq.Expressions;

namespace BestApp.Data.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    // Sadece bu sınıftan miras alanların erişebilmesi için protected kullanıyoruz
    protected readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        // FindAsync, id'ye göre arama yapmanın en hızlı yoludur
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        // AsNoTracking(), sadece okuma yapacağımız için EF Core'un veriyi takip etmesini engeller, performansı artırır.
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).AsNoTracking().ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        // Güncelleme ve silme işlemleri EF Core'da asenkron değildir, senkron çalışır.
        _dbSet.Update(entity);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }
}