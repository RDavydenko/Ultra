using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultra.Core.Entities.Extensions;
using Microsoft.EntityFrameworkCore;
using Ultra.Core.Extensions;
using System.Linq.Expressions;
using Ultra.Infrastructure.Models;
using Ultra.Extensions;

namespace Ultra.Core.DAL.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> AutoInclude<T>(this IQueryable<T> source)
            where T : class
        {
            typeof(T).GetProperties()
                .Where(p => p.GetMethod?.IsVirtual == true && p.PropertyType.IsClass)
                .Select(p => p.Name)
                .ForEach(name => source.Include(name));

            return source;
        }

        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, Expression<Func<T, bool>> whereExpr, bool condition)
        {
            return condition ? source.Where(whereExpr) : source;
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> source, PageModel? pageModel)
        {
            if (pageModel == null)
            {
                return source;
            }

            if (pageModel.Offset < 0 || pageModel.PageSize <= 0)
            {
                return source;
            }

            return source.Skip(pageModel.Offset).Take(pageModel.PageSize);
        }

        public static async Task<CollectionPage<T>> ToCollectionPageAsync<T>(this IQueryable<T> source, PageModel? pageModel)
        {
            var totalCount = await source.CountAsync();
            var items = await source.Page(pageModel).ToListAsync();

            var pagesCount = 0;
            if (pageModel != null && pageModel.Offset >= 0 && pageModel.PageSize != 0)
            {
                pagesCount = (totalCount / pageModel.PageSize) + ((totalCount % pageModel.PageSize) > 0 ? 1 : 0);
            }

            return new CollectionPage<T>(
                items, 
                new PageInfo
                {
                    Pages = pagesCount,
                    TotalCount = totalCount,
                }
            );
        }
    }
}
