import { Button, Result } from "antd";
import React, { FC } from "react";
import { useStores } from "src/stores";

const NotFoundPlaceholder: FC = () => {
  const { navigationStore } = useStores();

  return (
    <div
      style={{
        height: "100%",
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
      }}
    >
      <Result
        status="404"
        title="404"
        subTitle="Страница не найдена."
        extra={
          <Button type="primary" onClick={() => navigationStore.back(1)}>
            Назад
          </Button>
        }
      />
    </div>
  );
};

export default NotFoundPlaceholder;
