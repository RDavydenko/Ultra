using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Ultra.Core.Entities.Interfaces;

namespace Ultra.Core.Entities.Abstract;

public abstract class GuidEntityBase : IGuidEntity
{
    [JsonInclude]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Attributes.Display("Идентификатор")]
    [Attributes.Meta.Disabled]
    public Guid Guid { get; protected set; }
}
