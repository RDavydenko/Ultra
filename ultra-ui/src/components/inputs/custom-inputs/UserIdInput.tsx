import { ExpandOutlined } from "@ant-design/icons";
import { Button, Col, Row } from "antd";
import _ from "lodash";
import { observer } from "mobx-react";
import React, { FC, useState } from "react";
import { Link } from "react-router-dom";
import { InputProps } from "src/infrastructure/interfaces";
import { useStores } from "src/stores";
import { isDefined } from "src/utils";
import DebounceSelect, { useSelect } from "../components/DebounceSelect";
import { routes } from "src/consts";

const UserIdInput: FC<InputProps> = ({ provider, config, disabled }) => {
  const { controller } = useSelect();
  const { collectionStore } = useStores();
  const [value, setValue] = useState<number | null>(null);
  const [label, setLabel] = useState<string | undefined>();

  // const sysName = config.meta["foreignKey.type"];
  // const displayNamePath = config.meta["foreignKey.displayable"];

  controller.subscribeOnChange((value, label) => {
    setValue(value);
    setLabel(label ?? value?.toString());
  });

  provider.getValue = () => controller.getValue()[0];
  provider.setValue = (val: number) => controller.setValue(val);

  const required = config.meta?.["validation.required"] ?? false;

  const searchById = async (userId: number) => {
    const users = await collectionStore.fetchUsers({ id: userId });
    return users?.[0]?.userName;
  };

  const searchByUserName = async (q?: string) => {
    const users = await collectionStore.fetchUsers({ q });
    return users.map((user) => {
      return {
        value: user.id,
        label: user.userName,
      };
    });
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
          fetchOptions={(q) => searchByUserName(q)}
          fetchDisplayName={(id) => searchById(id)}
          showSearch
          allowClear
        />
      </Col>

      <Col span={1}>
        {isDefined(value) && isDefined(label) ? (
          <Link
            to={routes.users.byId.path(value!)}
            target="_blank"
            rel="noopener noreferrer"
          >
            <Button
              style={{ marginRight: "0.2em" }}
              icon={<ExpandOutlined />}
              title={`Открыть пользователя ${label}`}
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

export default observer(UserIdInput);
