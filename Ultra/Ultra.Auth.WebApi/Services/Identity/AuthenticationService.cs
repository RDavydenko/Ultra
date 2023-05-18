using Microsoft.EntityFrameworkCore;
using Ultra.Auth.WebApi.DAL;
using Ultra.Auth.WebApi.DAL.Entities;
using Ultra.Core.DAL.Extensions;
using Ultra.Infrastructure.Models;

namespace Ultra.Auth.WebApi.Services.Identity
{
    internal interface IAuthenticationService
    {
        Task<Result<User>> GetUser(int userId);
        Task<Result<User>> GetUser(string login);
        Task<Result> IsUserExists(int userId);
    }

    internal class AuthenticationService : IAuthenticationService
    {
        private readonly IQueryable<User> _repository;

        public AuthenticationService(AuthDbContext context)
        {
            _repository = context.Set<User>()
                .Include(x => x.Roles)
                .ThenInclude(x => x.Role)
                .ThenInclude(x => x.Permissions)
                .ThenInclude(x => x.Permission)
                .AsSplitQuery();
        }

        public async Task<Result<User>> GetUser(int userId)
        {
            var user = await _repository
                .AsNoTracking()
                .IsActive()
                .GetOrDefaultByIdAsync(userId);

            if (user == null)
            {
                return Result.NotFound<User>();
            }

            return user;
        }

        public async Task<Result<User>> GetUser(string login)
        {
            var user = await _repository
                .AsNoTracking()
                .IsActive()
                .Where(x => x.Login == login)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return Result.NotFound<User>();
            }

            return user;
        }

        public async Task<Result> IsUserExists(int userId)
        {
            var exists = await _repository
                .IsActive()
                .WithId(userId)
                .AnyAsync();

            if (!exists)
            {
                return Result.Failed();
            }

            return Result.Success();
        }
    }
}
