export interface LatLngLocal {
  lat: number;
  lng: number;
}

export interface Boundaries {
  northEast: LatLngLocal;
  southWest: LatLngLocal;
}
