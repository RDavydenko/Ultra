import {
  EnvironmentOutlined,
  GlobalOutlined,
  StarFilled,
} from "@ant-design/icons";
import { Card, Space, Tooltip, Typography } from "antd";
import React, { FC, useState } from "react";
import { Link } from "react-router-dom";
import classNames from "classnames";

import { useStores } from "src/stores";
import { EntityType } from "src/models";
import { routes } from "src/consts";

interface DataItemProps {
  entityType: EntityType;
}

const DataItem: FC<DataItemProps> = ({ entityType }) => {
  const [favoriteInternal, setFavoriteInternal] = useState(entityType.favorite);
  const { entityStore } = useStores();

  const toggleFavorite = (e: React.MouseEvent) => {
    e.preventDefault();
    setFavoriteInternal(!favoriteInternal);
    entityStore.toggleFavorite(entityType.systemName);
  };

  return (
    <Link to={routes.data.entities.path(entityType.systemName)}>
      <Card
        style={{ width: 300, margin: 8 }}
        title={
          <Typography.Title level={5}>
            {entityType.displayName ?? entityType.systemName}
          </Typography.Title>
        }
        extra={
          <Space>
            {entityType.isGeoEntity && (
              <Tooltip title="Поддерживается отображение объектов на карте">
                <EnvironmentOutlined onClick={(e) => e.preventDefault()} />
              </Tooltip>
            )}
            <Tooltip
              title={
                favoriteInternal
                  ? "Убрать из избранных"
                  : "Добавить в избранные"
              }
            >
              <StarFilled
                className={classNames("favorite-star-icon", {
                  selected: favoriteInternal,
                })}
                onClick={(e) => toggleFavorite(e)}
              />
            </Tooltip>
          </Space>
        }
        hoverable
      >
        <div>
          <Typography.Text type="secondary">
            Системное название:{" "}
          </Typography.Text>{" "}
          <span>{entityType.systemName}</span>
        </div>
        <div>
          <Typography.Text type="secondary">Количество: </Typography.Text>{" "}
          <span>{entityType.count}</span>
        </div>
      </Card>
    </Link>
  );
};

export default DataItem;
