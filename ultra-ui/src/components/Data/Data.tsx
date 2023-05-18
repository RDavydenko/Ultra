import { Card, Pagination } from "antd";
import { observer } from "mobx-react";
import React, { FC, useEffect } from "react";
import { useStores } from "src/stores";
import DataItem from "./components/DataItem";

const PAGE_SIZE = 20;

const Data: FC = () => {
  const { entityStore } = useStores();

  useEffect(() => {
    entityStore.fetchEntityTypes({
      pageSize: PAGE_SIZE,
      pageNumber: 1,
    });
  }, []);

  const fetchPage = (page: number, pageSize: number) => {
    entityStore.fetchEntityTypes({
      pageSize,
      pageNumber: page,
    });
  };

  return (
    <div style={{ marginLeft: "5px", height: "100%" }}>
      <div style={{ display: "inline-flex", flexWrap: "wrap" }}>
        {entityStore.entityTypes.loading
          ? [...Array(PAGE_SIZE)].map((_, i) => (
              <Card key={i} loading={true} style={{ width: 300, margin: 8 }} />
            ))
          : entityStore.entityTypes.value.items?.map((x) => (
              <DataItem key={x.systemName} entityType={x} />
            ))}
      </div>
      {entityStore.entityTypes.loaded && (
        <div style={{ position: "relative", width: "100%", height: "50px" }}>
          <Pagination
            onChange={fetchPage}
            hideOnSinglePage
            showTotal={(total, range) =>
              `${range[0]}-${range[1]} из ${total} записей`
            }
            pageSize={PAGE_SIZE}
            showSizeChanger={false}
            total={entityStore.entityTypes.value.pageInfo.totalCount}
            disabled={entityStore.entityTypes.loading}
            style={{ position: "absolute", right: "15px", bottom: "15px" }}
          />
        </div>
      )}
    </div>
  );
};

export default observer(Data);
