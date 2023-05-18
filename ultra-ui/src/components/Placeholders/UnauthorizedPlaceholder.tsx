import { Button, Result } from "antd";
import React, { FC } from "react";

const UnauthorizedPlaceholder: FC = () => {
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
        title="401"
        subTitle="Вы не авторизованы в системе."
        extra={
          <Button
            type="primary"
            onClick={() => (window.location.href = "/login")}
          >
            Вход
          </Button>
        }
      />
    </div>
  );
};

export default UnauthorizedPlaceholder;
