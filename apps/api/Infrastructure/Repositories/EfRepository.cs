using Application.Abstractions.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public abstract class EfRepository<T, TId>(AppDbContext context) : IRepository<T, TId> where T : class
{
    protected readonly AppDbContext _context = context;

    public virtual Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default) =>
        _context.Set<T>().FindAsync(new object[] { id! }, cancellationToken).AsTask();

    public virtual Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _context.Set<T>().AsNoTracking().ToListAsync(cancellationToken);

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        await _context.Set<T>().AddAsync(entity, cancellationToken);

    public virtual void Update(T entity) => _context.Set<T>().Update(entity);

    public virtual void Delete(T entity) => _context.Set<T>().Remove(entity);
}

/// <summary>
/// Unit of Work implementation backed by EF Core's SaveChangesAsync.
/// </summary>
public sealed class EfUnitOfWork(AppDbContext context) : IUnitOfWork
{
    public Task<int> CommitAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
