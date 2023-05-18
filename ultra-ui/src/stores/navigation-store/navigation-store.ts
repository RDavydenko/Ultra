import { makeAutoObservable, reaction } from "mobx";
import { NavigateProps, To } from "react-router-dom";
import { AppStore } from "../app-store";

interface NavigationProps {
  to: To;
  params?: Omit<NavigateProps, "to">;
}

export class NavigationStore {
  props?: NavigationProps;
  delta?: number;

  path = "";

  constructor(appStore: AppStore) {
    makeAutoObservable(this);

    reaction(
      () => this.path,
      () => appStore.setActivePage(this.path)
    );
  }

  setPathInternal = (path: string) => (this.path = path);

  to = (to: To, params?: Omit<NavigateProps, "to">) => {
    this.reset();
    this.props = { to, params };
  };

  back = (delta: number) => {
    this.reset();
    this.delta = -delta;
  };

  private reset = () => {
    this.delta = undefined;
    this.props = undefined;
  };
}
