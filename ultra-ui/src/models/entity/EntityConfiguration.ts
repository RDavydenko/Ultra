export enum FieldMethods {
  Created = "Created",
  Updated = "Updated",
  Patched = "Patched",
}

export enum EntityMethods {
  Read = "Read",
  Create = "Create",
  Update = "Update",
  Patch = "Patch",
  Delete = "Delete",
}

export interface FieldConfiguration {
  systemName: string;
  displayName?: string;
  type: string;
  meta: Record<string, any>;
  methods: FieldMethods[];
}

export interface EntityConfiguration {
  systemName: string;
  displayName?: string;
  methods: EntityMethods[];
  fields: FieldConfiguration[];
  meta: Record<string, any>;
}

export enum FieldType {
  // .NET types
  Boolean = "Boolean", // bool         -
  Int8 = "Int8", // sbyte        +
  UInt8 = "UInt8", // byte         +
  Int16 = "Int16", // short        +
  UInt16 = "UInt16", // ushort       +
  Int32 = "Int32", // int          +
  UInt32 = "UInt32", // uint         +
  Int64 = "Int64", // long         +
  UInt64 = "UInt64", // ulong        +
  Decimal = "Decimal", // decimal      +
  Float = "Float", // float        +
  Double = "Double", // double       +
  Char = "Char", // char         -
  String = "String", // string,      +

  // dates (by .NET)
  DateTime = "DateTime", // DateTime,    +
  Date = "Date", // DateTime,    +
  Time = "Time", // DateTime,    +
  Interval = "Interval", // TimeSpan,    -

  // Custom with dates
  DateTimePeriod = "DateTimePeriod", // DateTime + DateTime,     +
  DatePeriod = "DatePeriod", // DateTime + DateTime,     +
  TimePeriod = "TimePeriod", // DateTime + DateTime,     +

  // References
  ReferenceParent = "ReferenceParent",
  ReferenceChild = "ReferenceChild",
  ReferenceChildren = "ReferenceChildren",

  // Custom
  Text = "Text", // string (TextArea)
  Location = "Location", // Location                 -
  UserId = "UserId", // UserId (with search)     -
  File = "File", // File                     -

  Undefined = "Undefined",
}
