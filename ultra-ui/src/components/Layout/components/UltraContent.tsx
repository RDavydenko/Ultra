import React, { FC, useEffect } from "react";
import { Content } from "antd/lib/layout/layout";
import { Routes, Route } from "react-router-dom";
import EmptyPage from "src/pages/EmptyPage";
import NotFoundPage from "src/pages/NotFoundPage";
import DataPage from "src/pages/DataPage";
import EntityPage from "src/pages/EntityPage";
import PrivateRoute from "src/components/Routes/PrivateRoute";
import { Entity, Action, routes } from "src/consts";
import { useStores } from "src/stores";
import MapPage from "src/pages/MapPage";
import ChatPage from "src/pages/ChatPage";

const UltraContent: FC = () => {
  const { userStore } = useStores();
  useEffect(() => {
    userStore.checkAuth();
  });

  return (
    <Content style={{ minHeight: 250 }}>
      <Routes>
        <Route path={routes.home.route} element={<EmptyPage />} />
        <Route
          path={routes.data.entities.byId.route}
          element={
            <PrivateRoute
              entity={Entity.Data}
              action={Action.View}
              element={<EntityPage />}
            />
          }
        />
        <Route
          path={routes.data.entities.route}
          element={
            <PrivateRoute
              entity={Entity.Data}
              action={Action.View}
              element={<EntityPage />}
            />
          }
        />
        <Route
          path={routes.data.route}
          element={
            <PrivateRoute
              entity={Entity.Data}
              action={Action.View}
              element={<DataPage />}
            />
          }
        />
        <Route
          path={routes.map.route}
          element={
            <PrivateRoute
              entity={Entity.Map}
              action={Action.View}
              element={<MapPage />}
            />
          }
        />
        <Route
          path={routes.chat.route}
          element={
            <PrivateRoute
              entity={Entity.Chat}
              action={Action.View}
              element={<ChatPage />}
            />
          }
        />
        <Route path="*" element={<NotFoundPage />} />
      </Routes>
    </Content>
  );
};

export default UltraContent;
