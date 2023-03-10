using Gatherly.Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Gatherly.Persistence.Specifications;

public static class SpecificationEvaluator
{
    public static IQueryable<TEntity> GetQuery<TEntity>(
        IQueryable<TEntity> inputQueryable,
        Specification<TEntity> specification)
        where TEntity : Entity
    {
        IQueryable<TEntity> queryable = inputQueryable;

        if (specification.Criteria is not null)
        {
            queryable = queryable.Where(specification.Criteria);
        }

        specification.IncludeExpressions.Aggregate(
            queryable,
            (current, includeExpression) =>
                current.Include(includeExpression));

        if (specification.OrderByExpression is not null)
        {
            queryable = queryable.OrderBy(specification.OrderByExpression);
        }
        else if (specification.OrderByDescendingExpression is not null)
        {
            queryable = queryable.OrderByDescending(
                specification.OrderByDescendingExpression);
        }
        if (specification.GroupByExpression != null)
        {
            queryable = queryable.GroupBy(specification.GroupByExpression).SelectMany(x => x);
        }
        if (specification.IsSplitQuery)
        {
            queryable = queryable.AsSplitQuery();
        }
        if (specification.IsPagingEnabled)
        {
            queryable = queryable.Skip(specification.Skip)
                         .Take(specification.Take);
        }
        return queryable;
    }
}
