using System;
using Ultra.Core.Entities.Interfaces;

namespace Ultra.Core.Entities.Abstract;

public abstract class StatefulEntityBase<TState> : EntityBase, IStatefulEntity<TState>
    where TState : struct, Enum
{
    public TState State { get; set; }
}
