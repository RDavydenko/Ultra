import urljoin from "url-join";

import { authorizedRequest } from "../httpService";
import CrudService from "./crud-service.types";
import { AppConfig } from "src/models";
import {
  objToPascalCase,
  objToCamelCase,
  removeNullableFields,
} from "src/utils";
import { EntityWithRelatedLinks } from "src/models/entity/EntityWithRelatedLinks";

export class CrudHttpService implements CrudService {
  constructor(
    private readonly sysName: string,
    private readonly appConfig: AppConfig
  ) {}

  getEntities = async () => {
    const res = await authorizedRequest<any[]>({
      method: "GET",
      url: urljoin(this.appConfig.apiUrl, "crud", this.sysName),
    });
    if (res.isSuccess) {
      res.object = res.object!.map((x) => objToPascalCase(x));
    }
    return res;
  };

  getEntityById = async (id: number) => {
    const res = await authorizedRequest<any>({
      method: "GET",
      url: urljoin(this.appConfig.apiUrl, "crud", this.sysName, id.toString()),
    });
    if (res.isSuccess) {
      res.object = objToPascalCase(res.object!);
    }
    return res;
  };

  createEntity = async (model: EntityWithRelatedLinks) => {
    const res = await authorizedRequest<any>({
      method: "POST",
      data: removeNullableFields(objToCamelCase(model)),
      url: urljoin(this.appConfig.apiUrl, "crud", this.sysName),
    });
    if (res.isSuccess) {
      res.object = objToPascalCase(res.object!);
    }
    return res;
  };

  updateEntity = async (id: number, model: EntityWithRelatedLinks) => {
    const res = await authorizedRequest<any>({
      method: "PUT",
      data: removeNullableFields(objToCamelCase(model)),
      url: urljoin(this.appConfig.apiUrl, "crud", this.sysName, id.toString()),
    });
    if (res.isSuccess) {
      res.object = objToPascalCase(res.object!);
    }
    return res;
  };

  patchEntity = (id: number, model: any) =>
    authorizedRequest<any>({
      method: "PATCH",
      data: model,
      url: urljoin(this.appConfig.apiUrl, "crud", this.sysName, id.toString()),
    });

  deleteEntity = (id: number) =>
    authorizedRequest<any>({
      method: "DELETE",
      url: urljoin(this.appConfig.apiUrl, "crud", this.sysName, id.toString()),
    });
}
