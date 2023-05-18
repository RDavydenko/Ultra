using System.Collections.Generic;
using System.Threading.Tasks;
using Ultra.Core.Models;
using Ultra.Infrastructure.Models;

namespace Ultra.Core.Services.Abstract
{
    public interface IEntityService
    {
        Task<Result<CollectionPage<EntityTypeOutputModel>>> GetEntityTypesPage(PageModel? pageModel = null);
        Task<Result<List<EntityTypeShortOutputModel>>> GetGeoEntityTypes();
        Task<Result<EntityConfiguration>> GetConfiguration(string sysName);
        Task<Result> ToggleFavorite(string sysName, int? id = null);
        Task<Result<bool>> GetEntityFavorite(string sysName, int id);
    }
}
