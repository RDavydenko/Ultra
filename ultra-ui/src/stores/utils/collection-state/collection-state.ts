import { makeAutoObservable, runInAction } from "mobx";

export class CollectionState<TValue> {
  loading: boolean = false;
  items: TValue[] = [];
  totalCount: number = 0;

  constructor() {
    makeAutoObservable(this);
  }

  reset = () => {
    runInAction(() => {
      this.items = [];
      this.totalCount = 0;
      this.loading = false;
    });
  };
}
