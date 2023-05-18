//using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;

//namespace Ultra.Core.DAL.Configuration
//{
//    public interface IDatabaseConfiguration
//    {
//        void Configure(IDatabaseConfigurationBuilder config);
//    }

//    public interface IDatabaseConfigurationBuilder
//    {
//        IEntityConfigurationBuilder<TEntity> Add<TEntity>(string? tableName = null)
//            where TEntity : class;
//    }

//    public interface IEntityConfigurationBuilder<TEntity>
//        where TEntity : class
//    {
//        IEntityConfigurationBuilder<TEntity> WithOne<TDepend>(Expression<Func<TDepend, object>>? foreignKey = null);
//        IEntityConfigurationBuilder<TEntity> WithMany<TDepend>(Expression<Func<TDepend, object>>? foreignKey = null);
//    }

//    internal class DatabaseConfigurationBuilder : IDatabaseConfigurationBuilder
//    {
//        private readonly List<EntityDbConfig> _configs = new();
//        private readonly List<ConnectionPower> _connections = new();
//        private EntityDbConfig _current;

//        public IEntityConfigurationBuilder<TEntity> Add<TEntity>(string? tableName = null)
//            where TEntity : class
//        {
//            var entityType = typeof(TEntity);
//            var sysName = entityType.Name;
//            tableName ??= GetName(sysName);

//            _current = new EntityDbConfig
//            {
//                EntityType = entityType,
//                SystemName = sysName,
//                TableName = tableName,
//            };

//            _configs.Add(_current);

//            return new EntityConfigurationBuilder<TEntity>(_current, _connections);
//        }

//        // TODO: to upper snake case
//        private string GetName(string name) => name;
//    }

//    internal class EntityConfigurationBuilder<TEntity> : IEntityConfigurationBuilder<TEntity>
//        where TEntity : class
//    {
//        private EntityDbConfig _current;
//        private readonly List<ConnectionPower> _connections;

//        public EntityConfigurationBuilder(EntityDbConfig current, List<ConnectionPower> connections)
//        {
//            _current = current;
//            _connections = connections;
//        }

//        public IEntityConfigurationBuilder<TEntity> WithMany<TDepend>(Expression<Func<TDepend, object>>? foreignKey = null)
//        {
//            throw new NotImplementedException();
//        }

//        public IEntityConfigurationBuilder<TEntity> WithOne<TDepend>(Expression<Func<TDepend, object>>? foreignKey = null)
//        {
//            throw new NotImplementedException();
//        }

//        private ConnectionPower GetOrCreateConnectionPower<TEntity, TDepend>()
//        {
//            _connections.SingleOrDefault(x => x.Entity1 == typeof(TEntity) || x.Entity2 == typeof(TDepend))
//        }
//    }

//    public class EntityDbConfig
//    {
//        public Type EntityType { get; set; }
//        public string TableName { get; set; }
//        public string SystemName { get; set; }
//    }

//    public class ConnectionPower
//    {
//        public Type Entity1 { get; set; }
//        public Type Entity2 { get; set; }
//        public PowerType Power { get; set; }
//        public string Entity1ForeignKey { get; set; }
//        public string Entity2ForeignKey { get; set; }

//        public enum PowerType
//        {
//            OneToOne,
//            OneToMany,
//            ManyToOne,
//            ManyToMany
//        }
//    }

//    /*
//     * public void Configure(config) 
//     * {
//     *      config.Add<User>()
//     *          .WithMany<UserRole>(x => x.UserId);
//     *          
//     *      => config.HasMany(x => x.UserRoles).WithOne().HasForeignKey(x => x.UserId);
//     *         
//     *         
//     *          
//     *      config.Add<UserRole>("USER_ROLE")
//     *          .WithOne<User>()
//     *          .WithOne<Role>();
//     *          
//     *      => config.ToTable("USER_ROLE");
//     *      => config.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
//     *      => config.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId);
//     * 
//     * 
//     * 
//     *      config.Add<Permission>()
//     *          .WithMany<Role>(x => x.RoleId);
//     *      
//     *      => config.HasMany(x => x.Roles).WithMany().HasForeignKey(x => x.RoleId);
//     *          
//     *          
//     *          
//     *      config.Add<Role>()
//     *          .WithMany<Permission>(x => x.PermissionId);
//     * 
//     *      => config.HasMany(x => x.Permissions).WithMany().HasForeignKey(x => x.PermissionId);
//     * }
//     */
//}
