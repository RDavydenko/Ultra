import { isDefined } from "src/utils";

const cacheKeyFunc = (sysName: string) => `visible_columns_${sysName}`;

export const saveVisibleFields = (sysName: string, fields: string[]) => {
  localStorage.setItem(cacheKeyFunc(sysName), JSON.stringify(fields));
};

export const restoreVisibleFields = (sysName: string): string[] | undefined => {
  const key = cacheKeyFunc(sysName);
  const value = localStorage.getItem(key);
  if (!isDefined(value)) {
    return undefined;
  }

  try {
    const fields = JSON.parse(value!);
    if (
      !Array.isArray(fields) ||
      !fields.every((field) => typeof field === "string")
    ) {
      throw new Error("Invalid value");
    }
    return fields;
  } catch {
    localStorage.removeItem(key);
    return undefined;
  }
};
