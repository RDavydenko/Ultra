import { SearchOutlined } from "@ant-design/icons";
import { Button, Drawer } from "antd";
import L, { Control } from "leaflet";
import "leaflet.markercluster";
import { observer } from "mobx-react";
import React, { FC, useEffect, useMemo, useRef, useState } from "react";

import {
  MapContainer,
  TileLayer,
  Marker,
  Popup,
  LayersControl,
} from "react-leaflet";
import MarkerClusterGroup from "react-leaflet-markercluster";
import { useStores } from "src/stores";
import { GeoEntity } from "src/stores/map-store/map-store.types";
import { isDefined } from "src/utils";
import { v4 as uuidV4 } from "uuid";
import EntityLayersSelector from "./components/EntityLayersSelector";
import EntitySearch from "./components/EntitySearch";
import EntitySearchButton from "./components/EntitySearchButton";

import "./Map.scss";
import { MapGeoEntity } from "./Map.types";
import { getMarkerStyle } from "./Map.utils";

const LOCAL_STORAGE_KEY = "map_last_session";

const saveCenterAndZoom = (latlng: L.LatLng, zoom: number) => {
  localStorage.setItem(
    LOCAL_STORAGE_KEY,
    JSON.stringify({
      latlng,
      zoom,
    })
  );
};

const restoreCenterAndZoom = ():
  | { latlng: L.LatLng; zoom: number }
  | undefined => {
  const data = localStorage.getItem(LOCAL_STORAGE_KEY);
  if (data) {
    return JSON.parse(data);
  }
  return undefined;
};

const markers = L.markerClusterGroup({
  maxClusterRadius: 50,
  spiderfyOnMaxZoom: true,
  showCoverageOnHover: false,
  zoomToBoundsOnClick: true,
  animate: true,
});

const Map: FC = () => {
  const [map, setMap] = useState<L.Map | null>(null);
  const [defaultCenter] = useState<L.LatLngExpression | undefined>(
    isDefined(restoreCenterAndZoom()?.latlng)
      ? [restoreCenterAndZoom()!.latlng.lat, restoreCenterAndZoom()!.latlng.lng]
      : undefined
  );
  const [defaultZoom] = useState<number | undefined>(
    restoreCenterAndZoom()?.zoom
  );
  const [entities, setEntities] = useState<MapGeoEntity[]>([]);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const fetchRef = useRef(0);
  const { mapStore } = useStores();

  useEffect(() => {
    if (map) {
      fetchGeoEntities(map.getBounds(), map.getZoom());
    }
  }, [mapStore.selectedGeoEntityTypes]);

  useEffect(() => {
    if (map === null) return;

    map.addLayer(markers);

    map.on("click", (e) => onMapClick(e.latlng.lat, e.latlng.lng));
    map.on("moveend", (e) => {
      saveCenterAndZoom(map.getCenter(), map.getZoom());
      // TODO: Потом добавить подгрузку по границам карты или кластеризацию
      //fetchGeoEntities(map.getBounds(), map.getZoom());
    });
  }, [map]);

  const onMapClick = (lat: number, lng: number) => {
    console.log("lat:", lat, "lng:", lng);
  };

  const fetchGeoEntities = (bounds: L.LatLngBounds, zoom: number) => {
    fetchRef.current += 1;
    const fetchId = fetchRef.current;

    Promise.all(
      mapStore.selectedGeoEntityTypes.map(async (type) => {
        const entities = await mapStore.fetchGeoEntities(type, {
          southWest: {
            lat: bounds.getSouthWest().lat,
            lng: bounds.getSouthWest().lng,
          },
          northEast: {
            lat: bounds.getNorthEast().lat,
            lng: bounds.getNorthEast().lng,
          },
        });
        return { entities, type };
      })
    ).then((results) => {
      if (fetchId !== fetchRef.current) {
        return;
      }

      markers.clearLayers();
      const geoEntities: MapGeoEntity[] = [];
      for (const { entities, type } of results) {
        const typeDisplayName = type.displayName ?? type.systemName;
        const markerClassName = getMarkerStyle(type.systemName).className;

        for (let i = 0; i < entities.length; i++) {
          const obj = entities[i];

          if (!isDefined(obj.location?.coordinates)) {
            continue;
          }

          const markerId = uuidV4();
          geoEntities.push({
            ...obj,
            markerId: markerId,
            typeId: type.id,
            typeSystemName: type.systemName,
            typeDisplayName: type.displayName,
          });

          const marker = L.marker(
            new L.LatLng(
              obj.location.coordinates[0],
              obj.location.coordinates[1]
            ),
            {
              icon: L.icon({
                iconUrl:
                  "https://unpkg.com/leaflet@1.9.3/dist/images/marker-icon.png",
                shadowUrl:
                  "https://unpkg.com/leaflet@1.9.3/dist/images/marker-shadow.png",
                className: markerClassName + ` geo-marker-${markerId}`,
                iconSize: [25, 41],
                iconAnchor: [12, 41],
                popupAnchor: [1, -34],
                tooltipAnchor: [16, -28],
                shadowSize: [41, 41],
              }),
            }
          );
          marker.bindPopup(
            L.popup({
              content: `
              <div>
                <b>${typeDisplayName} #${obj.id}</b>
                <p style="margin: 0">${obj.displayName}</p>
                <a href="/d/${type.systemName}/${obj.id}" target="_blank">Открыть</a>
              </div>`,
            })
          );
          markers.addLayer(marker);
        }
      }
      setEntities(geoEntities);
    });
  };

  return (
    <>
      <MapContainer
        center={defaultCenter ?? [53.3399, 83.74073]}
        zoom={defaultZoom ?? 11.5}
        scrollWheelZoom={true}
        ref={setMap}
        fadeAnimation
        zoomAnimation
      >
        <LayersControl>
          <LayersControl.BaseLayer checked name="Карта OpenStreetMap">
            <TileLayer
              url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
              attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            />
          </LayersControl.BaseLayer>
          <LayersControl.BaseLayer name="Карта Google">
            <TileLayer
              url="http://{s}.google.com/vt/lyrs=m&x={x}&y={y}&z={z}"
              maxZoom={20}
              subdomains={["mt0", "mt1", "mt2", "mt3"]}
              attribution='&copy; <a href="https://google.com" target="_blank">Google</a>'
            />
          </LayersControl.BaseLayer>
          <LayersControl.BaseLayer name="Карта 2GIS">
            <TileLayer
              url="https://tile{s}.maps.2gis.com/tiles?x={x}&y={y}&z={z}&v=1"
              subdomains={["0", "1", "2", "3"]}
              maxZoom={18}
              attribution='&copy; <a href="http://www.2gis.ru" target="_blank" title="2ГИС — Городской информационный сервис"> 2ГИС — Городской информационный сервис</a>'
            />
          </LayersControl.BaseLayer>
        </LayersControl>
        <EntityLayersSelector store={mapStore} />
        <EntitySearchButton onClick={() => setDrawerOpen(true)} />
      </MapContainer>
      <Drawer
        title="Поиск на карте"
        placement="right"
        open={drawerOpen}
        width={500}
        onClose={() => setDrawerOpen(false)}
      >
        <EntitySearch map={map} entities={entities} />
      </Drawer>
    </>
  );
};

export default observer(Map);
