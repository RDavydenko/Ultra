import { runInAction } from "mobx";
import { DataState } from "src/stores/utils/data-state/data-state";
import { GraphqlService, HttpResponse } from "src/services";
import { isDefined, isPromise } from "./functions";
import {
  GraphQLCollectionResponse,
  GraphQLHandlers,
} from "src/models/graphql/graphql";
import { CollectionState } from "src/stores";

export type Func<TOut> = () => TOut;
export type Func1<TIn, TOut> = (param: TIn) => TOut;
export type Action0 = () => void;
export type Action1<TIn> =
  | ((value: TIn) => void)
  | ((value: TIn) => Promise<void>);

const resolve = async <TOut>(func: Func<TOut> | Func<Promise<TOut>>) => {
  const res = func();
  if (isPromise(res)) {
    return await res;
  }
  return res;
};

interface InternalPerformActionProps<TResult> {
  condition?: Func<boolean> | Func<Promise<boolean>>;
  action: Func<TResult> | Func<Promise<TResult>>;
  setLoading?: (loading: boolean) => void;
  isSuccess?: Func1<TResult, boolean>;
  onSuccess?: Action1<TResult>;
  onError?: Action1<TResult | unknown>;
}

const internalPerformAction =
  <T>({
    condition,
    action,
    setLoading,
    isSuccess,
    onSuccess,
    onError,
  }: InternalPerformActionProps<T>) =>
  async () => {
    if (condition && (await resolve(condition)) === false) {
      return;
    }

    try {
      runInAction(() => setLoading?.(true));

      const res = await resolve(action);
      if (isSuccess) {
        if (isSuccess(res)) {
          onSuccess &&
            runInAction(() => {
              onSuccess(res);
            });
        } else {
          onError && runInAction(() => onError(res));
        }
      } else {
        onSuccess &&
          runInAction(() => {
            onSuccess(res);
          });
      }
    } catch (ex) {
      onError && runInAction(() => onError(ex));
    } finally {
      runInAction(() => setLoading?.(false));
    }
  };

export interface FetchDataStateParams<TValue> {
  condition?: Func<boolean> | Func<Promise<boolean>>;
  getDataState: Func<DataState<TValue>>;
  getData: Func<HttpResponse<TValue>> | Func<Promise<HttpResponse<TValue>>>;
  onSuccess?: Action1<TValue>;
  onError?: Action1<unknown>;
}

export const fetchDataState = <T>({
  condition,
  getDataState,
  getData,
  onSuccess,
  onError,
}: FetchDataStateParams<T>) =>
  internalPerformAction<HttpResponse<T>>({
    condition,
    setLoading: (loading) => (getDataState().loading = loading),
    action: getData,
    isSuccess: (response) => response.isSuccess,
    onSuccess: (response) => {
      getDataState().value = response.object!;
      getDataState().loaded = true;
      onSuccess && onSuccess(response.object!);
    },
    onError: onError,
  });

export interface PerformActionProps<TValue> {
  action: Func<HttpResponse<TValue>> | Func<Promise<HttpResponse<TValue>>>;
  condition?: Func<boolean> | Func<Promise<boolean>>;
  setLoading?: (loading: boolean) => void;
  onSuccess?: Action1<HttpResponse<TValue>>;
  onError?: Action1<HttpResponse<TValue> | unknown>;
}

export const performAction = <T>({
  action,
  condition,
  setLoading,
  onSuccess,
  onError,
}: PerformActionProps<T>) =>
  internalPerformAction<HttpResponse<T>>({
    condition,
    setLoading: setLoading,
    action: action,
    isSuccess: (response) => response.isSuccess,
    onSuccess: onSuccess,
    onError: onError,
  });

export interface FetchCollectionStateParams<TValue> {
  condition?: Func<boolean> | Func<Promise<boolean>>;
  getCollectionState: Func<CollectionState<TValue>>;
  fetchCollection:
    | Func<HttpResponse<TValue[]>>
    | Func<Promise<HttpResponse<TValue[]>>>;
  onSuccess?: Action1<HttpResponse<TValue[]>>;
  onError?: Action1<HttpResponse<TValue[]> | unknown>;
}

