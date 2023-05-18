using System;
using Ultra.Core.Entities.Attributes;
using Ultra.Core.Entities.Interfaces;

namespace Ultra.Core.Entities.Abstract;

public abstract class CrudEntityBase : EntityBase, ICrudEntity
{
    [Display("Создавший пользователь"), UserId]
    public int CreateUserId { get; set; }

    [Display("Дата создания")]
    public DateTime CreateDate { get; set; }

    [Display("Обновивший пользователь"), UserId]
    public int? UpdateUserId { get; set; }

    [Display("Дата обновления")]
    public DateTime? UpdateDate { get; set; }

    [Display("Удаливший пользователь"), UserId]
    public int? DeleteUserId { get; set; }

    [Display("Дата удаления")]
    public DateTime? DeleteDate { get; set; }

    public void MarkCreated(int userId) => (CreateUserId, CreateDate) = (userId, DateTime.Now);
    public void MarkUpdated(int userId) => (UpdateUserId, UpdateDate) = (userId, DateTime.Now);
    public void MarkDeleted(int userId) => (DeleteUserId, DeleteDate) = (userId, DateTime.Now);
}
