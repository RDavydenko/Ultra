using System.Collections.Generic;
using System.Text.Json;
using Ultra.Core.Entities.Interfaces;

namespace Ultra.Core.Models
{
    public class PatchModel<T>
        where T : class, IEntity
    {
        public Dictionary<string, object> Values { get; set; }

        public PatchModel(Dictionary<string, object> values)
        {
            Values = values ?? new Dictionary<string, object>();
        }

        internal Dictionary<string, object> GetValues() => Values.ExtractJsonValues<T>();
    }

    internal static class PatchModelExtensions
    {
        public static Dictionary<string, object> ExtractJsonValues<T>(this Dictionary<string, object> dict)
        {
            var res = new Dictionary<string, object>(dict.Count);
            foreach (var pair in dict)
            {
                var propName = pair.Key;
                var propValue = pair.Value;

                if (pair.Value is JsonElement j)
                {
                    var propType = typeof(T).GetProperty(propName)?.PropertyType;
                    if (propType != null)
                    {
                        res.Add(propName, j.Deserialize(propType));
                    }

                    continue;
                }

                res.Add(propName, propValue);
            }

            return res;
        }
    }
}
