export interface AppPageModel {
  page: AppPage | string;
  path: string;
}

export enum AppPage {
  Home = "",
  Login = "login",
  Data = "d",
  Chat = "chat",
  Map = "map",
  Users = "users",
  Files = "files",
  Favorites = "favorites",
  Settings = "settings",
}
