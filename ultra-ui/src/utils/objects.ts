import { isDefined } from "./functions";
import { firstLetterToLower, firstLetterToUpper } from "./strings";

export const objToPascalCase = (obj: any) => {
  if (typeof obj === "object") {
    const newObj: any = {};
    for (const key in obj) {
      newObj[firstLetterToUpper(key)] = obj[key];
    }
    return newObj;
  }
  return obj;
};

export const objToCamelCase = (obj: any) => {
  if (!isDefined(obj)) {
    return undefined;
  }
  if (Array.isArray(obj)) {
    const newArr: any[] = [];
    for (let i = 0; i < obj.length; i++) {
      newArr[i] = objToCamelCase(obj[i]);
    }
    return newArr;
  } else if (typeof obj === "object") {
    const newObj: any = {};
    for (const key in obj) {
      newObj[firstLetterToLower(key)] = objToCamelCase(obj[key]);
    }
    return newObj;
  }
  return obj;
};

export const removeNullableFields = (obj: any) => {
  if (typeof obj === "object") {
    console.log("До", obj);
    const newObj = Object.fromEntries(
      Object.entries(obj).filter(([_, v]) => v != null)
    );
    console.log("После", newObj);
    return newObj;
  }
  return obj;
};

export const isEmptyObject = (obj: any) => {
  for (const _ in obj) {
    return false;
  }
  return true;
};
