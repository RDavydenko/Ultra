import { ExpandOutlined } from "@ant-design/icons";
import { Button, Col, Row } from "antd";
import { observer } from "mobx-react";
import React, { FC, useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { InputProps } from "src/infrastructure/interfaces";
import { useStores } from "src/stores";
import { isDefined } from "src/utils";
import DebounceSelect, { useSelect } from "../components/DebounceSelect";
import { routes } from "src/consts";

const ParentReferenceInput: FC<InputProps & { referenceTo?: string }> = ({
  provider,
  config,
  disabled,
  referenceTo,
}) => {
  const { controller } = useSelect();
  const { collectionStore } = useStores();
  const [value, setValue] = useState<number | null>(null);

  const foreignSysName = config.meta["foreignKey.type"];
  const foreignDisplayNamePath = config.meta["foreignKey.displayable"];

  provider.getValue = () => controller.getValue()[0];
  provider.setValue = (val: number) => controller.setValue(val);

  controller.subscribeOnChange((value) => setValue(value));

  const required = config.meta?.["validation.required"] ?? false;

  const search = async (q?: string) => {
    const entities = await collectionStore.fetchEntities(
      foreignSysName,
      foreignDisplayNamePath,
      q
    );

    return entities.map((item) => {
      return {
        value: item.Id,
        label: item.DisplayName,
      };
    });
  };

  const searchById = async (id: number) => {
    if (isDefined(referenceTo)) {
      const reference = provider.root.getValue(referenceTo);
      return reference?.DisplayName;
    }
    return undefined;
  };

  return (
    <Row gutter={2}>
      <Col span={23}>
        <DebounceSelect
          controller={controller}
          style={{ width: "100%" }}
          placeholder={`Выберите ${config.displayName ?? config.systemName}`}
          disabled={disabled}
          fetchByClick
          fetchOptions={(q) => search(q)}
          fetchDisplayName={(id) => searchById(id)}
          showSearch
          allowClear
        />
      </Col>

      <Col span={1}>
        {isDefined(value) ? (
          <Link
            to={routes.data.entities.byId.path(foreignSysName, value!)}
            target="_blank"
            rel="noopener noreferrer"
          >
            <Button
              style={{ marginRight: "0.2em" }}
              icon={<ExpandOutlined />}
              title={`Открыть запись #${value}`}
            ></Button>
          </Link>
        ) : (
          <Button
            style={{ marginRight: "0.2em" }}
            icon={<ExpandOutlined />}
            disabled
          ></Button>
        )}
      </Col>
    </Row>
  );
};

export default observer(ParentReferenceInput);
