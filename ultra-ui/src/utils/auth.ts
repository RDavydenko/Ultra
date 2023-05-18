const accessTokenItemName = "access_token";
const refreshTokenItemName = "refresh_token";

export const setAccessToken = (token: string) => {
  localStorage.getItem(accessTokenItemName);
  localStorage.setItem(accessTokenItemName, token);
};

export const getAccessToken = (): string | null => {
  return localStorage.getItem(accessTokenItemName);
};

export const clearAccessToken = () => {
  localStorage.removeItem(accessTokenItemName);
};

export const setRefreshToken = (token: string) => {
  localStorage.getItem(refreshTokenItemName);
  localStorage.setItem(refreshTokenItemName, token);
};

export const getRefreshToken = (): string | null => {
  return localStorage.getItem(refreshTokenItemName);
};

export const clearRefreshToken = () => {
  localStorage.removeItem(refreshTokenItemName);
};
