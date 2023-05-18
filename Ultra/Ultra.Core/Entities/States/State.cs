using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ultra.Core.Entities.States
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum State
    {
        /// <summary>
        /// Активный
        /// </summary>
        ACTIVE,

        /// <summary>
        /// Черновик
        /// </summary>
        DRAFT,

        /// <summary>
        /// Удаленный
        /// </summary>
        DELETED
    }
}
