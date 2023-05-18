import { makeAutoObservable, reaction } from "mobx";
import {
  AppConfig,
  CollectionPage,
  EntityConfiguration,
  EntityMethods,
  EntityType,
  GraphQLHandlers,
  GraphQLOperator,
  PageModel,
} from "src/models";
import { DataState } from "src/stores/utils/data-state/data-state";
import CrudService from "src/services/crud-service/crud-service.types";
import EntityService from "src/services/entities-service/entities-service.types";
import {
  buildQuery,
  buildSimpleEntitiesQuery,
  fetchDataState,
  fetchGraphQL,
  fetchGraphQLCollectionState,
  fetchGraphQLEntities,
  isDefined,
  performAction,
} from "src/utils";
import { NotificationStore } from "../notification-store";
import { CollectionState } from "../utils";
import { CrudHttpService, GraphqlService } from "src/services";
import { defaultGraphQLHandlers } from "src/utils/tables";
import UserService from "src/services/user-service/user-service.types";

export class CollectionStore {
  constructor(
    private readonly notificationStore: NotificationStore,
    private readonly graphqlService: GraphqlService,
    private readonly userService: UserService
  ) {
    makeAutoObservable(this);
  }

  fetchEntities = (
    sysName: string,
    displayNamePath?: string,
    q?: string,
    excludedIds?: number[]
  ) => {
    displayNamePath ??= "Id";
    const filters = [];
    if (isDefined(q)) {
      filters.push({
        key: displayNamePath,
        operator: GraphQLOperator.Contains,
        expression: q,
      });
    }
    if (isDefined(excludedIds)) {
      filters.push({
        key: "id",
        operator: GraphQLOperator.NotIn,
        expression: excludedIds,
      });
    }

    return fetchGraphQLEntities({
      graphqlService: this.graphqlService,
      query: buildSimpleEntitiesQuery(sysName, displayNamePath),
      handlers: {
        filterHandlers: filters,
      },
      onError: (err) => {
        this.notificationStore.error("Произошла ошибка при загрузке объектов");
        console.log(err);
      },
    });
  };

  fetchUsers = async (model: { q?: string; id?: number }) => {
    const errorMessage = "Произошла ошибка при загрузке списка пользователей";
    try {
      const res = await this.userService.getUsers(model.q, model.id);
      if (res.isSuccess) {
        return res.object!.items;
      }
      this.notificationStore.error(res.error || errorMessage);
      return [];
    } catch (error) {
      this.notificationStore.error(errorMessage);
      console.log(error);
      return [];
    }
  };

  fetchValuesForSearch = async (
    entitySysName: string,
    fieldSysName: string,
    model?: { q?: string; page?: 1; pageSize?: 20 }
  ) => {
    const filters = [];
    if (isDefined(model?.q)) {
      filters.push({
        key: fieldSysName,
        operator: GraphQLOperator.Contains,
        expression: model!.q,
      });
    }

    const response = await fetchGraphQL({
      graphqlService: this.graphqlService,
      query: buildQuery(entitySysName, [fieldSysName], {
        // where: true,
        // skip: true,
        // take: true,
        custom: [{ name: "distinct", type: "Boolean" }],
      }),
      handlers: {
        filterHandlers: filters,
        customHandlers: [
          {
            key: "distinct",
            value: true,
          },
        ],
      },
    });

    return [response!.items, response!.totalCount];
  };
}
