import React, { FC, useState } from "react";
import { Header } from "antd/lib/layout/layout";
import { Badge, Menu } from "antd";
import "./UltraHeader.css";
import SubMenu from "antd/lib/menu/SubMenu";
import { useStores } from "src/stores";
import {
  BellOutlined,
  ExportOutlined,
  NotificationOutlined,
  SettingOutlined,
  UserOutlined,
} from "@ant-design/icons";
import { Link } from "react-router-dom";
import { observer } from "mobx-react";
import { routes } from "src/consts";

const UltraHeader: FC = () => {
  const { userStore } = useStores();
  const [notifiesCount, setNotifiesCount] = useState(10);

  const logout = () => {
    userStore.logout();
    window.location.href = routes.login.path;
  };

  return (
    <Header>
      <div className="logo-2">
        <h3>Ultra</h3>
      </div>
      <Menu
        style={{ display: "block", color: "white" }}
        theme="dark"
        mode="horizontal"
        selectable={false}
        items={[
          {
            key: "user",
            style: { float: "right" },
            icon: <UserOutlined style={{ fontSize: "18px" }} />,
            label: userStore.user.value?.userName,
            theme: "light",
            children: [
              {
                key: "settings",
                icon: <SettingOutlined />,
                label: <Link to={routes.settings.path}>Настройки</Link>,
              },
              {
                key: "logout",
                icon: <ExportOutlined />,
                label: "Выйти",
                onClick: logout,
              },
            ],
          },
          {
            key: "notifications",
            style: { float: "right" },
            label: (
              <Badge
                size="small"
                count={notifiesCount}
                overflowCount={99}
                offset={notifiesCount >= 10 ? [8, 0] : undefined}
              >
                <BellOutlined style={{ color: "white", fontSize: "18px" }} />
              </Badge>
            ),
          },
        ]}
      />
    </Header>
  );
};

export default observer(UltraHeader);
