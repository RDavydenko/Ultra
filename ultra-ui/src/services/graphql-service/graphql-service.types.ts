import { RequestDocument } from "graphql-request";
import {
  GraphQLCollectionResponse,
  GraphQLHandlers,
} from "src/models/graphql/graphql";
import { HttpResponse } from "../interfaces";

export interface RequestParams {
  query: RequestDocument;
  variables?: any;
}

export interface GraphqlService {
  request: <T>(
    params: RequestParams
  ) => Promise<HttpResponse<GraphQLCollectionResponse<T>>>;

  requestInternal: <T>(
    query: RequestDocument,
    handlers?: GraphQLHandlers
  ) => Promise<HttpResponse<GraphQLCollectionResponse<T>>>;
}
