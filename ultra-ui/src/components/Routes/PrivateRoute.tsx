import { observer } from "mobx-react";
import React, { FC } from "react";
import { Navigate } from "react-router-dom";
import { Action, Entity, Role, routes } from "src/consts";
import { useStores } from "src/stores";
import ForbiddenPlaceholder from "../Placeholders/ForbiddenPlaceholder";
import UnauthorizedPlaceholder from "../Placeholders/UnauthorizedPlaceholder";

interface PrivateRouteProps {
  redirectTo?: string;
  element: React.ReactElement;
  role?: Role;
  entity?: Entity;
  action?: Action;
}

const PrivateRoute: FC<PrivateRouteProps> = ({
  element,
  redirectTo,
  role,
  entity,
  action,
}) => {
  const mode: string = "new";
  const { userStore } = useStores();
  const isAuth = () => {
    if (!userStore.isAuth) {
      return false;
    }
    if (role && !userStore.hasRole(role)) {
      return false;
    }
    if (entity && action && !userStore.hasPermission(entity, action)) {
      return false;
    }
    return true;
  };

  if (mode === "old") {
    return !isAuth() ? (
      <Navigate
        to={redirectTo ?? (window.location.href = routes.login.path)}
        replace
      />
    ) : (
      element
    );
  } else {
    return !isAuth() ? (
      !userStore.isAuth ? (
        <UnauthorizedPlaceholder />
      ) : (
        <ForbiddenPlaceholder />
      )
    ) : (
      element
    );
  }
};

export default observer(PrivateRoute);
