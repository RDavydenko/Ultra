import { EntityWithRelatedLinks } from "src/models/entity/EntityWithRelatedLinks";
import { HttpResponse } from "../interfaces";

export default interface CrudService {
  getEntities: () => Promise<HttpResponse<any[]>>;
  getEntityById: (id: number) => Promise<HttpResponse<any>>;
  createEntity: (model: EntityWithRelatedLinks) => Promise<HttpResponse<any>>;
  updateEntity: (
    id: number,
    model: EntityWithRelatedLinks
  ) => Promise<HttpResponse<any>>;
  patchEntity: (id: number, model: any) => Promise<HttpResponse<any>>; // TODO: patch model
  deleteEntity: (id: number) => Promise<HttpResponse<any>>;
}
