import {
  PushpinOutlined,
  CloseOutlined,
  UpSquareOutlined,
  UpOutlined,
} from "@ant-design/icons";
import { Card, Checkbox, Col, Divider, Row, Space } from "antd";
import { CheckboxChangeEvent } from "antd/lib/checkbox";
import { CheckboxValueType } from "antd/lib/checkbox/Group";
import { observer } from "mobx-react";
import React, { FC, useCallback, useEffect, useMemo, useState } from "react";
import { GeoEntityType } from "src/models";
import { MapStore } from "src/stores";
import { getMarkerStyle } from "../Map.utils";

interface EntityLayersSelectorProps {
  store: MapStore;
}

const EntityLayersSelector: FC<EntityLayersSelectorProps> = ({ store }) => {
  const MIN_WIDTH = 40,
    MIN_HEIGHT = 40,
    MAX_WIDTH = 300,
    MAX_HEIGHT = 300;
  const [width, setWidth] = useState(MIN_WIDTH);
  const [height, setHeight] = useState(MIN_HEIGHT);
  const [opened, setOpened] = useState(false);
  const [pinned, setPinned] = useState(false);

  const [checkedEntities, setCheckedEntities] = useState<GeoEntityType[]>(
    store.selectedGeoEntityTypes
  );
  const [checkAll, setCheckAll] = useState(false);

  useEffect(() => {
    store.fetchGeoEntityTypes();
  }, []);

  useEffect(() => {
    if (opened) {
      setWidth(MAX_WIDTH);
      setHeight(MAX_HEIGHT);
    } else {
      setWidth(MIN_WIDTH);
      setHeight(MIN_HEIGHT);
    }
  }, [opened]);

  useEffect(() => {
    if (store.geoEntityTypes.value.length) {
      setCheckAll(store.geoEntityTypes.value.length === checkedEntities.length);
    }
  }, [store.geoEntityTypes.value]);

  const onHover = () => {
    if (!opened) {
      setOpened(true);
    }
  };

  const onHoverLeave = () => {
    if (!pinned) {
      setOpened(false);
    }
  };

  const openAndPin = () => {
    setOpened(true);
    setPinned(true);
  };

  const unPin = () => {
    setPinned(false);
  };

  const setCheckedEntitiesWrapper = (entities: GeoEntityType[]) => {
    store.selectGeoEntityTypes(entities);
    setCheckedEntities(entities);
  };

  const onCheck = (e: CheckboxChangeEvent) => {
    const checked = e.target.checked;
    const value: GeoEntityType = e.target.value;

    let newCheckedEntities: GeoEntityType[] = [];
    if (!checked)
      newCheckedEntities = checkedEntities.filter((x) => x.id !== value.id);
    else newCheckedEntities = [...checkedEntities, value];

    setCheckedEntitiesWrapper(newCheckedEntities);
    setCheckAll(
      newCheckedEntities.length === store.geoEntityTypes.value.length
    );
  };

  const onCheckAllChange = (e: CheckboxChangeEvent) => {
    const checked = e.target.checked;
    const newCheckedEntities = checked ? store.geoEntityTypes.value : [];

    setCheckedEntitiesWrapper(newCheckedEntities);
    setCheckAll(checked);
  };

  const element = useMemo(
    () => (
      <Card
        title={opened && "Отображаемые объекты"}
        extra={
          opened &&
          (!pinned ? (
            <PushpinOutlined onClick={() => openAndPin()} title="Закрепить" />
          ) : (
            <CloseOutlined onClick={() => unPin()} title="Закрыть" />
          ))
        }
        onMouseOver={() => onHover()}
        onMouseLeave={() => onHoverLeave()}
        onClick={(e) => {
          !opened && openAndPin();
        }}
        onMouseMove={(e) => e.stopPropagation()}
        onDrag={(e) => e.stopPropagation()}
        style={{
          width: width,
          height: height,
          backgroundColor: "white",
          transition: "all .1s linear",
        }}
      >
        {!opened ? (
          <UpOutlined
            style={{
              fontSize: "24px",
              position: "relative",
              left: "-17px",
              top: "-18px",
            }}
          />
        ) : (
          <div>
            <Checkbox
              onChange={onCheckAllChange}
              checked={checkAll}
              style={{ marginBottom: "0.5rem" }}
            >
              Выбрать все
            </Checkbox>
            <div style={{ maxHeight: "170px", overflowY: "auto" }}>
              {store.geoEntityTypes.value.map((entity, i) => (
                <Row key={i}>
                  <Col span={24}>
                    <Checkbox
                      value={entity}
                      onChange={(e) => onCheck(e)}
                      checked={
                        checkedEntities.find((x) => x.id === entity.id) !==
                        undefined
                      }
                      style={{ width: "100%" }}
                    >
                      <Space>
                        {entity.displayName ?? entity.systemName}
                        <div
                          className="map__marker-preview"
                          style={{
                            backgroundColor: getMarkerStyle(entity.systemName)
                              .color,
                          }}
                        />
                      </Space>
                    </Checkbox>
                  </Col>
                </Row>
              ))}
            </div>
          </div>
        )}
      </Card>
    ),
    [
      width,
      height,
      opened,
      pinned,
      checkedEntities,
      store.geoEntityTypes.value,
      checkAll,
    ]
  );

  return (
    <div className="leaflet-bottom leaflet-left">
      <div className="leaflet-control leaflet-bar">{element}</div>
    </div>
  );
};

export default observer(EntityLayersSelector);
