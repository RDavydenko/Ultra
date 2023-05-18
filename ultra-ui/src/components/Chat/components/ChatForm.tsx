import { LinkOutlined, SendOutlined } from "@ant-design/icons";
import { Button, Input } from "antd";
import { observer } from "mobx-react";
import React, { FC, useState } from "react";
import { useStores } from "src/stores";
import { isDefined } from "src/utils";

const ChatForm: FC = () => {
  const { chatStore } = useStores();
  const [text, setText] = useState<string>("");

  const send = () => {
    const model = {
      text: text,
    };
    setText("");
    chatStore.sendMessage(model);
  };

  const canSend = () => text.trim().length !== 0;

  return (
    <div style={{ position: "absolute", bottom: "0", width: "100%" }}>
      <Input.TextArea
        value={text}
        onChange={(e) => setText(e.target.value)}
        disabled={!isDefined(chatStore.channelId)}
        placeholder="Введите новое сообщение"
        autoSize={{ minRows: 2, maxRows: 6 }}
        onPressEnter={(e) => {
          if (e.ctrlKey) {
            setText(text + "\n");
          } else if (canSend()) {
            send();
            e.preventDefault();
            setText("");
          }
        }}
      />
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          backgroundColor: "#E4E6E8",
        }}
      >
        <div>
          <Button
            style={{ backgroundColor: "#E4E6E8", border: "none" }}
            disabled={!isDefined(chatStore.channelId)}
            icon={<LinkOutlined />}
          />
        </div>
        <div>
          <Button
            style={{ backgroundColor: "#E4E6E8", border: "none" }}
            disabled={!isDefined(chatStore.channelId)}
            onClick={() => canSend() && send()}
            icon={<SendOutlined />}
          />
        </div>
      </div>
    </div>
  );
};

export default observer(ChatForm);
