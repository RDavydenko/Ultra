using System;
using System.Threading.Tasks;

namespace Ultra.Core.Services.Abstract
{
    public interface ICrudServiceHelper
    {
        Task<int> GetCountAsync(Type entityType);
    }
}
