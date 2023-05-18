import { observer } from "mobx-react";
import React, { FC } from "react";
import { EntityStore } from "src/stores/entity-store/entity-store";
import EntitiesTable from "./components/EntitiesTable/EntitiesTable";
import DetailViewEntity from "./DetailViewEntity";

interface ListViewEntityProps {
  store: EntityStore;
}

const ListViewEntity: FC<ListViewEntityProps> = ({ store }) => {
  return (
    <>
      <EntitiesTable store={store} />
      <DetailViewEntity store={store} />
    </>
  );
};

export default observer(ListViewEntity);
