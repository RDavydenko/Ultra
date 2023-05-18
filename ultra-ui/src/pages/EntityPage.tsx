import React, { FC, useEffect } from "react";
import { useParams } from "react-router-dom";
import { isDefined } from "src/utils";
import { EntityStore } from "src/stores/entity-store/entity-store";
import { CrudHttpService, EntitiesHttpService } from "src/services";
import { useStores } from "src/stores";
import ListViewEntity from "src/components/Entity/ListViewEntity";
import { Content } from "antd/lib/layout/layout";
import { useEntityContext } from "src/infrastructure";

export enum ViewTypes {
  View,
  Create,
  Edit,
}

const EntityPage: FC = () => {
  const { entityStore } = useStores();
  const { sysName, id } = useParams();
  const { typesResolver } = useEntityContext();

  useEffect(() => {
    entityStore.setQueryListViewFieldFactory(
      typesResolver.getListViewGraphQLProjection
    );
    entityStore.setQueryDetailViewFieldFactory(
      typesResolver.getDetailViewGraphQLProjection
    );
    if (isDefined(id)) {
      entityStore.setSysName(sysName!);
      entityStore.fetchConfiguration().then(() => {
        entityStore.fetchEntities();
        entityStore.openDetail(Number(id));
      });
    } else if (sysName !== entityStore.sysName) {
      entityStore.reset();
      entityStore.setSysName(sysName!);
      entityStore.fetchConfiguration().then(() => {
        entityStore.fetchEntities();
      });
    }
  }, [
    sysName,
    id,
    typesResolver.getListViewGraphQLProjection,
    typesResolver.getDetailViewGraphQLProjection,
    entityStore,
  ]);

  return (
    <Content
      style={{
        padding: 24,
        margin: 0,
        minHeight: 280,
      }}
    >
      <ListViewEntity store={entityStore} />
    </Content>
  );
};

export default EntityPage;
