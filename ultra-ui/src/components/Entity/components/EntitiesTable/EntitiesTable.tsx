import {
  DeleteOutlined,
  ExpandOutlined,
  SearchOutlined,
} from "@ant-design/icons";
import {
  Button,
  Checkbox,
  Input,
  InputRef,
  Popconfirm,
  Row,
  Space,
  Table,
} from "antd";
import {
  ColumnType,
  FilterConfirmProps,
  FilterValue,
  SorterResult,
  TableCurrentDataSource,
  TablePaginationConfig,
} from "antd/lib/table/interface";
import { observer } from "mobx-react";
import React, {
  FC,
  RefObject,
  useEffect,
  useMemo,
  useRef,
  useState,
} from "react";
import { EntityTypesResolver, useEntityContext } from "src/infrastructure";
import {
  EntityConfiguration,
  EntityMethods,
  FieldConfiguration,
} from "src/models";
import { EntityStore, NavigationStore, useStores } from "src/stores";
import { buildGraphQLHandlers } from "src/utils/antd";
import EntitiesHeader from "../EntitiesHeader/EntitiesHeader";
import { routes } from "src/consts";
import { isDefined } from "src/utils";
import { getColumnSearchProps } from "../ColumnSearchFilter";

interface EntitiesTableProps {
  store: EntityStore;
}

const getAllTableColumns = (
  config: EntityConfiguration,
  store: EntityStore,
  resolver: EntityTypesResolver,
  navigationStore: NavigationStore,
  filteredInfo: Record<string, FilterValue | null>,
  sortedInfo: SorterResult<any>
): ColumnType<any>[] => {
  const dataColumns: ColumnType<any>[] = config.fields
    .filter((field) => resolver.isVisibleInListView(field))
    .filter(
      (field) =>
        !isDefined(store.visibleFields) ||
        store.visibleFields.includes(field.systemName)
    )
    .map((field) => {
      const columnInfo = resolver.getListViewColumn(field)!;
      const column: ColumnType<any> = columnInfo;
      // TODO: Либо продумать более элегантное решение для фикса колонки ID
      if (field.systemName === "Id") {
        column.fixed = "left";
      }
      return {
        ...column,
        filteredValue: filteredInfo[field.systemName] || null,
        sortOrder:
          sortedInfo.columnKey === field.systemName ? sortedInfo.order : null,
        ...(columnInfo.filter ? getColumnSearchProps(field) : {}),
        key: field.systemName,
      };
    });

  const actionColumn: ColumnType<any> = {
    fixed: "right",
    width: "100px",
    render: (_, record) => (
      <div style={{ display: "flex" }}>
        <Button
          style={{ marginRight: "0.2em" }}
          icon={<ExpandOutlined />}
          onClick={() =>
            navigationStore.to(
              routes.data.entities.byId.path(store.sysName!, record.Id)
            )
          }
          title={`Открыть запись #${record.Id}`}
        />
        {config.methods.find((method) => method === EntityMethods.Delete) && (
          <Popconfirm
            title={`Запись #${record.Id} будет удалена`}
            onConfirm={() => store.deleteEntity(Number(record.Id))}
          >
            <Button
              style={{ marginRight: "0.2em", color: "red" }}
              icon={<DeleteOutlined />}
              loading={store.loading}
              title={`Удалить запись #${record.Id}`}
            />
          </Popconfirm>
        )}
      </div>
    ),
  };
  return [...dataColumns, actionColumn];
};

const getVisibleTableColumns = (
  columns: ColumnType<any>[],
  store: EntityStore
) => {
  return columns.filter(
    (column) =>
      !isDefined(column.key) ||
      !isDefined(store.visibleFields) ||
      store.visibleFields.includes(column.key as string /*systemName*/)
  );
};

const EntitiesTable: FC<EntitiesTableProps> = ({ store }) => {
  const { navigationStore } = useStores();
  const { typesResolver } = useEntityContext();

  const [filteredInfo, setFilteredInfo] = useState<
    Record<string, FilterValue | null>
  >({});
  const [sortedInfo, setSortedInfo] = useState<SorterResult<any>>({});
  const [paginationInfo, setPaginationInfo] = useState<TablePaginationConfig>(
    {}
  );

  const allTableColumns = useMemo(() => {
    if (store.config.hasValue) {
      console.log("render columns");
      return getAllTableColumns(
        store.config.value,
        store,
        typesResolver,
        navigationStore,
        filteredInfo,
        sortedInfo
      );
    }
    return [];
  }, [store.config.value, store.config.hasValue, filteredInfo, sortedInfo]);

  const tableColumns = useMemo(() => {
    console.log("apply/change visibility columns");
    return getVisibleTableColumns(allTableColumns, store);
  }, [allTableColumns, store.visibleFields]);

  const onTableChange = (
    pagination: TablePaginationConfig,
    filters: Record<string, FilterValue | null>,
    sorter: SorterResult<any> | SorterResult<any>[],
    extra: TableCurrentDataSource<any>
  ) => {
    setFilteredInfo(filters);
    setSortedInfo(sorter as SorterResult<any>);
    setPaginationInfo(pagination);
    store.setHandlers(buildGraphQLHandlers(filters, sorter, pagination));
  };

  const onReset = () => {
    setFilteredInfo({});
    setSortedInfo({});
    setPaginationInfo({});
  };

  return (
    <Table
      title={() => <EntitiesHeader store={store} onReset={onReset} />}
      loading={store.entities.loading}
      columns={tableColumns}
      rowKey={(entity) => entity.Id}
      dataSource={store.entities.items}
      pagination={{
        ...store.handlers.paginationHandler,
        showSizeChanger: true,
        showTotal: (total, range) =>
          `${range[0]}-${range[1]} из ${total} записей`,
      }}
      onChange={onTableChange}
      bordered
      scroll={{
        x: "100vw",
      }}
    />
  );
};

export default observer(EntitiesTable);
