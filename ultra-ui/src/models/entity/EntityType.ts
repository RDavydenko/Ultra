export interface EntityType {
  systemName: string;
  displayName?: string;
  count: number;
  favorite: boolean;
  isGeoEntity: boolean;
}

export interface GeoEntityType {
  id: number;
  systemName: string;
  displayName?: string;
  geoFieldName: string;
  displayableField: string;
}
