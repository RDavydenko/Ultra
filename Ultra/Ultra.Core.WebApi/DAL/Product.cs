using Ultra.Core.Entities.Abstract;
using Ultra.Core.Entities.Attributes;
using Ultra.Core.Entities.Attributes.Access;
using Ultra.Core.Entities.Attributes.Data.Methods;
using Ultra.Core.Entities.Attributes.Meta;
using Ultra.Core.WebApi.DAL.Entities;

namespace Ultra.Core.WebApi.DAL
{
    [Read, Create, Update, Delete]
    [Display("Товар", DisplayableField = nameof(Code))]
    public class Product : EntityBase
    {
        [Created]
        [Display("Код"), Required]
        public string Code { get; set; }

        [Created, Updated, Patched]
        [Display("Название")]
        public string? Name { get; set; }

        [Created, Updated, Patched]
        [Display("Цена")]
        [Range(Min = 0, Max = 1_000_000), Required]
        public decimal Price { get; set; }
    }
}
