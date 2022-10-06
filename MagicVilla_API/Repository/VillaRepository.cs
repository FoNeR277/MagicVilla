using System.Linq.Expressions;
using MagicVilla_API.Data;
using MagicVilla_API.Models;
using MagicVilla_API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Repository;

public class VillaRepository : Repository<Villa>, IVillaRepository

{
    private readonly ApplicationDbContext _dbContext;
    public VillaRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Villa> UpdateAsync(Villa entity)
    {
        entity.UpdatedDate = DateTime.Now;
        _dbContext.Villas.Update(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }
}