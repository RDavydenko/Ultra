// eslint-disable-next-line eqeqeq
export const isDefined = <T>(val: T | undefined): val is T => val != undefined;

export const isPromise = (func: any) =>
  typeof func === "object" && typeof func.then === "function";

export const delay = (ms: number) => new Promise((r) => setTimeout(r, ms));
