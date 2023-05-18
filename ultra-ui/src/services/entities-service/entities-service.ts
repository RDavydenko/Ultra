import urljoin from "url-join";

import { authorizedRequest } from "../httpService";
import EntitiesService from "./entities-service.types";
import {
  AppConfig,
  CollectionPage,
  EntityConfiguration,
  EntityType,
  GeoEntityType,
  PageModel,
} from "src/models";
import { Dummy } from "../interfaces";

export class EntitiesHttpService implements EntitiesService {
  constructor(private appConfig: AppConfig) {}

  getEntityConfiguration = (sysName: string) =>
    authorizedRequest<EntityConfiguration>({
      method: "GET",
      url: urljoin(this.appConfig.apiUrl, "Entities", sysName, "configuration"),
    });

  toggleFavorite = (sysName: string, id?: number) =>
    authorizedRequest<Dummy>({
      method: "POST",
      url: urljoin(
        this.appConfig.apiUrl,
        "Entities",
        sysName,
        "favorite",
        id !== undefined ? id.toString() : ""
      ),
    });

  getEntityFavorite = (sysName: string, id: number) =>
    authorizedRequest<boolean>({
      method: "GET",
      url: urljoin(
        this.appConfig.apiUrl,
        "Entities",
        sysName,
        "favorite",
        id.toString()
      ),
    });

  getEntityTypes = (pageModel?: PageModel) =>
    authorizedRequest<CollectionPage<EntityType>>({
      method: "GET",
      url: urljoin(this.appConfig.apiUrl, "Entities"),
      params: pageModel,
    });

  getGeoEntityTypes = () =>
    authorizedRequest<GeoEntityType[]>({
      method: "GET",
      url: urljoin(this.appConfig.apiUrl, "Entities", "geo"),
    });
}
