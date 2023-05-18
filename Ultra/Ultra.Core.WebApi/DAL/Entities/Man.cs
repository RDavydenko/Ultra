using DelegateDecompiler;
using Microsoft.Linq.Translations;
using System.ComponentModel.DataAnnotations.Schema;
using Ultra.Core.Entities.Abstract;
using Ultra.Core.Entities.Attributes;
using Ultra.Core.Entities.Attributes.Access;
using Ultra.Core.Entities.Attributes.Data.Methods;
using Ultra.Core.Entities.Interfaces;

namespace Ultra.Core.WebApi.DAL.Entities
{
    [Read, Create, Update, Delete]
    [Display("Клиент", DisplayableField = nameof(Name))]
    public class Man : EntityBase
    {
        [Created, Updated, Patched]
        public string? Name { get; set; }

        [Created, Patched]
        public int? Age { get; set; }

        [Display("Назначенный пользователь"), UserId]
        [Created, Updated, Patched]
        public int? AssignedUserId { get; set; }

        [Created, Updated, Patched]
        public virtual ICollection<House> Houses { get; set; }
    }
}
