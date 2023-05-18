import axios, { AxiosError, AxiosRequestConfig, AxiosResponse } from "axios";
import jwt_decode from "jwt-decode";
import qs from "qs";
import urljoin from "url-join";

// import { HttpStatus } from "@consts/api";

import { HttpResponse } from "src/services/interfaces";

import {
  clearAccessToken,
  getAccessToken,
  getRefreshToken,
  setAccessToken,
  setRefreshToken,
  //getErrorMessage,
} from "src/utils";
import { routes } from "src/consts";

type TokenData = {
  access_token: string;
  refresh_token: string;
};
export type TokenResponse = AxiosResponse<TokenData>;
type TokenCallback = (response: TokenResponse) => Promise<TokenResponse>;

const getTokenExpiredAt = (): string | undefined => {
  const accessToken = getAccessToken();
  return accessToken != undefined
    ? (jwt_decode(accessToken) as any)?.exp
    : undefined;
};

let refreshTokenPromise: Promise<TokenResponse> | undefined | null = undefined;

export const getRefreshTokenPromise = () => {
  return refreshTokenPromise;
};

export const setRefreshTokenPromise = (
  promise: Promise<TokenResponse> | undefined
) => {
  refreshTokenPromise = promise;
};

export const authorizedRequest = async <T>(
  params: Partial<AxiosRequestConfig>
) => {
  let accessToken = getAccessToken();
  const fullParams: AxiosRequestConfig = {
    ...params,
    headers: {
      ...params.headers,
      Authorization: `Bearer ${accessToken}`,
    },
  };
  try {
    if (isTokenExpired()) {
      if (!refreshTokenPromise) {
        console.log("token expired");
        refreshTokenPromise = refreshTokens();
      }

      if (refreshTokenPromise) {
        await refreshTokenPromise;
        accessToken = getAccessToken();
        refreshTokenPromise = undefined;
        fullParams.headers!.Authorization = `Bearer ${accessToken}`;
      }
    }
    const response = await axios({ ...fullParams });

    if (200 <= response.status && response.status < 300) {
      return response.data as HttpResponse<T>;
    } else {
      return {
        isSuccess: false,
        message: response.data?.message || "Произошла непредвиденная ошибка",
        statusCode: response.status,
        object: null,
      } as HttpResponse<T>;
    }
  } catch (err) {
    // приведение, потому что catch(err) - позволяет быть any/unknown
    const error = err as AxiosError;
    if (error.response && error.response.status === 401) {
      console.log("401 error");
      redirectToAuth();
      throw error;
    } else {
      return {
        isSuccess: false,
        message:
          error.response?.statusText || "Произошла непредвиденная ошибка",
        object: null,
        statusCode: error.response?.status ?? 500,
      } as HttpResponse<T>;
    }
  }
};

export const isTokenExpired = () => {
  return (
    getTokenExpiredAt() === undefined ||
    Date.now() >= parseInt(getTokenExpiredAt()!) * 1000
  );
};

export const refreshTokens = (afterRefreshCallback?: TokenCallback) => {
  // Try to get new access token by refresh token
  const refreshToken = getRefreshToken();

  if (refreshToken) {
    try {
      return axios({
        method: "post",
        url: urljoin(window.identityServerApiUrl + "connect/token"),
        data: qs.stringify({
          grant_type: "refresh_token",
          client_id: "ultra_ui_client",
          refresh_token: refreshToken,
        }),
        headers: {
          "content-type": "application/x-www-form-urlencoded;charset=utf-8",
        },
      }).then((tokenRefreshResponse: TokenResponse) => {
        const newAccessToken = tokenRefreshResponse.data?.access_token;
        setAccessToken(newAccessToken);
        setRefreshToken(tokenRefreshResponse.data?.refresh_token);

        return afterRefreshCallback
          ? afterRefreshCallback(tokenRefreshResponse)
          : Promise.resolve(tokenRefreshResponse);
      });
    } catch (error) {
      redirectToAuth();
    }
  } else {
    redirectToAuth();
  }
  return undefined;
};

export const redirectToAuth = () => {
  console.log("redirectToAuth");
  if ((window as any).redirectToAuth) {
    setTimeout(() => {
      clearAccessToken();
      window.location.href = routes.login.path;
    }, 1000);
  }
};
