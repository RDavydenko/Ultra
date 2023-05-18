export interface GraphQLPageInfo {
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface GraphQLCollectionResponse<T> {
  items: T[];
  pageInfo: GraphQLPageInfo;
  totalCount: number;
}

export enum GraphQLOperator {
  Equal = "eq",
  NotEqual = "neq",
  Greater = "gt",
  NotGreater = "ngt",
  GreaterOrEqual = "gte",
  NotGreaterOrEqual = "ngte",
  Less = "lt",
  NotLess = "nlt",
  LessOrEqual = "lte",
  NotLessOrEqual = "nlte",
  In = "in",
  NotIn = "nin",
  Contains = "contains",
  NotContains = "ncontains",
  StartsWith = "startsWith",
  NotStartsWith = "nstartsWith",
  EndsWith = "endsWith",
  NotEndsWith = "nendsWith",
}

export interface GraphQLPaginationHandler {
  current: number;
  pageSize: number;
  total: number;
}

export interface GraphQLFilterHandler {
  key: string;
  operator?: GraphQLOperator | string;
  expression: any;
}

export interface GraphQLSortHandler {
  key: string;
  direction: "ASC" | "DESC";
}

export interface GraphQLCustomHandler {
  key: string;
  value: any;
}

export interface GraphQLHandlers {
  sortHandlers?: GraphQLSortHandler[];
  filterHandlers?: GraphQLFilterHandler[];
  paginationHandler?: GraphQLPaginationHandler;
  customHandlers?: GraphQLCustomHandler[];
  nonConvertibleVariables?: any;
}
