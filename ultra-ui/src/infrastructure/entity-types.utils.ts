import { FieldType } from "src/models";

const integerTypes: string[] = [
  FieldType.Int8,
  FieldType.UInt8,
  FieldType.Int16,
  FieldType.UInt16,
  FieldType.Int32,
  FieldType.UInt32,
  FieldType.Int64,
  FieldType.UInt64,
];

const decimalTypes: string[] = [
  FieldType.Decimal,
  FieldType.Float,
  FieldType.Double,
];

export const isDecimalType = (type: string) => {
  return decimalTypes.includes(type);
};

export const isIntegerType = (type: string) => {
  return integerTypes.includes(type);
};
