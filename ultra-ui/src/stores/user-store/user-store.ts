import { makeAutoObservable, runInAction } from "mobx";
import { LoginModel } from "src/models";
import UserService from "src/services/user-service/user-service.types";
import {
  clearAccessToken,
  clearRefreshToken,
  getAccessToken,
  performAction,
  setAccessToken,
  setRefreshToken,
} from "src/utils";
import { NotificationStore } from "../notification-store";
import jwt_decode from "jwt-decode";
import { DataState } from "../utils";
import { UserModel } from "./user-store.types";
import { Role } from "src/consts/role";
import { Action, Entity } from "src/consts/permissions";
import AuthService from "src/services/auth-service/auth-service.types";

export class UserStore {
  public readonly user = new DataState<UserModel>();
  isAuth = false;
  roles: Role[] = [];
  permissions: { entity: Entity; action: Action }[] = [];

  loading = false;

  constructor(
    private readonly notificationStore: NotificationStore,
    private readonly userService: UserService,
    private readonly authService: AuthService
  ) {
    makeAutoObservable(this);
    this.parseToken(getAccessToken());
  }

  login = (model: LoginModel) =>
    performAction({
      action: () => this.userService.login(model),
      setLoading: (loading) => (this.loading = loading),
      onSuccess: (res) => {
        if (res.isSuccess) {
          setAccessToken(res.object!.access_token);
          setRefreshToken(res.object!.refresh_token);
        }
        this.parseToken(res.object!.access_token);
      },
      onError: (err) => {
        this.notificationStore.error("Неверный логин и/или пароль");
        console.log(err);
      },
    })();

  logout = () => {
    runInAction(() => {
      this.user.reset();
      this.roles = [];
      this.permissions = [];
      this.loading = false;
      this.isAuth = false;
      clearAccessToken();
      clearRefreshToken();
    });
  };

  checkAuth = this.authService.checkAuth;

  hasRole = (role: Role) => this.roles.some((r) => r === role);

  hasPermission = (entity: Entity, action: Action) =>
    this.permissions.some((p) => p.entity === entity && p.action === action);

  getUserName = async (userId: number) => {
    try {
      const result = await this.userService.getUserName(userId);
      if (result.isSuccess) {
        return result.object!;
      }
      return userId;
    } catch (error) {
      console.log(error);
      return userId;
    }
  };

  private parseToken = (token: string | null) => {
    if (token === null) {
      return;
    }

    const data = jwt_decode(token) as any;
    runInAction(() => {
      this.user.set({
        id: Number(data.id),
        login: data.name,
        userName: data.given_name,
      });
      this.roles = Array.isArray(data.role) ? data.role : [data.role];
      this.permissions = (
        Array.isArray(data.permission) ? data.permission : [data.permission]
      ).map((x: string) => {
        const s = x.split(",");
        return { entity: s[0], action: s[1] };
      });
      this.isAuth = true;
    });
  };
}
