namespace Ultra.Infrastructure.Models
{
    public class EntityPermissionModel
    {
        public bool AnyAccess { get; set; }
        public bool AllAccess { get; set; }
        public int[] IncludeIds { get; set; } = Array.Empty<int>();
        public int[] ExcludeIds { get; set; } = Array.Empty<int>();
    }
}
