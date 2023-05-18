using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultra.Core.Entities.Extensions;
using Ultra.Core.Extensions;
using Ultra.Core.Tools;

namespace Ultra.Core.Mapping
{
    public class CodeGenerationConfig : ICodeGenerationRegister
    {
        public void Register(Mapster.CodeGenerationConfig config)
        {
            var entities = Executor.WebAssembly.GetDbEntities();
            Console.WriteLine($"Количество сущностей: {entities.Count()}");

            foreach (var entity in entities)
            {
                if (entity.IsAvailableToCreate())
                {
                    config.AdaptTo("[name]CreateModel")
                        .ForTypes(entity);
                }
            }
        }
    }
}
