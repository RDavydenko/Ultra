using System;

namespace Ultra.Core.Entities.Interfaces
{
    public interface IStatefulEntity<TState>
        where TState : struct, Enum
    {
        TState State { get; }
    }
}
