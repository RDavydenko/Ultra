using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultra.Core.DAL.Entities;
using Ultra.Core.Models;

namespace Ultra.Core.Web.Mapping
{
    internal class EntityTypeMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<EntityType, EntityTypeOutputModel>()
                .Map(trg => trg.SystemName, src => src.SystemName);
        }
    }
}
