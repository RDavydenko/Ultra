import {
  FilterValue,
  SorterResult,
  TablePaginationConfig,
} from "antd/lib/table/interface";
import {
  GraphQLFilterHandler,
  GraphQLHandlers,
  GraphQLOperator,
} from "src/models";
import { isDefined } from "./functions";
import { firstLetterToLower } from "./strings";

const mapAntdSortDirection = (direction: string) => {
  if (direction === "ascend") return "ASC";

  return "DESC";
};

export const buildGraphQLHandlers = (
  filters: Record<string, FilterValue | null>,
  sorter: SorterResult<any> | SorterResult<any>[],
  pagination: TablePaginationConfig
): GraphQLHandlers => {
  const sort: SorterResult<any> = Array.isArray(sorter) ? sorter[0] : sorter;
  const filteredKeys = Object.keys(filters).filter(
    (key) =>
      isDefined(filters[key]) &&
      Array.isArray(filters[key]) &&
      filters[key]!.length > 0
  );
  const orExpressions: any[] = [];
  for (const key of filteredKeys) {
    const values = filters[key]!.map((value) => ({
      [firstLetterToLower(key)]: { [GraphQLOperator.Equal]: value },
    }));
    orExpressions.push({ or: values });
  }

  return {
    filterHandlers:
      orExpressions.length > 0
        ? [
            {
              key: "and",
              expression: orExpressions,
            },
          ]
        : undefined,
    sortHandlers: isDefined(sort?.order)
      ? [
          {
            key: sort.columnKey!.toString(),
            direction: mapAntdSortDirection(sort.order!),
          },
        ]
      : undefined,
    paginationHandler: {
      current: pagination.current!,
      pageSize: pagination.pageSize!,
      total: pagination.total!,
    },
  };
};
