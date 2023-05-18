import urljoin from "url-join";
import { AppConfig } from "src/models";
import { authorizedRequest } from "../httpService";
import AuthService from "./auth-service.types";

export class AuthHttpService implements AuthService {
  constructor(private appConfig: AppConfig) {}

  checkAuth = async () => {
    const result = await authorizedRequest<any>({
      method: "GET",
      url: urljoin(this.appConfig.authApiUrl, "Auth"),
    });

    return result.statusCode === 200;
  };
}
