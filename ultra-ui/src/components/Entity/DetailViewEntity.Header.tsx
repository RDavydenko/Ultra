import React, { FC, useEffect, useState } from "react";
import { Space, Tooltip, Typography } from "antd";
import { EntityConfiguration, EntityMethods } from "src/models";
import { EntityStore } from "src/stores/entity-store/entity-store";
import { isDefined } from "src/utils";
import { observer } from "mobx-react";
import { LoadingOutlined, StarFilled } from "@ant-design/icons";
import classNames from "classnames";

const { Text, Paragraph } = Typography;

interface DetailViewEntityHeaderProps {
  store: EntityStore;
}

const getCopyable = (config: EntityConfiguration, entity: any) => {
  if (!isDefined(entity?.Id)) {
    return false;
  }
  return {
    text: `@${config.systemName}#${entity.Id}`,
  };
};

const DetailViewEntityHeader: FC<DetailViewEntityHeaderProps> = ({ store }) => {
  const [favoriteInternal, setFavoriteInternal] = useState(false);

  useEffect(() => {
    if (store.isEntityFavorite.loaded) {
      setFavoriteInternal(store.isEntityFavorite.value);
    }
  }, [store.isEntityFavorite.value]);

  const toggleFavorite = (e: React.MouseEvent) => {
    e.preventDefault();
    setFavoriteInternal(!favoriteInternal);
    store.toggleFavorite(store.config.value.systemName, store.entityId!);
  };

  return store.config.loaded ? (
    <Space>
      <Text
        style={{ marginBottom: 0 }}
        copyable={
          store.entity.hasValue
            ? getCopyable(store.config.value, store.entity.value)
            : false
        }
      >
        {store.config.value.displayName ?? store.config.value.systemName}{" "}
        {store.entity.value.Id && "#" + store.entity.value.Id}
      </Text>
      {store.isEntityFavorite.loading ? (
        <LoadingOutlined />
      ) : store.detailType === EntityMethods.Update ? (
        <Tooltip
          title={
            favoriteInternal ? "Убрать из избранных" : "Добавить в избранные"
          }
        >
          <StarFilled
            className={classNames("favorite-star-icon", {
              selected: favoriteInternal,
            })}
            onClick={(e) => toggleFavorite(e)}
            spin={store.isEntityFavorite.loading}
          />
        </Tooltip>
      ) : (
        <></>
      )}
    </Space>
  ) : (
    <></>
  );
};

export default observer(DetailViewEntityHeader);
