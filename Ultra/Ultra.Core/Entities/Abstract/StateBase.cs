using Ultra.Core.Entities.Interfaces;

namespace Ultra.Core.Entities.Abstract;

public abstract class StateBase : EntityBase, IState
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
