import { observer } from "mobx-react";
import React, { FC, useEffect } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { useStores } from "src/stores";
import { isDefined } from "src/utils";

const Navigation: FC = () => {
  const navigate = useNavigate();
  const { pathname } = useLocation(); // contains relative link (/d/House/1 or /favorites)
  const { navigationStore } = useStores();

  useEffect(() => {
    if (isDefined(navigationStore.props)) {
      navigate(navigationStore.props.to, navigationStore.props.params);
    } else if (isDefined(navigationStore.delta)) {
      navigate(navigationStore.delta);
    }
  }, [navigationStore.props, navigationStore.delta]);

  useEffect(() => {
    navigationStore.setPathInternal(pathname);
  }, [pathname]);

  return <></>;
};

export default observer(Navigation);
