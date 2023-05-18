using System.Linq;
using System.Threading.Tasks;
using Ultra.Core.Entities.Interfaces;
using Ultra.Core.Models;
using Ultra.Infrastructure.Models;

namespace Ultra.Core.Services.Abstract
{
    public interface ICrudService<T>
        where T : class, IEntity
    {
        Task<IQueryable<T>> GetQueryable();
        Task<Result<T>> GetAsync(int id);
        Task<Result<T>> CreateAsync(T model, RelatedLink[]? linksToAddOrUpdate = null, RelatedLink[]? linksToDelete = null);
        Task<Result<T>> UpdateAsync(int id, T model, RelatedLink[]? linksToAddOrUpdate = null, RelatedLink[]? linksToDelete = null);
        Task<Result<T>> PatchAsync(int id, PatchModel<T> model);
        Task<Result> DeleteAsync(int id);
    }
}
