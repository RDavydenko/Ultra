import { Avatar, Card, Col, Row, Typography } from "antd";
import { observer } from "mobx-react";
import moment from "moment";
import React, { FC, useCallback, useEffect, useMemo } from "react";
import { MessageFullModel } from "src/models";
import { useStores } from "src/stores";

import "./ChatMessage.scss";
import CheckmarkIcon from "src/components/Icons/CheckmarkIcon";

interface ChatMessageProps {
  message: MessageFullModel;
}

const ChatMessage: FC<ChatMessageProps> = ({ message }) => {
  const { userStore } = useStores();

  const isSender = useMemo(() => {
    return userStore.user.value?.id === message.sendUserId;
  }, [userStore.user.value, message.sendUserId]);

  const doubleCheckmark = useMemo(() => {
    return isSender && (message.received || message.read);
  }, [isSender, message.received, message.read]);

  const blueCheckmark = useMemo(() => {
    return isSender && message.read;
  }, [isSender, message.read]);

  const checkmarkLabel = useMemo(() => {
    if (message.read) return "Прочитано";
    if (message.received) return "Получено";
    return "Отправлено";
  }, [message.read, message.received]);

  const formatDate = (date: Date) => {
    return moment(date).format("HH:mm");
  };

  if (isSender) {
    return (
      <div className="message-row my-message">
        <div className="message my-message">
          <div className="message__header">
            <div className="message__header__date">
              {formatDate(message.sendDate)}
            </div>
            <div className="message__header__markers">
              <div title={checkmarkLabel}>
                <CheckmarkIcon
                  size={16}
                  doubled={doubleCheckmark}
                  style={{
                    fill: blueCheckmark ? "#53bdeb" : "#949494",
                  }}
                />
              </div>
            </div>
          </div>
          <div className="message__body">
            <div className="message__body__text">{message.text}</div>
          </div>
        </div>
      </div>
    );
  } else {
    return (
      <div className="message-row">
        <Avatar className="message-avatar" />
        <div className="message">
          <div className="message__header">
            <div className="message__header__user-name">
              {message.sendUserName}
            </div>
            <div className="message__header__date">
              {formatDate(message.sendDate)}
            </div>
          </div>
          <div className="message__body">
            <div className="message__body__text">{message.text}</div>
          </div>
        </div>
      </div>
    );
  }
};

export default observer(ChatMessage);
