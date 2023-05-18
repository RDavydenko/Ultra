import { makeAutoObservable, reaction, runInAction } from "mobx";
import {
  AppConfig,
  CollectionPage,
  EntityConfiguration,
  EntityMethods,
  EntityType,
  FieldConfiguration,
  GraphQLHandlers,
  PageModel,
} from "src/models";
import { DataState } from "src/stores/utils/data-state/data-state";
import CrudService from "src/services/crud-service/crud-service.types";
import EntityService from "src/services/entities-service/entities-service.types";
import {
  buildEntityQueryTemplate,
  fetchDataState,
  fetchGraphQLCollectionState,
  fetchGraphQLDataState,
  getByIdGraphQLHandlers,
  isDefined,
  performAction,
} from "src/utils";
import { NotificationStore } from "../notification-store";
import { CollectionState } from "../utils";
import { CrudHttpService, GraphqlService } from "src/services";
import { defaultGraphQLHandlers } from "src/utils/tables";
import { RelatedLink } from "src/models/entity/EntityWithRelatedLinks";
import { restoreVisibleFields, saveVisibleFields } from "./entity-store.utils";

export class EntityStore {
  private crudService?: CrudService;

  readonly config: DataState<EntityConfiguration> =
    new DataState<EntityConfiguration>();
  readonly entity: DataState<any> = new DataState<any>();
  readonly isEntityFavorite: DataState<boolean> = new DataState<boolean>();
  readonly entities: CollectionState<any> = new CollectionState<any>();
  readonly entityTypes: DataState<CollectionPage<EntityType>> = new DataState<
    CollectionPage<EntityType>
  >();

  queryListViewFieldFactory?: (config: FieldConfiguration) => string;
  queryDetailViewFieldFactory?: (config: FieldConfiguration) => string;
  sysName?: string;
  loading = false;
  loaded = false;
  handlers: GraphQLHandlers = defaultGraphQLHandlers;
  configurationLoaded = false;

  detailView = false;
  detailType: EntityMethods.Create | EntityMethods.Update =
    EntityMethods.Create;
  entityId?: number;
  linksToAddOrUpdate: RelatedLink[] = [];
  linksToDelete: RelatedLink[] = [];

  visibleFields?: string[];

  constructor(
    private readonly appConfig: AppConfig,
    private readonly notificationStore: NotificationStore,
    private readonly entityService: EntityService,
    private readonly graphqlService: GraphqlService
  ) {
    makeAutoObservable(this);

    reaction(
      () => this.handlers,
      () => this.fetchEntities()
    );
  }

  openDetail(entityId?: number) {
    this.entity.reset();
    if (isDefined(entityId)) {
      this.entityId = entityId;
      this.fetchEntity();
      this.detailType = EntityMethods.Update;
    } else {
      this.detailType = EntityMethods.Create;
    }

    this.detailView = true;
  }

  setQueryListViewFieldFactory = (
    factory: (config: FieldConfiguration) => string
  ) => {
    this.queryListViewFieldFactory = factory;
  };

  setQueryDetailViewFieldFactory = (
    factory: (config: FieldConfiguration) => string
  ) => {
    this.queryDetailViewFieldFactory = factory;
  };

  setVisibleFields = (fields: string[]) => {
    this.visibleFields = fields;
    saveVisibleFields(this.sysName!, fields);
  };

  reset() {
    this.config.reset();
    this.entity.reset();
    this.entities.reset();
    this.isEntityFavorite.reset();
    this.loading = false;
    this.loaded = false;
    this.configurationLoaded = false;
    this.resetHandlers();
    this.visibleFields = undefined;
  }

  resetHandlers() {
    this.handlers = defaultGraphQLHandlers;
  }

  closeDetail() {
    this.entityId = undefined;
    this.detailView = false;
  }

  setSysName(sysName: string) {
    this.sysName = sysName;
    this.crudService = new CrudHttpService(this.sysName, this.appConfig);
    this.visibleFields = restoreVisibleFields(this.sysName);
  }

  setHandlers(handlers: GraphQLHandlers) {
    this.handlers = handlers;
  }

  fetchConfiguration = fetchDataState({
    getDataState: () => this.config,
    condition: () => !this.config.loading && isDefined(this.sysName),
    getData: () => this.entityService.getEntityConfiguration(this.sysName!),
    onError: (err) => {
      this.notificationStore.error(
        "Произошла ошибка при получении конфигурации"
      );
      console.log(err);
    },
    onSuccess: () => {
      this.configurationLoaded = true;
    },
  });

