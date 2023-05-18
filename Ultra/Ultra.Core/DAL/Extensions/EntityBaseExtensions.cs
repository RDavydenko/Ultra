using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Ultra.Core.Entities.Interfaces;

namespace Ultra.Core.DAL.Extensions
{
    public static class EntityBaseExtensions
    {
        public static IQueryable<TEntity> WithId<TEntity>(this IQueryable<TEntity> source, int id)
            where TEntity : class, IEntity
            => source.Where(x => x.Id == id);

        public static Task<TEntity> GetByIdAsync<TEntity>(this IQueryable<TEntity> source, int id)
            where TEntity : class, IEntity
            => source.WithId(id).FirstAsync();

        public static Task<TEntity?> GetOrDefaultByIdAsync<TEntity>(this IQueryable<TEntity> source, int id)
            where TEntity : class, IEntity
            => source.WithId(id).FirstOrDefaultAsync();
    }
}
