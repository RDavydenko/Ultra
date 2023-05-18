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
    [Display("Город", DisplayableField = nameof(Name))]
    [Read, Create, Update, Delete]
    public class City : CrudEntityBase
    {
        [Created, Updated, Patched]
        [Display("Название"), Required]
        public string Name { get; set; }

        [Created, Updated, Patched]
        [Display("Год основания"), Year]
        public int? FoundationYear { get; set; }

        [Created, Updated, Patched]
        [Range(Min = 0), Required]
        public int Population { get; set; }

        [Created, Updated, Patched]
        [Display("Местоположение")]
        public Point? Location { get; set; }

        [Created, Updated, Patched]
        [Display("Постройки")]
        public ICollection<House> Houses { get; set; }
    }
}
