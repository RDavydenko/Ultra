export const firstLetterToUpper = (s: string) => {
  if (!s || !s.length) return s;

  return s.charAt(0).toUpperCase() + s.slice(1);
};

export const firstLetterToLower = (s: string) => {
  if (!s || !s.length) return s;

  return s.charAt(0).toLowerCase() + s.slice(1);
};
