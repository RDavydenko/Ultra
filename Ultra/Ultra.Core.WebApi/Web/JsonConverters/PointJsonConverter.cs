using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ultra.Core.WebApi.Web.JsonConverters
{
    public class PointJsonConverter : JsonConverter<Point>
    {
        public override Point? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var values = ReadValues(ref reader);
            var coordinates = (double[])values["coordinates"];
            return new Point(coordinates[0], coordinates[1]);
        }

        private Dictionary<string, object> ReadValues(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var props = new[] { "coordinates" };
            var dictionary = new Dictionary<string, object>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject) return dictionary;

                // Get the key.
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();

                var propertyName = reader.GetString()!;
                reader.Read();

                if (!props.Contains(propertyName)) continue;

                if (propertyName == "coordinates")
                {
                    if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();
                    reader.Read();

                    var array = new List<double>();

                    while (reader.TokenType != JsonTokenType.EndArray)
                    {
                        array.Add(reader.GetDouble());
                        reader.Read();
                    }

                    dictionary.Add(propertyName, array.ToArray());
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Point value, JsonSerializerOptions options)
        {
            writer.WriteNullValue();
        }
    }
}
