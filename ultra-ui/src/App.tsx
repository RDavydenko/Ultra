import React, { FC, useEffect } from "react";
import "./App.scss";
import { observer } from "mobx-react";
import { BrowserRouter } from "react-router-dom";
import { LoadingOutlined } from "@ant-design/icons";
import { Spin } from "antd";
import { ConfigProvider } from "antd";
import ruRU from "antd/es/locale/ru_RU";
import { useStores } from "./stores";
import AppContainer from "./containers/AppContainer";
import Navigation from "./components/Navigation/Navigation";

const App: FC = () => {
  const { configurationStore, signalRService, chatStore, userStore } =
    useStores();

  useEffect(() => {
    if (userStore?.isAuth === true) {
      chatStore.fetchUnreadMessagesCount();
    }

    return () => {
      signalRService?.dispose();
    };
  }, [signalRService, chatStore, userStore?.isAuth]);

  return (
    <Spin
      size="large"
      delay={0}
      spinning={configurationStore.loading}
      tip={<div style={{ paddingTop: 50, paddingLeft: 40 }}>Загрузка...</div>}
      indicator={
        <LoadingOutlined
          className="layoutSpinner"
          style={{ fontSize: 64 }}
          spin
        />
      }
    >
      <BrowserRouter basename="/">
        <Navigation />
        <ConfigProvider locale={ruRU}>
          {!configurationStore.loading && <AppContainer />}
        </ConfigProvider>
      </BrowserRouter>
    </Spin>
  );
};

export default observer(App);
