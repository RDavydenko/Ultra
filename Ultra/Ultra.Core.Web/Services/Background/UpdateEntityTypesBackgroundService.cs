using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ultra.Core.DAL;
using Ultra.Core.DAL.Entities;
using Ultra.Core.Entities.Attributes;
using Ultra.Core.Extensions;
using Ultra.Core.Tools;

namespace Ultra.Core.Web.Services.Background
{
    public class UpdateEntityTypesBackgroundService : BackgroundService
    {
        private static readonly TimeSpan _period = new TimeSpan(0, 5, 0);
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UpdateEntityTypesBackgroundService> _logger;

        public UpdateEntityTypesBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<UpdateEntityTypesBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(_period);

            do
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    using var context = scope.ServiceProvider.GetRequiredService<UltraDbContextBase>();

                    var dbEntities = Executor.WebAssembly.GetDbEntities()
                        .Select(x => new
                        {
                            SystemName = x.Name,
                            DisplayName = x.GetAttribute<DisplayAttribute>()?.DisplayName
                        })
                        .ToList();

                    _logger.LogInformation($"[{nameof(UpdateEntityTypesBackgroundService)}]: Начато обновление записей в ENTITY_TYPE. " +
                        $"Сущностей в сборке: {dbEntities.Count}");

                    var entityTypes = await context.Set<EntityType>().ToListAsync();

                    var entityTypesToDelete = entityTypes
                        .Where(x => !dbEntities.Select(m => m.SystemName).Contains(x.SystemName))
                        .ToList();
                    var entityTypesToAdd = dbEntities
                        .Where(x => !entityTypes.Select(m => m.SystemName).Contains(x.SystemName))
                        .Select(x => new EntityType
                        {
                            SystemName = x.SystemName,
                            DisplayName = x.DisplayName
                        })
                        .ToList();

                    context.Set<EntityType>().RemoveRange(entityTypesToDelete);
                    context.Set<EntityType>().AddRange(entityTypesToAdd);

                    await context.SaveChangesAsync();

                    _logger.LogInformation($"[{nameof(UpdateEntityTypesBackgroundService)}]: Завершено обновление записей в ENTITY_TYPE. " +
                        $"Добавлено: {entityTypesToAdd.Count}. Удалено: {entityTypesToDelete.Count}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[{nameof(UpdateEntityTypesBackgroundService)}]: Ошибка");
                }

            } while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken));

            _logger.LogInformation($"[{nameof(UpdateEntityTypesBackgroundService)}]: Отмена задачи");
        }
    }
}
