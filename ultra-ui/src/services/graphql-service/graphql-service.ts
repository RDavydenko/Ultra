import gqlRequest, { RequestDocument } from "graphql-request";
import { AppConfig } from "src/models";
import {
  GraphQLCollectionResponse,
  GraphQLHandlers,
} from "src/models/graphql/graphql";
import {
  buildOrderString,
  buildWhereString,
  getAccessToken,
  isDefined,
} from "src/utils";
import { HttpResponse } from "../interfaces";
import { GraphqlService, RequestParams } from "./graphql-service.types";

export class GraphqlHttpService implements GraphqlService {
  constructor(private readonly config: AppConfig) {}

  request = async <T>(
    params: RequestParams
  ): Promise<HttpResponse<GraphQLCollectionResponse<T>>> => {
    try {
      const response = await gqlRequest({
        url: this.config.graphqlUrl,
        document: params.query,
        variables: params.variables,
        requestHeaders: {
          Authorization: `Bearer ${getAccessToken()}`,
        },
      });

      return {
        isSuccess: true,
        statusCode: 200,
        object: response[
          Object.keys(response)[0]
        ] as GraphQLCollectionResponse<T>,
      };
    } catch (ex) {
      return {
        isSuccess: false,
        statusCode: 500,
        object: null,
        error: ex as string,
      };
    }
  };

  requestInternal = <T>(
    query: RequestDocument,
    handlers?: GraphQLHandlers
  ): Promise<HttpResponse<GraphQLCollectionResponse<T>>> => {
    const customHandlers: any = {};
    handlers?.customHandlers?.forEach(
      (handler) => (customHandlers[handler.key] = handler.value)
    );

    return this.request({
      query,
      variables: {
        where: buildWhereString(handlers?.filterHandlers),
        order: buildOrderString(handlers?.sortHandlers),
        skip: isDefined(handlers?.paginationHandler)
          ? (handlers!.paginationHandler.current - 1) *
            handlers!.paginationHandler.pageSize
          : undefined,
        take: isDefined(handlers?.paginationHandler)
          ? handlers!.paginationHandler.pageSize
          : undefined,
        ...customHandlers,
      },
    });
  };
}
