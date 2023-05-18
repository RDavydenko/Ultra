export const removeAt = <T>(array: T[], index: number) => {
  return [...array.slice(0, index), ...array.slice(index + 1, array.length)];
};

export const replaceAt = <T>(array: T[], index: number, elem: T) => {
  return [
    ...array.slice(0, index),
    elem,
    ...array.slice(index + 1, array.length),
  ];
};
