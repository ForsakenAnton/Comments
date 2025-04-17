// Ignore Spelling: Funcs

using Entities.Models;
using System.Linq.Expressions;

namespace Contracts;

public interface IRepositoryBase<T>
{
    IQueryable<T> FindAll(bool trackChanges);

    //public IQueryable<T> FindAllWithIncludes<TEntity>(
    //    bool trackChanges,
    //    params Expression<Func<T, TEntity>>[] includes);

    public IQueryable<T> FindAllWithIncludes(
    bool trackChanges,
    params Expression<Func<T, object>>[] includes);

    IQueryable<T> FindByConditionWithIncludes(
        Expression<Func<T, bool>> expression,
        bool trackChanges,
        params Expression<Func<T, object>>[] includeExpressions);

    public IQueryable<T> FindAllWithNestedIncludes(
        bool trackChanges,
        params Func<IQueryable<T>, IQueryable<T>>[] includeFuncs);

    void Create(T entity);
    Task CreateAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}
