import { ExpandOutlined, MinusOutlined, PlusOutlined } from "@ant-design/icons";
import { Button, Space, Table } from "antd";
import _ from "lodash";
import { observer } from "mobx-react";
import React, { FC, useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { InputProps } from "src/infrastructure/interfaces";
import { useStores } from "src/stores";
import DebounceSelect, { useSelect } from "../components/DebounceSelect";
import { routes } from "src/consts";

interface RelatedEntity {
  Id: number;
  DisplayName: string;
}

const PAGE_SIZE = 5;

const ReferenceChildrenInput: FC<InputProps> = ({
  provider,
  config,
  disabled,
}) => {
  const { controller } = useSelect();
  const [currentPage, setCurrentPage] = useState(1);
  const [entities, setEntities] = useState<RelatedEntity[]>([]);
  const [selectedId, setSelectedId] = useState(0);
  const [selectedDisplayName, setSelectedDisplayName] = useState("");
  const [addedItemSelected, setAddedItemSelected] = useState(false);
  const { entityStore, collectionStore } = useStores();

  const relatedSystemName = config.meta["foreignKey.type"];
  const relatedPropertyName = config.meta["foreignKey.path"];
  const relatedDisplayNamePath = config.meta["foreignKey.displayable"];

  provider.getValue = () => entities;
  provider.setValue = (val) => setEntities(val);

  useEffect(() => {
    const newPage = Math.floor(entities.length / PAGE_SIZE) + 1;
    setCurrentPage(newPage);
  }, [entities]);

  const unlink = (id: number) => {
    setEntities(entities.filter((x) => x.Id !== id));

    const alreadyAddedIndex = entityStore.linksToAddOrUpdate.findIndex(
      (x) =>
        x.entityId === id &&
        x.entitySystemName === relatedSystemName &&
        x.entityPropertyName === relatedPropertyName
    );
    if (alreadyAddedIndex !== -1) {
      entityStore.linksToAddOrUpdate.splice(alreadyAddedIndex, 1);
    } else {
      entityStore.linksToDelete.push({
        entityId: id,
        entitySystemName: relatedSystemName,
        entityPropertyName: relatedPropertyName,
      });
    }
  };

  const link = (id: number, displayName: string) => {
    setEntities([...entities, { Id: id, DisplayName: displayName }]);

    const alreadyDeletedIndex = entityStore.linksToDelete.findIndex(
      (x) =>
        x.entityId === id &&
        x.entitySystemName === relatedSystemName &&
        x.entityPropertyName === relatedPropertyName
    );
    if (alreadyDeletedIndex !== -1) {
      entityStore.linksToDelete.splice(alreadyDeletedIndex, 1);
    }

    entityStore.linksToAddOrUpdate.push({
      entityId: id,
      entitySystemName: relatedSystemName,
      entityPropertyName: relatedPropertyName,
    });
  };

  const addLink = () => {
    link(selectedId, selectedDisplayName);
    setSelectedId(0);
    setSelectedDisplayName("");
    setAddedItemSelected(false);
    controller.clear();
  };

  const selectRelated = (id: number, displayName: string) => {
    setAddedItemSelected(true);
    setSelectedId(id);
    setSelectedDisplayName(displayName);
  };

  const fetchRelatedEntities = async (q?: string) => {
    const collection = await collectionStore.fetchEntities(
      relatedSystemName,
      relatedDisplayNamePath,
      q,
      entities.map((x) => x.Id)
    );
    return collection.map((x) => {
      return {
        value: x.Id,
        label: x.DisplayName,
      };
    });
  };

  return (
    <Table
      bordered
      size="small"
      scroll={{ y: 240 }}
      footer={() =>
        !disabled && (
          <div
            style={{
              width: "100%",
              display: "flex",
              justifyContent: "flex-end",
            }}
          >
            <DebounceSelect
              controller={controller}
              value={{ value: selectedId, label: selectedDisplayName }}
              style={{ width: "400px" }}
              size="small"
              showSearch
              placeholder={`Выберите запись ${relatedSystemName} для добавления`}
              fetchOptions={fetchRelatedEntities}
              onChange={(_, option) => {
                if (Array.isArray(option)) return;
                selectRelated(Number(option.value), String(option.label));
              }}
            />
            <Button
              disabled={!addedItemSelected}
              size="small"
              style={{
                ...(addedItemSelected ? { color: "green" } : {}),
                marginLeft: "0.2rem",
              }}
              icon={<PlusOutlined />}
              onClick={addLink}
            >
              Добавить
            </Button>
          </div>
        )
      }
      pagination={{
        current: currentPage,
        pageSize: PAGE_SIZE,
        showTotal: (total, range) =>
          `${range[0]}-${range[1]} из ${total} записей`,
        showSizeChanger: false,
        showLessItems: false,
        hideOnSinglePage: true,
        onChange: (page, pageSize) => setCurrentPage(page),
      }}
      columns={[
        {
          key: "1",
          title: "ID",
          dataIndex: "Id",
          width: "150px",
        },
        {
          key: "2",
          title: "Название",
          dataIndex: "DisplayName",
        },
        {
          key: "3",
          width: "70px",
          render: (_, record) => (
            <Space size={5}>
              <Link
                to={routes.data.entities.byId.path(
                  relatedSystemName,
                  record.Id
                )}
                target="_blank"
                rel="noopener noreferrer"
              >
                <Button
                  size="small"
                  icon={<ExpandOutlined />}
                  title={`Открыть запись #${record.Id}`}
                />
              </Link>
              <Button
                disabled={disabled}
                size="small"
                onClick={() => unlink(record.Id)}
                style={{ color: "red" }}
                icon={<MinusOutlined />}
                title={`Отвязать запись #${record.Id}`}
              />
            </Space>
          ),
        },
      ]}
      dataSource={entities}
    />
  );
};

export default observer(ReferenceChildrenInput);
