﻿using Microsoft.EntityFrameworkCore;
using ProdutosECIA.Infrastructure.DataContext;
using ProdutosECIA.Infrastructure.Repositories.Interfaces;

namespace ProdutosECIA.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>> include = null)
    {
        var query = _dbSet.AsQueryable();

        if (include != null)
        {
            query = include(query);
        }

        return await query.ToListAsync();
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }
        return false;
    }
}