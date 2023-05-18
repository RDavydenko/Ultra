using Microsoft.EntityFrameworkCore;
using Ultra.Core.DAL;

namespace Ultra.Core.WebApi.DAL
{
    public class UltraDbContext : UltraDbContextBase
    {
        public override string Schema => "ult_crm";

        
        public UltraDbContext(DbContextOptions<UltraDbContext> options)
            : base(options)
        {

        }
    }
}
