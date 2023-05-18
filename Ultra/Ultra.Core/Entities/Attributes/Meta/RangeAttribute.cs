using System;
using System.Collections.Generic;
using Ultra.Core.Entities.Attributes.Meta.Abstract;
using Ultra.Core.Models.Meta;

namespace Ultra.Core.Entities.Attributes.Meta
{
    public class RangeAttribute : MetaAttributeBase
    {
        private bool _hasMin;
        private bool _hasMax;

        private int _min;
        private int _max;

        public int Min { get => _min; set => (_min, _hasMin) = (value, true); }
        public int Max { get => _max; set => (_max, _hasMax) = (value, true); }

        public override IEnumerable<MetaModel> GetMeta()
        {
            if ((_hasMin && _hasMax) && (Min > Max))
                throw new InvalidOperationException();

            if (_hasMin)
                yield return new MetaModel("range.min", Min);

            if (_hasMax)
                yield return new MetaModel("range.max", Max);
        }
    }
}
