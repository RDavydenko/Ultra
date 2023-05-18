import { observer } from "mobx-react";
import React, { FC, useEffect, useState } from "react";
import { Modal, Transfer } from "antd";
import { EntityStore } from "src/stores";
import { isDefined } from "src/utils";
import { useEntityContext } from "src/infrastructure";

interface RecordType {
  key: string;
  title: string;
  chosen: boolean;
}

interface TableVisibleColumnsModalProps {
  store: EntityStore;
  open: boolean;
  setClose: () => void;
  onChangeSeleted?: (keys: string[]) => void;
  cacheKeyFactory?: () => string;
}

const TableVisibleColumnsModal: FC<TableVisibleColumnsModalProps> = ({
  store,
  open,
  setClose,
  onChangeSeleted,
}) => {
  const { typesResolver } = useEntityContext();
  const [allFields, setAllFields] = useState<RecordType[]>([]);
  const [selectedSystemNames, setSelectedSystemNames] = useState<string[]>([]);

  const loadFields = () => {
    const tempSelectedSystemNames = [];
    const allFieldsData = [];
    for (const field of store.config.value.fields.filter((field) =>
      typesResolver.isVisibleInListView(field)
    )) {
      const data = {
        key: field.systemName,
        title: field.displayName ?? field.systemName,
        chosen:
          !isDefined(store.visibleFields) ||
          store.visibleFields.includes(field.systemName),
      };
      if (data.chosen) {
        tempSelectedSystemNames.push(data.key);
      }
      allFieldsData.push(data);
    }
    setAllFields(allFieldsData);
    setSelectedSystemNames(tempSelectedSystemNames);
  };

  useEffect(() => {
    if (store.config.loaded) {
      loadFields();
    }
  }, [store.config.loaded, store.config.value?.fields, store.visibleFields]);

  const handleSubmit = () => {
    onChangeSeleted?.(selectedSystemNames);
    setClose();
  };

  return (
    <Modal
      title="Скрыть/показать столбцы"
      open={open}
      onCancel={setClose}
      onOk={handleSubmit}
      width="650px"
    >
      <Transfer
        dataSource={allFields}
        listStyle={{
          width: 250,
          height: 300,
        }}
        titles={["Скрытые столбцы", "Видимые столбцы"]}
        operations={["показать", "скрыть"]}
        targetKeys={selectedSystemNames}
        onChange={(selectedKeys) => setSelectedSystemNames(selectedKeys)}
        render={(item) => item.title}
      />
    </Modal>
  );
};

export default observer(TableVisibleColumnsModal);
