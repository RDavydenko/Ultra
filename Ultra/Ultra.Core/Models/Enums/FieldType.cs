using System.Text.Json.Serialization;

namespace Ultra.Core.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FieldType
    {
        Undefined = 0,

        // .NET types
        Boolean, // bool    
        Int8, // sbyte      
        UInt8, // byte      
        Int16, // short     
        UInt16, // ushort   
        Int32, // int       
        UInt32, // uint     
        Int64, // long      
        UInt64, // ulong    
        Decimal, // decimal 
        Float, // float     
        Double, // double   
        Char, // char       
        String, // string,  

        // dates (by .NET)    
        DateTime, // DateTime,
        Date, // DateTime,    
        Time, // DateTime,    
        Interval, // TimeSpan,

        // Custom with dates
        DateTimePeriod, // DateTime + DateTime, 
        DatePeriod, // DateTime + DateTime,     
        TimePeriod, // DateTime + DateTime,     

        // References
        ReferenceParent,
        ReferenceChild,
        ReferenceChildren,

        // Custom
        Location, // Location           
        UserId, // UserId (with search) 
        File, // File                   
    }
}
