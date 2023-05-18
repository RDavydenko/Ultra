using System;

namespace Ultra.Core.Entities.Interfaces
{
    public interface IGuidEntity : IDbEntity
    {
        Guid Guid { get; }
    }
}
