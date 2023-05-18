using Mapster;
using Microsoft.EntityFrameworkCore;
using Ultra.Auth.WebApi.DAL;
using Ultra.Auth.WebApi.DAL.Entities;
using Ultra.Auth.WebApi.Models;
using Ultra.Auth.WebApi.Services.Interfaces;
using Ultra.Core.DAL.Extensions;
using Ultra.Core.Services.Abstract;
using Ultra.Extensions;
using Ultra.Infrastructure.Models;

namespace Ultra.Auth.WebApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserAccessor _userAccessor;
        private readonly IQueryable<User> _repository;

        public UserService(
            AuthDbContext context,
            IUserAccessor userAccessor)
        {
            _repository = context.Set<User>()
                .Include(x => x.Roles)
                .ThenInclude(x => x.Role)
                .ThenInclude(x => x.Permissions)
                .ThenInclude(x => x.Permission);
            _userAccessor = userAccessor;
        }

        public async Task<Result<UserModel>> GetUser(int userId)
        {
            var user = await _repository
                .WithId(userId)
                .ProjectToType<UserModel>()
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return Result.NotFound<UserModel>();
            }

            return Result.Success(user);
        }

        public async Task<Result> ExistsUser(int userId)
        {
            var exists = await _repository
                .WithId(userId)
                .AnyAsync();

            if (!exists)
            {
                return Result.NotFound();
            }

            return Result.Success();
        }

        public async Task<Result<UserModel>> GetUser(string login)
        {
            var user = await _repository
                .Where(x => x.Login == login)
                .ProjectToType<UserModel>()
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return Result.NotFound<UserModel>();
            }

            return Result.Success(user);
        }

        public async Task<Result<string>> GetUserName(int userId)
        {
            var user = await _repository
                .WithId(userId)
                .Select(x => new { x.UserName })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return Result.NotFound<string>();
            }

            return Result.Success(user.UserName);
        }

        public async Task<Result<CollectionPage<UserShortModel>>> SearchUsers(string? userName, int? userId)
        {
            return await _repository
                .WhereIf(x => x.UserName.ToLower().Contains(userName!.ToLower()), userName.IsNotNullOrEmpty())
                .WhereIf(x => x.Id == userId!.Value, userId.HasValue)
                .ProjectToType<UserShortModel>()
                .ToCollectionPageAsync(PageModel.Default);
        }

        public Task<Result<UserModel>> GetCurrentUser() =>
            GetUser(_userAccessor.UserId);

        public async Task<Result<List<RoleModel>>> GetUserRoles(int userId)
        {
            var roles = await _repository
                .WithId(userId)
                .SelectMany(x => x.Roles.Select(x => x.Role))
                .ProjectToType<RoleModel>()
                .ToListAsync();

            return roles;
        }
    }
}
