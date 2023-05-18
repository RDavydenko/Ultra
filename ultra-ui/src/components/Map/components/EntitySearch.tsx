import React, { FC, useMemo, useState } from "react";
import L from "leaflet";
import { observer } from "mobx-react";
import { useStores } from "src/stores";
import { Button, Collapse, Form, Input, Select, Space, Table } from "antd";
import { MapGeoEntity } from "../Map.types";
import { Link } from "react-router-dom";
import { ExpandOutlined, SearchOutlined } from "@ant-design/icons";
import { groupBy, orderBy } from "lodash";
import { isDefined } from "src/utils";
import { routes } from "src/consts";

interface SearchGeoEntityType {
  typeSystemName: string;
  typeDisplayName?: string;
  entities: MapGeoEntity[];
}

interface EntitySearchProps {
  map: L.Map | null;
  entities: MapGeoEntity[];
}

const EntitySearch: FC<EntitySearchProps> = ({ map, entities }) => {
  const { mapStore } = useStores();
  const [name, setName] = useState("");
  const [entityTypeId, setEntityTypeId] = useState<number | undefined>();

  const geoEntities = useMemo(() => {
    const data = isDefined(entityTypeId)
      ? entities.filter((x) => x.typeId === entityTypeId)
      : entities;
    const grouped = groupBy(data, (x) => x.typeSystemName);
    const objects: SearchGeoEntityType[] = [];

    for (const key in grouped) {
      const array = grouped[key];
      const type = mapStore.geoEntityTypes.value.find(
        (x) => x.systemName === key
      );
      objects.push({
        typeSystemName: key,
        typeDisplayName: type?.displayName,
        entities: orderBy(
          array.filter((x) => x.displayName.toLowerCase().includes(name)),
          (x) => x.id
        ),
      });
    }

    return objects;
  }, [entities, name, entityTypeId]);

  const blinkMarker = (markerId: string, duration = 3000) => {
    const markers = document.getElementsByClassName(`geo-marker-${markerId}`);
    for (let i = 0; i < markers.length; i++) {
      markers[i].classList.add("blink");
    }
    setTimeout(() => {
      for (let i = 0; i < markers.length; i++) {
        markers[i].classList.remove("blink");
      }
    }, duration);
  };

  const show = (entity: MapGeoEntity) => {
    const flyDurationInSeconds = 1.5;
    map?.flyTo(
      {
        lat: entity.location.coordinates[0],
        lng: entity.location.coordinates[1],
      },
      18, // maxZoom, выбирать среди layer'ов в Map.tsx
      { duration: flyDurationInSeconds }
    );
    setTimeout(() => blinkMarker(entity.markerId), flyDurationInSeconds * 1000);
  };

  const find = (q: string) => setName(q.toLowerCase());

  const chooseEntityType = (entityTypeId?: number) =>
    setEntityTypeId(entityTypeId);

  return (
    <div>
      <Form layout="vertical">
        <Form.Item label="Тип объекта">
          <Select
            style={{ width: "100%" }}
            allowClear
            placeholder="Выберите тип объекта"
            options={mapStore.selectedGeoEntityTypes.map((x) => {
              return { label: x.displayName ?? x.systemName, value: x.id };
            })}
            onChange={(val) => chooseEntityType(val)}
          />
        </Form.Item>
        <Form.Item label="Название">
          <Input
            style={{ width: "100%" }}
            placeholder="Введите название"
            onChange={(e) => find(e.target.value)}
          />
        </Form.Item>
        <Form.Item label="Объекты">
          {geoEntities.map((group) => (
            <Collapse
              key={group.typeSystemName}
              className="ant-collapse-small"
              defaultActiveKey={[group.typeSystemName]}
            >
              <Collapse.Panel
                header={group.typeDisplayName ?? group.typeSystemName}
                key={group.typeSystemName}
              >
                <Table
                  size="small"
                  dataSource={group.entities}
                  columns={[
                    {
                      title: "ID",
                      dataIndex: "id",
                      key: "id",
                      width: "100px",
                    },
                    {
                      title: "Название",
                      key: "displayName",
                      dataIndex: "displayName",
                    },
                    {
                      key: "actions",
                      width: "70px",
                      render: (_, record) => (
                        <Space size={5}>
                          <Button
                            size="small"
                            key="1"
                            onClick={() => show(record)}
                            icon={<SearchOutlined />}
                            title={`Показать на карте`}
                          />
                          <Link
                            key="2"
                            to={routes.data.entities.byId.path(
                              record.typeSystemName,
                              record.id
                            )}
                            target="_blank"
                            rel="noopener noreferrer"
                          >
                            <Button
                              size="small"
                              icon={<ExpandOutlined />}
                              title={`Открыть запись #${record.id}`}
                            />
                          </Link>
                        </Space>
                      ),
                    },
                  ]}
                  pagination={{
                    pageSize: 5,
                    hideOnSinglePage: true,
                  }}
                />
              </Collapse.Panel>
            </Collapse>
          ))}
        </Form.Item>
      </Form>
    </div>
  );
};

export default observer(EntitySearch);
