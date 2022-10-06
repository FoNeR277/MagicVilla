using System.Linq.Expressions;
using MagicVilla_API.Data;
using MagicVilla_API.Models;
using MagicVilla_API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DbSet<T> _dbSet;

    protected Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        this._dbSet = _dbContext.Set<T>();
    }
    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
    {
        IQueryable<T> query = _dbSet;
        if(filter is not null)
            query = query.Where(filter);
        return await query.ToListAsync();
    }

    public async Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true)
    {
        IQueryable<T> query = _dbSet;
        if (!tracked)
            query = query.AsNoTracking();
        if(filter is not null)
            query = query.Where(filter);
        return await query.FirstOrDefaultAsync();
    }

    public async Task CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await SaveAsync();
    }

    public async Task RemoveAsync(T entity)
    { 
        _dbSet.Remove(entity);
        await SaveAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await SaveAsync();
    }

    public async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}