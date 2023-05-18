import { Col, Row } from "antd";
import { observer } from "mobx-react";
import React, { FC } from "react";
import ChatArea from "./ChatArea";

import "./ChatBox.scss";
import ChatForm from "./ChatForm";
import ChatHeader from "./ChatHeader";

const ChatBox: FC = () => {
  return (
    <div className="chat-box">
      <ChatHeader />
      <Row>
        <Col span={3} />
        <Col span={18} style={{ height: "calc(100vh - 64px - 40px - 60px)" }}>
          <ChatArea />
          <ChatForm />
        </Col>
        <Col span={3} />
      </Row>
    </div>
  );
};

export default observer(ChatBox);
