import {
  CollectionPage,
  EntityConfiguration,
  EntityType,
  GeoEntityType,
  PageModel,
} from "src/models";

import { Dummy, HttpResponse } from "../interfaces";

export default interface EntityService {
  getEntityConfiguration: (
    sysName: string
  ) => Promise<HttpResponse<EntityConfiguration>>;

  toggleFavorite: (
    sysName: string,
    id?: number
  ) => Promise<HttpResponse<Dummy>>;

  getEntityFavorite: (
    sysName: string,
    id: number
  ) => Promise<HttpResponse<boolean>>;

  getEntityTypes: (
    pageModel?: PageModel
  ) => Promise<HttpResponse<CollectionPage<EntityType>>>;

  getGeoEntityTypes: () => Promise<HttpResponse<GeoEntityType[]>>;
}
