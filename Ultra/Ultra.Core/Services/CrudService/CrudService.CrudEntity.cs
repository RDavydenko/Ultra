using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ultra.Core.DAL;
using Ultra.Core.Entities.Interfaces;
using Ultra.Core.Extensions;
using Ultra.Core.Models;
using Ultra.Core.Services.Abstract;
using Ultra.Extensions;

namespace Ultra.Core.Services.CrudService
{
    public class CrudCrudService<T> : CrudService<T>
        where T : class, ICrudEntity
    {
        private readonly UltraDbContextBase _context;
        private readonly IUserAccessor _userAccessor;

        public CrudCrudService(
            UltraDbContextBase context,
            IPermissionService<T> permissionService,
            IUserAccessor userAccessor)
            : base(context, permissionService)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        protected override async Task<IQueryable<T>> _GetQueryable()
        {
            return (await base._GetQueryable()).Where(x => x.DeleteUserId == null);
        }

        protected override Task BeforeCreate(T model)
        {
            model.CreateUserId =_userAccessor.UserId;
            model.CreateDate = DateTime.Now;

            return Task.CompletedTask;
        }

        protected override Task BeforeUpdate(int id, T model)
        {
            model.UpdateUserId = _userAccessor.UserId;
            model.UpdateDate = DateTime.Now;

            return Task.CompletedTask;
        }

        protected override Task BeforePatch(int id, PatchModel<T> model)
        {
            if (model.Values.Any())
            {
                model.Values.TryAdd(nameof(ICrudEntity.UpdateUserId), _userAccessor.UserId);
                model.Values.TryAdd(nameof(ICrudEntity.UpdateDate), DateTime.Now);
            }

            return Task.CompletedTask;
        }

        protected override Task _DeleteAsync(int id)
        {
            return _context.Set<T>()
                .Where(x => x.Id == id)
                .UpdateFromQueryAsync(new Dictionary<string, object>
                {
                    { nameof(ICrudEntity.DeleteUserId), _userAccessor.UserId },
                    { nameof(ICrudEntity.DeleteDate), DateTime.Now },
                });
        }
    }
}
