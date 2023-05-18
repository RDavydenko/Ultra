import { observer } from "mobx-react";
import React, { FC, useEffect, useMemo, useState } from "react";
import InfiniteScrollReverse from "src/components/InfiniteScroll/Reverse/InfiniteScrollReverse";
import { MessageFullModel } from "src/models";
import { CollectionState, useStores } from "src/stores";
import { delay, isDefined } from "src/utils";
import ChatMessage from "./ChatMessage";

const ChatArea: FC = () => {
  const { chatStore } = useStores();
  const [page, setPage] = useState(2);
  const [scrollableRef, secScrollableRef] = useState<
    HTMLDivElement | undefined
  >();

  useEffect(() => {
    if (scrollableRef) {
      setTimeout(() => {
        scrollableRef.scrollIntoView({
          block: "end",
          inline: "end",
        });
      }, 50);
    }
  }, [chatStore.channelId]);

  // useEffect(() => {
  //   setMessages(
  //     isDefined(chatStore.channelId)
  //       ? chatStore.channelMessages[chatStore.channelId]?.items ?? []
  //       : []
  //   );
  // }, [chatStore.channelId, chatStore.channelMessages]);

  const loadMoreData = () => {
    if (chatStore.channel && chatStore.channelsLoading) {
      return;
    }

    chatStore.fetchMessages(page).then(() => {
      setPage(page + 1);
    });
  };

  return (
    <InfiniteScrollReverse
      setRef={secScrollableRef}
      style={{
        height: "calc(100vh - 64px - 40px - 60px - 85px)",
        overflowY: "scroll",
        /*display: "flex",
        flexDirection: "column",
        justifyContent: "flex-end",*/
      }}
      isLoading={chatStore.messagesLoading}
      loadMore={() => loadMoreData()}
      hasMore={
        isDefined(chatStore.channel) &&
        isDefined(chatStore.channel.messages) &&
        isDefined(chatStore.channel.messagesTotalCount) &&
        chatStore.channel.messages.length < chatStore.channel.messagesTotalCount
      }
      // loader={
      //   <>
      //     <Skeleton paragraph={{ rows: 1 }} active />
      //     <Skeleton paragraph={{ rows: 2 }} active />
      //     <Skeleton paragraph={{ rows: 3 }} active />
      //     <Skeleton paragraph={{ rows: 1 }} active />
      //     <Skeleton paragraph={{ rows: 3 }} active />
      //   </>
      // }
    >
      {chatStore.channel?.messages?.map((item) => (
        <ChatMessage key={item.guid} message={item} />
      ))}
    </InfiniteScrollReverse>
  );
};

export default observer(ChatArea);
