import { GeoEntity } from "src/stores/map-store/map-store.types";

export type MapGeoEntity = GeoEntity & {
  markerId: string;
  typeId: number;
  typeSystemName: string;
  typeDisplayName?: string;
};
