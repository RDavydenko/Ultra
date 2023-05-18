import { makeAutoObservable } from "mobx";
import { AppPage, AppPageModel } from "./app-store.types";

export class AppStore {
  currentPage: AppPageModel = {
    page: AppPage.Home,
    path: "",
  };

  constructor() {
    makeAutoObservable(this);
  }

  setActivePage = (path: string) => {
    let p = path.slice(1);

    if (p === "") {
      this.currentPage = {
        page: AppPage.Home,
        path: p,
      };
    } else {
      const index = p.indexOf("/");
      if (index !== -1) p = p.slice(0, index);
      this.currentPage = {
        page: p,
        path: path,
      };
    }
  };
}
