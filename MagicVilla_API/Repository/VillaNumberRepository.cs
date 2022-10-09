using System.Linq.Expressions;
using MagicVilla_API.Data;
using MagicVilla_API.Models;
using MagicVilla_API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Repository;

public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository

{
    private readonly ApplicationDbContext _dbContext;
    public VillaNumberRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<VillaNumber> UpdateAsync(VillaNumber entity)
    {
        entity.UpdatedDate = DateTime.Now;
        _dbContext.VillasNumbers.Update(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }
}