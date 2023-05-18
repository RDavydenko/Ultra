import { Button, Result } from "antd";
import React, { FC } from "react";
import { useStores } from "src/stores";

const ForbiddenPlaceholder: FC = () => {
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
        status="403"
        title="403"
        subTitle="У вас нет прав для просмотра этой страницы."
        extra={
          <Button type="primary" onClick={() => navigationStore.back(1)}>
            Назад
          </Button>
        }
      />
    </div>
  );
};

export default ForbiddenPlaceholder;
