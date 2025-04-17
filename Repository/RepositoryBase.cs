// Ignore Spelling: Funcs

using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Repository;

public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    protected RepositoryContext RepositoryContext;

    public RepositoryBase(RepositoryContext repositoryContext)
    {
        RepositoryContext = repositoryContext;
    }

    public IQueryable<T> FindAll(bool trackChanges) =>
        !trackChanges ?
            RepositoryContext.Set<T>()
                .AsNoTracking() :
            RepositoryContext.Set<T>();

    public IQueryable<T> FindAllWithIncludes(
        bool trackChanges,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = RepositoryContext.Set<T>();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return query;
    }

    public IQueryable<T> FindByConditionWithIncludes(
        Expression<Func<T, bool>> expression,
        bool trackChanges,
        params Expression<Func<T, object>>[] includes)
    {
        var query = RepositoryContext.Set<T>().Where(expression);

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return query;
    }

    public IQueryable<T> FindAllWithNestedIncludes(
    bool trackChanges,
    params Func<IQueryable<T>, IQueryable<T>>[] includeFuncs)
    {
        IQueryable<T> query = RepositoryContext.Set<T>();

        foreach (var include in includeFuncs)
        {
            query = include(query);
        }

        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return query;
    }

    public void Create(T entity) => RepositoryContext.Set<T>().Add(entity);
    public async Task CreateAsync(T entity) => await RepositoryContext.Set<T>().AddAsync(entity);
    public void Update(T entity) => RepositoryContext.Set<T>().Update(entity);
    public void Delete(T entity) => RepositoryContext.Set<T>().Remove(entity);
}