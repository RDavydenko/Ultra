namespace Ultra.Core.Models
{
    public class EntityTypeOutputModel
    {
        public int Id { get; set; }
        public string SystemName { get; set; }
        public string? DisplayName { get; set; }
        public int Count { get; set; }
        public bool Favorite { get; set; }
        public bool IsGeoEntity { get; set; }
    }
}
