import { GraphQLHandlers } from "src/models";

export const defaultPagination = {
  current: 1,
  pageSize: 10,
  total: 0,
};

export const defaultGraphQLHandlers: GraphQLHandlers = {
  paginationHandler: defaultPagination,
};
