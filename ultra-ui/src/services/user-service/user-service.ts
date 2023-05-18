import urljoin from "url-join";
import UserService from "./user-service.types";
import {
  AppConfig,
  AccessTokenResponse,
  LoginModel,
  CollectionPage,
  UserShortModel,
} from "src/models";
import qs from "qs";
import axios from "axios";
import { HttpResponse } from "../interfaces";
import { authorizedRequest } from "../httpService";

export class UserHttpService implements UserService {
  constructor(private appConfig: AppConfig) {}

  login = async (model: LoginModel) => {
    const response = await axios({
      method: "POST",
      url: urljoin(this.appConfig.authApiUrl, "connect/token"),
      data: qs.stringify({
        grant_type: "password",
        client_id: "ultra_ui_client",
        username: model.username,
        password: model.password,
      }),
      headers: {
        "content-type": "application/x-www-form-urlencoded;charset=utf-8",
      },
    });

    if (200 <= response.status && response.status < 300) {
      return {
        isSuccess: true,
        statusCode: response.status,
        object: response.data as AccessTokenResponse,
      } as HttpResponse<AccessTokenResponse>;
    } else {
      return {
        isSuccess: false,
        message: response.data?.message || "Произошла непредвиденная ошибка",
        statusCode: response.status,
        object: null,
      } as HttpResponse<AccessTokenResponse>;
    }
  };

  getUserName = (userId: number) =>
    authorizedRequest<string>({
      method: "GET",
      url: urljoin(
        this.appConfig.authApiUrl,
        "users",
        userId.toString(),
        "userName"
      ),
    });

  getUsers = (q?: string, id?: number) =>
    authorizedRequest<CollectionPage<UserShortModel>>({
      method: "GET",
      params: {
        q,
        id,
      },
      url: urljoin(this.appConfig.authApiUrl, "users", `search`),
    });
}
