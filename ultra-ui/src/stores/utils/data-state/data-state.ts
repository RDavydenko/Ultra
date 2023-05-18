import { makeAutoObservable, runInAction } from "mobx";
import { isEmptyObject } from "src/utils";

export class DataState<TValue> {
  loading: boolean = false;
  loaded: boolean = false;
  value: TValue = {} as any;

  constructor() {
    makeAutoObservable(this);
  }

  reset() {
    this.loading = false;
    this.loaded = false;
    this.value = {} as any;
  }

  set(value: TValue) {
    this.loading = false;
    this.loaded = true;
    this.value = value;
  }

  get hasValue() {
    return !isEmptyObject(this.value);
  }
}
