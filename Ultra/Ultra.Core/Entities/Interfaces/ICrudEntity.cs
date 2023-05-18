using System;

namespace Ultra.Core.Entities.Interfaces;

public interface ICrudEntity : IEntity
{
    int CreateUserId { get; set; }
    DateTime CreateDate { get; set; }
    int? UpdateUserId { get; set; }
    DateTime? UpdateDate { get; set; }
    int? DeleteUserId { get; set; }
    DateTime? DeleteDate { get; set; }
}
