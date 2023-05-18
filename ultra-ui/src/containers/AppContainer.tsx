import React, { FC } from "react";
import UltraLayout from "src/components/Layout/UltraLayout";
import { Routes, Route } from "react-router-dom";
import LoginPage from "src/pages/LoginPage";
import { routes } from "src/consts";

const AppContainer: FC = () => {
  return (
    <Routes>
      <Route path={routes.login.route} element={<LoginPage />} />
      <Route path="*" element={<UltraLayout />} />
    </Routes>
  );
};

export default AppContainer;
