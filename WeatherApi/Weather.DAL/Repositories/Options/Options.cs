using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Weather.DAL.Repositories.Options;

public class QueryOptions<T> where T : class
{
    public Expression<Func<T, bool>>? Filter { get; set; }

    public List<Expression<Func<T, object>>> Includes { get; set; } = new();

    public Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy { get; set; }

    public int? Skip { get; set; }
    public int? Take { get; set; }

    public IQueryable<T> Apply(IQueryable<T> query)
    {
        if (Filter != null)
            query = query.Where(Filter);

        foreach (var include in Includes)
            query = query.Include(include);

        if (OrderBy != null)
            query = OrderBy(query);

        if (Skip.HasValue)
            query = query.Skip(Skip.Value);

        if (Take.HasValue)
            query = query.Take(Take.Value);

        return query;
    }
}