export const fetchCollectionState = <T>({
  condition,
  getCollectionState,
  fetchCollection,
  onSuccess,
  onError,
}: FetchCollectionStateParams<T>) =>
  internalPerformAction<HttpResponse<T[]>>({
    condition,
    setLoading: (loading) => (getCollectionState().loading = loading),
    action: fetchCollection,
    isSuccess: (response) => response.isSuccess,
    onSuccess: (response) => {
      getCollectionState().items = response.object!;
      getCollectionState().totalCount = response.object!.length;
      onSuccess && onSuccess(response);
    },
    onError: onError,
  });

export interface FetchGraphQLCollectionStateParams<T> {
  graphqlService: GraphqlService;
  query: string;
  getCollectionState: Func<CollectionState<T>>;
  handlers?: GraphQLHandlers;
  condition?: Func<boolean> | Func<Promise<boolean>>;
  onSuccess?: Action1<HttpResponse<GraphQLCollectionResponse<T>>>;
  onError?: Action1<HttpResponse<GraphQLCollectionResponse<T>> | unknown>;
}

export const fetchGraphQLCollectionState = <T>({
  graphqlService,
  query,
  getCollectionState,
  handlers,
  condition,
  onSuccess,
  onError,
}: FetchGraphQLCollectionStateParams<T>) =>
  internalPerformAction<HttpResponse<GraphQLCollectionResponse<T>>>({
    condition,
    setLoading: (loading) => (getCollectionState().loading = loading),
    action: () => graphqlService.requestInternal<T>(query, handlers),
    isSuccess: (response) => response.isSuccess,
    onSuccess: (response) => {
      getCollectionState().items = response.object!.items;
      getCollectionState().totalCount = response.object!.totalCount;
      if (isDefined(handlers?.paginationHandler)) {
        handlers!.paginationHandler!.total = response.object!.totalCount;
      }
      onSuccess && onSuccess(response);
    },
    onError: onError,
  });

export interface FetchGraphQLDataStateParams<T> {
  graphqlService: GraphqlService;
  query: string;
  getDataState: Func<DataState<T>>;
  handlers?: GraphQLHandlers;
  condition?: Func<boolean> | Func<Promise<boolean>>;
  onSuccess?: Action1<T>;
  onError?: Action1<HttpResponse<T> | unknown>;
}

export const fetchGraphQLDataState = <T>({
  graphqlService,
  query,
  getDataState,
  handlers,
  condition,
  onSuccess,
  onError,
}: FetchGraphQLDataStateParams<T>) =>
  internalPerformAction<HttpResponse<GraphQLCollectionResponse<T>>>({
    condition,
    setLoading: (loading) => (getDataState().loading = loading),
    action: () => graphqlService.requestInternal<T>(query, handlers),
    isSuccess: (response) =>
      response.isSuccess && response.object?.items?.length === 1,
    onSuccess: (response) => {
      getDataState().value = response.object!.items![0];
      getDataState().loaded = true;
      onSuccess && onSuccess(response.object!.items![0]);
    },
    onError: onError,
  });

export interface FetchGraphQLParams {
  graphqlService: GraphqlService;
  query: string;
  handlers?: GraphQLHandlers;
  onError?: Action1<any>;
}

export const fetchGraphQLEntities = async ({
  graphqlService,
  query,
  handlers,
  onError,
}: FetchGraphQLParams) => {
  try {
    return (
      (await graphqlService.requestInternal<any>(query, handlers)).object
        ?.items ?? []
    );
  } catch (error) {
    onError?.(error);
    return [];
  }
};

export const fetchGraphQL = async <T>({
  graphqlService,
  query,
  handlers,
  onError,
}: FetchGraphQLParams): Promise<GraphQLCollectionResponse<T> | undefined> => {
  try {
    return (await graphqlService.requestInternal<T>(query, handlers)).object!;
  } catch (error) {
    onError?.(error);
    return undefined;
  }
};
