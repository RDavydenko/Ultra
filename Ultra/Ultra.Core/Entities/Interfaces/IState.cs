namespace Ultra.Core.Entities.Interfaces;

public interface IState : IEntity
{
    string Code { get; }
    string Name { get; }
    string Description { get; }
}
