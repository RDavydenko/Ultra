using System;
using System.Linq;
using Ultra.Core.Entities.Interfaces;
using Ultra.Core.Entities.States;

namespace Ultra.Core.DAL.Extensions
{
    public static class StatefulEntityExtensions
    { 
        public static IQueryable<TEntity> WithState<TEntity, TState>(this IQueryable<TEntity> source, TState state)
            where TEntity : class, IStatefulEntity<TState>
            where TState : struct, Enum
            => source.Where(x => x.State.Equals(state));

        public static IQueryable<TEntity> IsActive<TEntity>(this IQueryable<TEntity> source)
            where TEntity : class, IStatefulEntity<State>
            => source.WithState(State.ACTIVE);

        public static bool IsActive<TEntity>(this TEntity entity)
            where TEntity : class, IStatefulEntity<State>
            => entity.State == State.ACTIVE;
    }
}
