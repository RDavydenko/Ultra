import React, { FC, useEffect, useMemo, useState } from "react";
import { Avatar, Divider, List, Skeleton } from "antd";
import InfiniteScroll from "react-infinite-scroll-component";
import moment from "moment";

import "./ChatList.scss";
import classNames from "classnames";
import { useStores } from "src/stores";
import { observer } from "mobx-react";

const ChatList: FC = () => {
  const { chatStore } = useStores();
  const [page, setPage] = useState(1);

  const selectedChannelId = useMemo(
    () => chatStore.channelId,
    [chatStore.channelId]
  );

  const loadMoreData = () => {
    if (chatStore.channelsLoading) {
      return;
    }
    chatStore.fetchChannels(page);
    setPage(page + 1);
  };

  const formatDate = (date?: Date) => {
    return date ? moment(date).format("HH:mm") : "";
  };

  const formatUnreadCount = (count: number) => {
    if (count > 99) return "99";
    return count.toString();
  };

  useEffect(() => {
    loadMoreData();
  }, []);

  return (
    <div
      id="scrollableDiv"
      style={{
        height: "calc(100vh - 64px - 40px)",
        overflow: "auto",
        border: "1px solid rgba(140, 140, 140, 0.35)",
      }}
    >
      <InfiniteScroll
        dataLength={chatStore.channels.length}
        next={loadMoreData}
        hasMore={chatStore.channels.length < chatStore.channelsTotalCount}
        loader={
          <>
            <Skeleton avatar paragraph={{ rows: 1 }} active />
            <Skeleton avatar paragraph={{ rows: 1 }} active />
            <Skeleton avatar paragraph={{ rows: 1 }} active />
            <Skeleton avatar paragraph={{ rows: 1 }} active />
          </>
        }
        scrollableTarget="scrollableDiv"
      >
        <List
          className="list-without-empty"
          dataSource={chatStore.channels}
          loading={chatStore.channelsLoading}
          renderItem={(item) => (
            <List.Item
              key={item.id}
              className={classNames("chat-list-item", {
                selected: item.id === selectedChannelId,
              })}
              onClick={() => chatStore.setChannelId(item.id)}
            >
              <List.Item.Meta
                avatar={<Avatar />}
                title={
                  <div
                    style={{ display: "flex", justifyContent: "space-between" }}
                  >
                    <div className="ellipsis-text">{item.name}</div>
                    <span>
                      {formatDate(
                        item.lastMessage?.sendDate ?? item.createDate
                      )}
                    </span>
                  </div>
                }
                description={
                  <div
                    style={{
                      display: "flex",
                      alignItems: "center",
                      justifyContent: "space-between",
                    }}
                  >
                    <div
                      className="ellipsis-text"
                      style={{ width: "calc(100% - 30px)" }}
                    >
                      {item.lastMessage?.text}
                    </div>
                    {item.unreadMessagesCount > 0 && (
                      <div
                        className={classNames("notification-count", {
                          silenced: item.silenced,
                        })}
                      >
                        {formatUnreadCount(item.unreadMessagesCount)}
                      </div>
                    )}
                  </div>
                }
              />
            </List.Item>
          )}
        />
      </InfiniteScroll>
    </div>
  );
};

export default observer(ChatList);
