import {
  EntityConfiguration,
  GeoEntityType,
  FieldConfiguration,
  FieldType,
  GraphQLFilterHandler,
  GraphQLHandlers,
  GraphQLOperator,
  GraphQLSortHandler,
  Boundaries,
} from "src/models";
import { isDefined } from "./functions";
import { firstLetterToLower } from "./strings";

export const buildEntityQueryTemplate = (
  config: EntityConfiguration,
  queryFieldFactory: (field: FieldConfiguration) => string
) => {
  const sysName = config.systemName;
  let fields = "";
  for (const field of config.fields) {
    fields += queryFieldFactory(field);
  }

  return `
    query Get${sysName}(
      $skip: Int
      $take: Int
      $where: ${sysName}FilterInput
      $order: [${sysName}SortInput!]
    ) {
      ${firstLetterToLower(sysName)}(
        skip: $skip
        take: $take
        where: $where
        order: $order
      ) {
        items {
          ${fields}
        }
        pageInfo {
          hasNextPage
          hasPreviousPage
        }
        totalCount
      }
    }
  `;
};

export const buildSimpleEntitiesQuery = (
  sysName: string,
  displayNamePath: string
) => {
  return `
query Get${sysName}(
  $where: ${sysName}FilterInput
) {
  ${firstLetterToLower(sysName)}(
    skip: 0
    take: 20
    where: $where
  ) {
    items {
      Id: id
      DisplayName: ${firstLetterToLower(displayNamePath)}
    }
    pageInfo {
      hasNextPage
      hasPreviousPage
    }
    totalCount
  }
}
`;
};

export const buildQuery = (
  entitySysName: string,
  fields: string[],
  args?: {
    where?: boolean;
    order?: boolean;
    skip?: boolean;
    take?: boolean;
    custom?: { name: string; type: string }[];
  }
) => {
  return `
query Get${entitySysName}(
  ${args?.skip ? `$skip: Int` : ""}
  ${args?.take ? `$take: Int` : ""}
  ${args?.where ? `$where: ${entitySysName}FilterInput` : ""}
  ${args?.order ? `$order: [${entitySysName}SortInput!]` : ""}
  ${
    args?.custom
      ? args.custom.map((x) => `$${x.name}: ${x.type}`).join("\n")
      : ""
  }
) {
  ${firstLetterToLower(entitySysName)}(
    ${args?.skip ? "skip: $skip" : ""}
    ${args?.take ? "take: $take" : ""}
    ${args?.where ? "where: $where" : ""}
    ${args?.order ? "order: $order" : ""}
    ${
      args?.custom
        ? args.custom.map((x) => `${x.name}: $${x.name}`).join("\n")
        : ""
    }
  ) {
    items {
      ${fields.map((x) => `${x}: ${firstLetterToLower(x)}`).join("\n")}
    }
    pageInfo {
      hasNextPage
      hasPreviousPage
    }
    totalCount
  }
}
`;
};

export const buildGeoEntitiesQuery = (
  model: GeoEntityType,
  bounds: Boundaries
) => {
  // TODO: Некорректно работает поиск внутри полигона
  const where = `
    where: { 
      ${firstLetterToLower(model.geoFieldName)}: { 
        intersects: {
          geometry: {
            type: "Polygon",
            coordinates: [
            [
              [${bounds.northEast.lat}, ${bounds.southWest.lng}],
              [${bounds.northEast.lat}, ${bounds.northEast.lng}],
              [${bounds.southWest.lat}, ${bounds.northEast.lng}],
              [${bounds.southWest.lat}, ${bounds.northEast.lng}],
              [${bounds.northEast.lat}, ${bounds.southWest.lng}] 
            ]
          ]
        }
      }
    }
  }`;

  return `
{
  ${firstLetterToLower(model.systemName)} (
    take: 1000
  ) {
    items {
      id
      displayName: ${firstLetterToLower(model.displayableField)}
      location: ${firstLetterToLower(model.geoFieldName)} {
        type
        coordinates
      }
    }
    pageInfo {
      hasNextPage
      hasPreviousPage
    }
    totalCount
  }
}`;
};

export const buildOrderString = (handlers?: GraphQLSortHandler[]) => {
  if (!isDefined(handlers) || handlers.length === 0) {
    return undefined;
  }

  const order = handlers.reduce(
    (acc: any, handler) => (
      (acc[firstLetterToLower(handler.key)] = handler.direction), acc
    ),
    {}
  );

  return order;
};

export const buildWhereString = (handlers?: GraphQLFilterHandler[]) => {
  if (!isDefined(handlers) || !handlers.length) {
    return undefined;
  }

  const obj: any = {};
  for (const handler of handlers) {
    if (isDefined(handler.operator)) {
      obj[firstLetterToLower(handler.key)] = {
        [handler.operator]: handler.expression,
      };
    } else {
      obj[firstLetterToLower(handler.key)] = handler.expression;
    }
  }

  return obj;
};

export const getByIdGraphQLHandlers = (id: number): GraphQLHandlers => {
  return {
    filterHandlers: [
      {
        key: "id",
        operator: GraphQLOperator.Equal,
        expression: id,
      },
    ],
  };
};
