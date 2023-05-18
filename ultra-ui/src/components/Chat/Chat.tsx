import React, { FC, useEffect } from "react";
import { Col, Row } from "antd";
import ChatList from "./components/ChatList";

import "./Chat.scss";
import ChatBox from "./components/ChatBox";

const Chat: FC = () => {
  return (
    <Row>
      <Col span={6}>
        <ChatList />
      </Col>
      <Col span={18}>
        <ChatBox />
      </Col>
    </Row>
  );
};

export default Chat;