  fetchEntities = () =>
    fetchGraphQLCollectionState({
      graphqlService: this.graphqlService,
      query: buildEntityQueryTemplate(
        this.config.value,
        this.queryListViewFieldFactory!
      ),
      handlers: this.ensureHandlers(this.handlers),
      condition: () =>
        isDefined(this.sysName) &&
        isDefined(this.queryListViewFieldFactory) &&
        isDefined(this.config.loaded) &&
        isDefined(this.config.value?.fields) &&
        isDefined(this.crudService),
      getCollectionState: () => this.entities,
      onError: (err) => {
        this.notificationStore.error("Произошла ошибка при загрузке объектов");
        console.log(err);
      },
    })();

  fetchEntity = () => {
    const condition = () =>
      isDefined(this.sysName) &&
      isDefined(this.queryDetailViewFieldFactory) &&
      isDefined(this.entityId) &&
      !this.entity.loading &&
      isDefined(this.crudService);

    if (condition()) {
      this.isEntityFavorite.loading = true;
      this.entityService
        .getEntityFavorite(this.sysName!, this.entityId!)
        .then((res) => {
          if (res.isSuccess) {
            runInAction(() => {
              this.isEntityFavorite.value = res.object!;
              this.isEntityFavorite.loaded = true;
            });
          }
        })
        .finally(() =>
          runInAction(() => (this.isEntityFavorite.loading = false))
        );
    }

    return fetchGraphQLDataState({
      graphqlService: this.graphqlService,
      query: buildEntityQueryTemplate(
        this.config.value,
        this.queryDetailViewFieldFactory!
      ),
      handlers: getByIdGraphQLHandlers(this.entityId!),
      condition: condition,
      getDataState: () => this.entity,
      onSuccess: () => {
        this.linksToAddOrUpdate = [];
        this.linksToDelete = [];
      },
      onError: (err) => {
        this.notificationStore.error("Произошла ошибка при получении объекта");
        console.log(err);
        this.linksToAddOrUpdate = [];
        this.linksToDelete = [];
      },
    })();
  };

  createEntity = (entity: any, successCallback?: (createdId: number) => void) =>
    performAction<any>({
      condition: () => isDefined(this.crudService),
      action: () =>
        this.crudService!.createEntity({
          entity,
          linksToAddOrUpdate: this.linksToAddOrUpdate,
          linksToDelete: this.linksToDelete,
        }),
      setLoading: (loading) => (this.loading = loading),
      onSuccess: (res) => {
        if (res.isSuccess) {
          successCallback?.(res.object.Id);
        }
      },
      onError: (err) => {
        this.notificationStore.error("Произошла ошибка при создании объекта");
        console.log(err);
      },
    })();

  updateEntity = (entity: any) =>
    performAction({
      condition: () => isDefined(this.entityId) && isDefined(this.crudService),
      action: () =>
        this.crudService!.updateEntity(this.entityId!, {
          entity,
          linksToAddOrUpdate: this.linksToAddOrUpdate,
          linksToDelete: this.linksToDelete,
        }),
      setLoading: (loading) => (this.loading = loading),
      onSuccess: () => {
        this.fetchEntity();
        this.fetchEntities();
      },
      onError: (err) => {
        this.notificationStore.error("Произошла ошибка при обновлении объекта");
        console.log(err);
      },
    })();

  deleteEntity = (entityId?: number) =>
    performAction({
      condition: () =>
        isDefined(entityId ?? this.entityId) && isDefined(this.crudService),
      action: () =>
        this.crudService!.deleteEntity((entityId ?? this.entityId)!),
      setLoading: (loading) => (this.loading = loading),
      onSuccess: () => {
        this.fetchEntities();
      },
      onError: (err) => {
        this.notificationStore.error("Произошла ошибка при удалении объекта");
        console.log(err);
      },
    })();

  toggleFavorite = (sysName: string, id?: number) =>
    performAction({
      action: () => this.entityService!.toggleFavorite(sysName, id),
      onError: (err) => {
        this.notificationStore.error(
          "Произошла ошибка при добавлении в избранное"
        );
        console.log(err);
      },
    })();

  fetchEntityTypes = (pageModel?: PageModel) =>
    fetchDataState({
      getDataState: () => this.entityTypes,
      condition: () =>
        !this.entityTypes.loading && isDefined(this.entityService),
      getData: () => this.entityService!.getEntityTypes(pageModel),
      onError: (err) => {
        this.notificationStore.error(
          "Произошла ошибка при получении списка объектов"
        );
        console.log(err);
      },
    })();

  private ensureHandlers = (handlers: GraphQLHandlers) => {
    handlers.sortHandlers ??= [];
    if (!handlers.sortHandlers.some((x) => x.key === "Id")) {
      handlers.sortHandlers.push({ key: "Id", direction: "ASC" });
    }

    return handlers;
  };
}
