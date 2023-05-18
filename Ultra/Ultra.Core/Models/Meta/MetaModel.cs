namespace Ultra.Core.Models.Meta
{
    public class MetaModel
    {
        public string Key { get; }
        public object Value { get; }

        public MetaModel(string key, object value)
        {
            Key = key;
            Value = value;
        }
    }
}
