using Microsoft.EntityFrameworkCore;
using System.Net;
using Ultra.Core.DAL.Entities;
using Ultra.Core.WebApi.DAL.Entities;

namespace Ultra.Core.WebApi.DAL.Extensions
{
    public static class DbContextExtensions
    {
        internal static async Task SeedAsync(this UltraDbContext context, IServiceProvider provider)
        {
            //context.Set<House>().RemoveRange(context.Set<House>());

            //try
            //{
            //    var items = await context.Set<House>()
            //        .DistinctBy(x => x.OwnerId)
            //        .Select(x => new { x.OwnerId })
            //        .ToListAsync();
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}

            await CreateHouseIfNotExists(context, "г.Барнаул, ул.1905 года, д.25", 53.35658719801987, 83.77415222010409);
            await CreateHouseIfNotExists(context, "г.Барнаул, ул.Советская, д.9", 53.354423014359746, 83.77325453310554);
            await CreateHouseIfNotExists(context, "г.Барнаул, пр-т Ленина, д.89", 53.35499351863006, 83.76792336026571);
            await CreateHouseIfNotExists(context, "г.Барнаул, пр-т Ленина, д.71", 53.35102629069618, 83.7720089835038);
            await CreateHouseIfNotExists(context, "г.Барнаул, пр-т Ленина, д.46", 53.345184914171725, 83.78229251322286);
            await CreateHouseIfNotExists(context, "г.Барнаул, ул.Молодежная, д.55", 53.34203318542668, 83.76161849985684);
            await CreateHouseIfNotExists(context, "г.Барнаул, пр-д Пирогова, д.105", 53.34937227679782, 83.73922986725076);
            await CreateHouseIfNotExists(context, "г.Барнаул, ул.Балтийская, д.44", 53.3360687260749, 83.67340383383245);
            await CreateHouseIfNotExists(context, "г.Барнаул, Павловский тр-т, д.188", 53.34982611090051, 83.63660533088327);
            await CreateHouseIfNotExists(context, "г.Барнаул, Павловский тр-т, д.188В", 53.34841673311036, 83.63937602687683);

            await CreateHouseIfNotExists(context, "г.Москва, Красная площадь, Мавзолей В.И. Ленина", 55.75370626425523, 37.61988264813156);

            await context.SaveChangesAsync();
        }

        private static async Task CreateHouseIfNotExists(UltraDbContext context, string address, double lat, double lng)
        {
            if (!await context.Set<House>().AnyAsync(x => x.Address == address))
            {
                context.Set<House>().Add(new House
                {
                    CreateUserId = 4,
                    CreateDate = DateTime.Now,
                    Address = address,
                    Location = new NetTopologySuite.Geometries.Point(lat, lng)
                });
            }
        }
    }
}
