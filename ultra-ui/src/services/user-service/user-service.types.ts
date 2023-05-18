import { CollectionPage, LoginModel, UserShortModel } from "src/models";
import { AccessTokenResponse } from "src/models/user/AccessTokenResponse";
import { HttpResponse } from "../interfaces";

export default interface UserService {
  login: (model: LoginModel) => Promise<HttpResponse<AccessTokenResponse>>;

  getUserName: (userId: number) => Promise<HttpResponse<string>>;

  getUsers: (
    q?: string,
    id?: number
  ) => Promise<HttpResponse<CollectionPage<UserShortModel>>>;
}
