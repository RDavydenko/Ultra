export interface GeoEntity {
  id: number;
  displayName: string;
  location: GeoLocation;
}

export interface GeoLocation {
  type: string;
  coordinates: number[];
}
