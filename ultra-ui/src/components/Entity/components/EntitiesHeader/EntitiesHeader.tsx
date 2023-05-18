import {
  ClearOutlined,
  PlusOutlined,
  SettingOutlined,
  UndoOutlined,
} from "@ant-design/icons";
import { Button, Space } from "antd";
import { observer } from "mobx-react";
import { FC, useState } from "react";
import { EntityMethods } from "src/models";
import { EntityStore } from "src/stores";
import styles from "./EntitiesHeader.module.scss";
import TableVisibleColumnsModal from "../TableVisibleColumnsModal/TableVisibleColumnsModal";

interface EntitiesHeaderProps {
  store: EntityStore;
  onReset: () => void;
}

const getActions = (
  store: EntityStore,
  onReset: () => void,
  setSettingsModalVisible: (value: boolean) => void
) => [
  {
    component: (
      <Button
        key="entity_table_reset"
        icon={<UndoOutlined />}
        title="Сбросить"
        onClick={() => {
          onReset();
          store.resetHandlers();
        }}
        disabled={store.config.loading}
      >
        Сбросить
      </Button>
    ),
    visible: true,
  },
  {
    component: (
      <Button
        key="entity_table_settings"
        icon={<SettingOutlined />}
        title="Настройки"
        onClick={() => setSettingsModalVisible(true)}
        disabled={store.config.loading}
      />
    ),
    visible: true,
  },
  {
    component: (
      <Button
        key="entity_table_create"
        type="primary"
        icon={<PlusOutlined />}
        onClick={() => store.openDetail()}
        title="Создать новую запись"
      >
        Создать
      </Button>
    ),
    visible:
      store.config.hasValue &&
      store.config.value.methods.find(
        (method) => method === EntityMethods.Create
      ),
  },
];

const EntitiesHeader: FC<EntitiesHeaderProps> = ({ store, onReset }) => {
  const [settingsModalVisible, setSettingsModalVisible] = useState(false);
  const actions = getActions(store, onReset, setSettingsModalVisible);

  return (
    <>
      <div className={styles.entity_header}>
        <div className={styles.title}>
          {store.config.hasValue
            ? store.config.value.displayName ?? store.config.value.systemName
            : null}
        </div>
        <Space>
          {actions
            .filter((x) => x.visible)
            .map(({ component }) => {
              return component;
            })}
        </Space>
      </div>
      <TableVisibleColumnsModal
        store={store}
        open={settingsModalVisible}
        setClose={() => setSettingsModalVisible(false)}
        onChangeSeleted={(keys) => store.setVisibleFields(keys)}
      />
    </>
  );
};

export default observer(EntitiesHeader);
