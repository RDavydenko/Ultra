import { SearchOutlined } from "@ant-design/icons";
import { Button } from "antd";
import React, { FC, useMemo } from "react";

interface EntitySearchButtonProps {
  onClick?: () => void;
}

const EntitySearchButton: FC<EntitySearchButtonProps> = ({ onClick }) => {
  const element = useMemo(
    () => <Button onClick={onClick} size="large" icon={<SearchOutlined />} />,
    [onClick]
  );

  return (
    <div
      className="leaflet-bottom leaflet-right"
      style={{ marginBottom: "10px" }}
    >
      <div className="leaflet-control leaflet-bar">{element}</div>
    </div>
  );
};

export default EntitySearchButton;
