export const extractRequiredMeta = <TValue>(path: string, meta: Record<string, any> | undefined): TValue => {
  const value = extractMeta<TValue>(path, meta);
  if (value === undefined) {
    const errorMessage = `Отсутствует обязательный параметр метаданных ${path}`;
    // console.error(errorMessage);
    throw Error(errorMessage);
  }
  return value;
}

export const extractMeta = <TValue>(path: string, meta: Record<string, any> | undefined): TValue | undefined => {
  const paths = path.split(".");
  let value: any = undefined;
  if (meta && paths.length) {
    value = meta[paths[0]];
  }

  for (let i = 1; i < paths.length; i++) {
    if (!value) {
      break;
    }
    value = value[paths[i]];
  }

  return value;
}