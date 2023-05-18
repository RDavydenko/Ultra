using DelegateDecompiler;
using Microsoft.Linq.Translations;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;
using Ultra.Core.Entities.Abstract;
using Ultra.Core.Entities.Attributes;
using Ultra.Core.Entities.Attributes.Access;
using Ultra.Core.Entities.Attributes.Data.Methods;
using Ultra.Core.Entities.Attributes.Meta;

namespace Ultra.Core.WebApi.DAL.Entities
{
    [Display("Строение", DisplayableField = nameof(Address))]
    [Read, Create, Update, Delete]
    public class House : CrudEntityBase
    {
        [Created, Updated, Patched]
        [Range(Min = 0, Max = 300)]
        public string? Address { get; set; }

        [Created, Patched]
        [Year]
        public int? BuildingYear { get; set; }

        [Created, Patched]
        [Range(Min = 10, Max = 50), Required]
        public int MagicNumber { get; set; }

        [Created, Updated, Patched]
        [ForeignKey(nameof(Owner))]
        public int? OwnerId { get; set; }        
        public virtual Man? Owner { get; set; }

        [Created, Updated, Patched]
        public Point? Location { get; set; }

        [Created, Updated, Patched]
        [ForeignKey(nameof(City))]
        [Display("Город")]
        public int? CityId { get; set; }

        [Display("Город")]
        public virtual City? City { get; set; }
    }
}
