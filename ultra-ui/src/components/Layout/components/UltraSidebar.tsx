import React, { FC, useEffect, useMemo, useState } from "react";
import Sider from "antd/lib/layout/Sider";
import { Badge, Menu, MenuProps } from "antd";
import { Link, useLocation } from "react-router-dom";

import {
  FileOutlined,
  DatabaseOutlined,
  StarOutlined,
  EnvironmentOutlined,
  MessageOutlined,
  EllipsisOutlined,
  SettingOutlined,
  UsergroupAddOutlined,
  DatabaseFilled,
  StarFilled,
  MessageFilled,
  EnvironmentFilled,
  SettingFilled,
  FileFilled,
} from "@ant-design/icons";
import { observer } from "mobx-react";
import { useStores } from "src/stores";
import { AppPage } from "src/stores/app-store/app-store.types";
import { routes } from "src/consts";

const iconStyles: React.CSSProperties = {
  color: "white",
  fontSize: "18px",
};

type MenuItem = Required<MenuProps>["items"][number];

interface MenuItemProps {
  key: React.Key;
  label: React.ReactNode;
  children?: MenuItem[];
  icon?: React.ReactNode;
  secondaryIcon?: React.ReactNode;
}

function getItem(props: MenuItemProps, selectedKey?: string): MenuItem {
  return {
    key: props.key,
    label: props.label,
    icon:
      selectedKey === props.key && props.secondaryIcon
        ? props.secondaryIcon
        : props.icon,
    children: props.children,
  };
}

const getItems = (
  collapsed: boolean,
  selectedKey: AppPage | string | undefined,
  unreadMessagesCount: number
): MenuItem[] => [
  getItem(
    {
      key: AppPage.Data,
      label: <Link to={routes.data.path}>Данные</Link>,
      icon: <DatabaseOutlined style={iconStyles} />,
      secondaryIcon: <DatabaseFilled style={iconStyles} />,
    },
    selectedKey
  ),
  getItem(
    {
      key: AppPage.Favorites,
      label: <Link to={routes.favorites.path}>Избранное</Link>,
      icon: <StarOutlined style={iconStyles} />,
      secondaryIcon: <StarFilled style={iconStyles} />,
    },
    selectedKey
  ),
  getItem({
    key: AppPage.Chat,
    label: (
      <Link to={routes.chat.path}>
        {collapsed && unreadMessagesCount !== 0
          ? `Непрочитанных сообщений: ${unreadMessagesCount}`
          : "Чат"}
      </Link>
    ),
    icon: (
      <Badge
        count={unreadMessagesCount}
        size="small"
        offset={collapsed ? [0, 9] : undefined}
      >
        {selectedKey !== AppPage.Chat ? (
          <MessageOutlined style={iconStyles} />
        ) : (
          <MessageFilled style={iconStyles} />
        )}
      </Badge>
    ),
  }),
  getItem(
    {
      key: AppPage.Map,
      label: <Link to={routes.map.path}>Карта</Link>,
      icon: <EnvironmentOutlined style={iconStyles} />,
      secondaryIcon: <EnvironmentFilled style={iconStyles} />,
    },
    selectedKey
  ),
  getItem(
    {
      key: AppPage.Files,
      label: <Link to={routes.files.path}>Файлы</Link>,
      icon: <FileOutlined style={iconStyles} />,
      secondaryIcon: <FileFilled style={iconStyles} />,
    },
    selectedKey
  ),
  getItem({
    key: "sub1",
    label: "",
    icon: <EllipsisOutlined />,
    children: [
      getItem(
        {
          key: AppPage.Settings,
          label: <Link to={routes.settings.path}>Настройки</Link>,
          icon: <SettingOutlined />,
          secondaryIcon: <SettingFilled />,
        },
        selectedKey
      ),
      getItem(
        {
          key: AppPage.Users,
          label: <Link to={routes.users.path}>Пользователи</Link>,
          icon: <UsergroupAddOutlined />,
        },
        selectedKey
      ),
    ],
  }),
];

const UltraSidebar: FC = () => {
  const { chatStore, appStore } = useStores();
  const [collapsed, setCollapsed] = useState(false);
  const [selectedKey, setSelectedKey] = useState("");

  const items = useMemo(
    () => getItems(collapsed, selectedKey, chatStore.unreadMessagesCount),
    [collapsed, selectedKey, chatStore.unreadMessagesCount]
  );

  useEffect(() => {
    setSelectedKey(appStore.currentPage.page);
  }, [appStore.currentPage.page]);

  return (
    <Sider
      collapsible
      collapsed={collapsed}
      onCollapse={(value) => setCollapsed(value)}
    >
      <Menu
        selectedKeys={selectedKey ? [selectedKey] : undefined}
        theme="dark"
        items={items}
        mode="inline"
        inlineCollapsed={collapsed}
      ></Menu>
    </Sider>
  );
};

export default observer(UltraSidebar);
