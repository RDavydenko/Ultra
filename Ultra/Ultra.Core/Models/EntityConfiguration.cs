using System;
using System.Collections.Generic;
using Ultra.Core.Models.Enums;
using Ultra.Infrastructure.Models.Enums;

namespace Ultra.Core.Models
{
    public class EntityConfiguration
    {
        public string SystemName { get; set; }
        public string? DisplayName { get; set; }
        public EntityMethod[] Methods { get; set; } = Array.Empty<EntityMethod>();
        public List<FieldConfiguration> Fields { get; set; } = new();
        public Dictionary<string, object> Meta { get; set; } = new();
    }

    public class FieldConfiguration
    {
        public string SystemName { get; set; }
        public string? DisplayName { get; set; }
        public FieldType Type { get; set; }
        public FieldMethods[] Methods { get; set; } = Array.Empty<FieldMethods>();
        public Dictionary<string, object> Meta { get; set; } = new();
    }
}
