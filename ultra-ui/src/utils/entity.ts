import { FieldConfiguration } from "src/models";

export const isForeignKeyField = (config: FieldConfiguration) =>
  config.meta?.["foreignKey"] === true;
