import React, { FC, useEffect } from "react";
import { Modal, Spin } from "antd";
import { EntityMethods } from "src/models";
import { EntityStore } from "src/stores/entity-store/entity-store";
import { observer } from "mobx-react";
import { useForm } from "../Form";
import Form from "../Form/Form";
import DetailViewEntityHeader from "./DetailViewEntity.Header";
import { useStores } from "src/stores";
import { routes } from "src/consts";

interface DetailViewEntityProps {
  store: EntityStore;
}

const getOkTitle = (mode?: string) => {
  return mode === EntityMethods.Create
    ? "Создать"
    : mode === EntityMethods.Update
    ? "Обновить"
    : "";
};

const submit = (
  store: EntityStore,
  mode: string | undefined,
  validate: () => boolean,
  getValues: () => any,
  createdCallback?: (id: number) => void
) => {
  console.log("submit", mode, getValues());
  if (mode === EntityMethods.Create) {
    validate() && store.createEntity(getValues(), createdCallback);
  } else if (mode === EntityMethods.Update) {
    validate() && store.updateEntity(getValues());
  }
};

const DetailViewEntity: FC<DetailViewEntityProps> = ({ store }) => {
  const { navigationStore } = useStores();
  const { form } = useForm();

  useEffect(() => {
    form.reset();
    if (store.entity.hasValue) {
      form.setValues(store.entity.value);
    }
  }, [form, store.entity.hasValue, store.entity.value, store.config.value]);

  return store.detailView ? (
    <Modal
      open={store.detailView}
      width={800}
      title={<DetailViewEntityHeader store={store} />}
      okText={getOkTitle(store.detailType)}
      onOk={() =>
        submit(store, store.detailType, form.validate, form.getValues, (id) =>
          navigationStore.to(
            routes.data.entities.byId.path(store.sysName!, id),
            { replace: true }
          )
        )
      }
      onCancel={() => {
        store.closeDetail();
        navigationStore.to(routes.data.entities.path(store.sysName!));
      }}
    >
      <Spin spinning={!store.configurationLoaded || store.entity.loading}>
        <Form
          mode={store.detailType}
          form={form}
          fields={store.config.value.fields ?? []}
          entityConfig={store.config.value}
        />
      </Spin>
    </Modal>
  ) : (
    <></>
  );
};

export default observer(DetailViewEntity);
