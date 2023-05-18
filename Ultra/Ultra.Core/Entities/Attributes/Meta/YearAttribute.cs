namespace Ultra.Core.Entities.Attributes.Meta
{
    public class YearAttribute : RangeAttribute
    {
        public YearAttribute()
        {
            Min = 0;
            Max = 9999;
        }
    }
}
