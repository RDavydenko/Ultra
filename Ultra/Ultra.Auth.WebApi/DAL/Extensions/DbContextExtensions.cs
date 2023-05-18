using Microsoft.EntityFrameworkCore;
using Ultra.Auth.WebApi.DAL.Entities;
using Ultra.Auth.WebApi.Services.Identity;
using Ultra.Core.Extensions;
using Ultra.Extensions;

namespace Ultra.Auth.WebApi.DAL.Extensions
{
    public static class DbContextExtensions
    {
        internal static async Task SeedAsync(this AuthDbContext context, IServiceProvider provider)
        {
            if (await context.Set<User>().IgnoreQueryFilters().AnyAsync())
            {
                return;
            }

            var hashService = provider.GetRequiredService<IPasswordHashService>();
            string salt;

            var user1 = new User()
            {
                Login = "1",
                UserName = "Наблюдатель",
                Salt = salt = hashService.GetSalt().ToBase64(),
                PasswordHash = hashService.GetHash("password123", salt.FromBase64()),
            };
            var user2 = new User()
            {
                Login = "2",
                UserName = "Редактор",
                Salt = salt = hashService.GetSalt().ToBase64(),
                PasswordHash = hashService.GetHash("password123", salt.FromBase64()),
            };
            var user3 = new User()
            {
                Login = "3",
                UserName = "Администратор",
                Salt = salt = hashService.GetSalt().ToBase64(),
                PasswordHash = hashService.GetHash("password123", salt.FromBase64()),
            };

            context.Set<User>().Add(user1);
            context.Set<User>().Add(user2);
            context.Set<User>().Add(user3);

            await context.SaveChangesAsync();
        }
    }
}
