import { Avatar, Typography } from "antd";
import { observer } from "mobx-react";
import React, { FC } from "react";
import { useStores } from "src/stores";
import { isDefined } from "src/utils";

import "./ChatHeader.scss";

const ChatHeader: FC = () => {
  const { chatStore } = useStores();

  return (
    <div className="chat-header">
      <div
        style={{ display: "flex", alignItems: "center", marginLeft: "10px" }}
      >
        {isDefined(chatStore.channel) && (
          <>
            <Avatar size="large" />
            <Typography.Title
              style={{ marginLeft: "0.5rem", lineHeight: "40px" }}
              level={4}
            >
              {chatStore.channel.name}
            </Typography.Title>
          </>
        )}
      </div>
    </div>
  );
};

export default observer(ChatHeader);
