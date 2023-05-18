import { makeAutoObservable } from "mobx";
import { Boundaries, GeoEntityType } from "src/models";
import {
  buildGeoEntitiesQuery,
  fetchDataState,
  fetchGraphQLEntities,
  firstLetterToLower,
} from "src/utils";
import { NotificationStore } from "../notification-store";
import { GraphqlService } from "src/services";
import { GeoEntity } from "./map-store.types";
import { DataState } from "../utils";
import EntityService from "src/services/entities-service/entities-service.types";

export class MapStore {
  readonly geoEntityTypes: DataState<GeoEntityType[]> = new DataState<
    GeoEntityType[]
  >();

  selectedGeoEntityTypes: GeoEntityType[] = [];

  constructor(
    private readonly notificationStore: NotificationStore,
    private readonly graphqlService: GraphqlService,
    private readonly entityService: EntityService
  ) {
    makeAutoObservable(this);
  }

  selectGeoEntityTypes = (models: GeoEntityType[]) => {
    this.selectedGeoEntityTypes = models;
  };

  fetchGeoEntities = async (model: GeoEntityType, bounds: Boundaries) => {
    const filters = [];

    //  _northEast.lat _southWest.lng
    //  _northEast.lat _northEast.lng
    //  _southWest.lat _northEast.lng
    //  _southWest.lat _northEast.lng
    //  _northEast.lat _southWest.lng

    filters.push({
      key: firstLetterToLower(model.geoFieldName),
      operator: "intersects",
      expression: `{
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
      }`,
    });

    return (await fetchGraphQLEntities({
      graphqlService: this.graphqlService,
      query: buildGeoEntitiesQuery(model, bounds),
      onError: (err) => {
        this.notificationStore.error(
          `Произошла ошибка при загрузке объектов ${
            model.displayName ?? model.systemName
          }`
        );
        console.log(err);
      },
    })) as GeoEntity[];
  };

  fetchGeoEntityTypes = () =>
    fetchDataState({
      getDataState: () => this.geoEntityTypes,
      getData: () => this.entityService.getGeoEntityTypes(),
      onError: (err) => {
        this.notificationStore.error(
          "Произошла ошибка при получении списка гео-объектов"
        );
        console.log(err);
      },
    })();
}
