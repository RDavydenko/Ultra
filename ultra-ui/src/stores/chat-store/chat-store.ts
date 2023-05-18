import { makeAutoObservable, reaction, runInAction } from "mobx";
import {
  AppConfig,
  ChatFullModel,
  CollectionPage,
  EntityConfiguration,
  EntityMethods,
  EntityType,
  GraphQLHandlers,
  GraphQLOperator,
  MessageActionModel,
  MessageCorrelationModel,
  MessageFullModel,
  MessageModel,
  PageModel,
  SendMessageModel,
} from "src/models";
import { isDefined, performAction, removeAt, replaceAt } from "src/utils";
import { NotificationStore } from "../notification-store";
import ChatService from "src/services/chat-service/chat-service.types";
import { orderBy, uniqBy } from "lodash";
import { v4 as uuid } from "uuid";
import { UserStore } from "../user-store";
import moment from "moment";
import { ChatEvent, ChatSignalRService } from "src/services";
import { AppStore } from "../app-store";
import { AppPage } from "../app-store/app-store.types";

const CHANNELS_PAGE_SIZE = 20;
const MESSAGES_PAGE_SIZE = 20;

export class ChatStore {
  channelId?: number;

  channels: ChatFullModel[] = [];
  channelsLoading = false;
  channelsTotalCount = 0;

  messagesLoading = false;

  private realUnreadMessagesCount = 0;
  unreadMessagesCount = 0;

  constructor(
    private readonly notificationStore: NotificationStore,
    private readonly chatService: ChatService,
    private readonly userStore: UserStore,
    private readonly appStore: AppStore,
    chatSignalRService: ChatSignalRService
  ) {
    makeAutoObservable(this);

    reaction(
      () => this.channelId,
      () => {
        this.fetchMessages();
      }
    );

    reaction(
      () => this.appStore.currentPage.page,
      (page) => {
        if (page === AppPage.Chat && isDefined(this.channel)) {
          this.tryReceiveMessagesFromChannel(this.channel);
          this.tryReadMessagesFromChannel(this.channel);
        }
      }
    );

    chatSignalRService.subscribe(
      ChatEvent.OnReceivedMessage,
      this.onReceivedMessage
    );
    chatSignalRService.subscribe(
      ChatEvent.OnMarkReadMessage,
      this.onMarkReadMessage
    );
    chatSignalRService.subscribe(
      ChatEvent.OnMarkReceivedMessage,
      this.onMarkReceivedMessage
    );
  }

  get channel() {
    if (isDefined(this.channelId)) {
      return this.channels.find((x) => x.id === this.channelId);
    }
    return undefined;
  }

  setChannelId = (id: number) => (this.channelId = id);

  sendMessage = async (model: SendMessageModel) => {
    if (!isDefined(this.channelId) || !this.userStore.user.loaded) return;

    model.text = model.text.trim();
    model.correlationId = uuid();

    const message: MessageFullModel = {
      channelId: this.channelId!,
      guid: uuid(),
      sendDate: new Date(),
      sendUserId: this.userStore.user.value.id,
      sendUserName: this.userStore.user.value.userName,
      read: false,
      received: false,
      text: model.text,
      correlationId: model.correlationId,
    };

    this.createOrUpdateMessage(this.channelId!, message);
    this.updateUnreadMessagesCount(this.channelId!, { zero: true });

    try {
      const result = await this.chatService.sendMessage(this.channelId, model);

      if (result.isSuccess) {
        this.createOrUpdateMessage(this.channelId!, result.object!);
      }
    } catch (error) {}
  };

  fetchChannels = (pageNumber: number = 1) =>
    performAction({
      action: () =>
        this.chatService.getChannels({
          pageSize: CHANNELS_PAGE_SIZE,
          pageNumber: pageNumber,
        }),
      setLoading: (loading) => (this.channelsLoading = loading),
      onSuccess: (res) => {
        const items = this.sortChannels(
          uniqBy([...this.channels, ...res.object!.items], (x) => x.id)
        );
        this.channelsTotalCount = res.object!.pageInfo.totalCount;
        this.channels = items;
      },
      onError: (err) => {
        this.notificationStore.error("Произошла ошибка при загрузке чатов");
        console.log(err);
      },
    })();

  // fetchChannel = () =>
  //   fetchDataState({
  //     getDataState: () => this.channel,
  //     condition: () => isDefined(this.channelId),
  //     getData: () => this.chatService.getChannel(this.channelId!),
  //     onError: (err) => {
  //       this.notificationStore.error("Произошла ошибка при получении чата");
  //       console.log(err);
  //     },
  //   })();

  fetchMessages = (pageNumber: number = 1) =>
    performAction({
      condition: () => isDefined(this.channel),
      action: () =>
        this.chatService.getMessages(this.channelId!, {
          pageSize: MESSAGES_PAGE_SIZE,
          pageNumber: pageNumber,
        }),
      setLoading: (loading) => (this.messagesLoading = loading),
      onSuccess: (res) => {
        const messages = this.channel!.messages ?? [];

        const newMessages = this.sortMessages(
          uniqBy([...messages, ...res.object!.items], (x) => x.guid)
        );

        this.channel!.messages = newMessages;
        this.channel!.messagesTotalCount = res.object!.pageInfo.totalCount;

        this.tryReceiveMessagesFromChannel(this.channel!);
        this.tryReadMessagesFromChannel(this.channel!);
      },
      onError: (err) => {
        this.notificationStore.error("Произошла ошибка при загрузке чатов");
        console.log(err);
      },
    })();

  receiveChannelMessages = () =>
    performAction({
      condition: () => isDefined(this.channelId),
      action: () => this.chatService.receiveMessageFromChannel(this.channelId!),
      onError: (err) => {
        this.notificationStore.error(
          "Произошла ошибка при получении сообщений чата"
        );
        console.log(err);
      },
    })();

  readChannelMessages = () =>
    performAction({
      condition: () => isDefined(this.channelId),
      action: () => {
        this.updateUnreadMessagesCount(this.channelId!, { zero: true });
        return this.chatService.readMessageFromChannel(this.channelId!);
      },
      onError: (err) => {
        this.notificationStore.error(
          "Произошла ошибка при чтении сообщений чата"
        );
        console.log(err);
      },
    })();

  fetchUnreadMessagesCount = () =>
    performAction({
      action: () => this.chatService.getUnreadMessagesCount(),
      onSuccess: (res) => {
        this.realUnreadMessagesCount += res.object!;
        this.unreadMessagesCount =
          this.realUnreadMessagesCount < 0 ? 0 : this.realUnreadMessagesCount;
      },
      onError: (err) => {
        this.notificationStore.error(
          "Произошла ошибка при получении количества не прочитанных сообщений"
        );
        console.log(err);
      },
    })();

  private createOrUpdateMessage = (
    channelId: number,
    message: MessageFullModel | MessageCorrelationModel
  ) => {
    const channel = this.channels.find((x) => x.id === channelId);
    if (!isDefined(channel)) return;

    let messages = channel.messages ?? [];
    const updateIndex = messages.findIndex(
      (x) =>
        x.guid === message.guid ||
        (isDefined(x.correlationId) &&
          isDefined(message.correlationId) &&
          x.correlationId === message.correlationId)
    );
    let newMessages: MessageFullModel[] = [];
    let needIncrementTotal = false;
    if (updateIndex === -1) {
      // create
      newMessages = [...messages, message as MessageFullModel];
      needIncrementTotal = true;
    } else {
      // update
      const oldMessage = messages[updateIndex];
      newMessages = [
        ...removeAt(messages, updateIndex),
        { ...oldMessage, ...message },
      ];
    }

    runInAction(() => {
      if (needIncrementTotal && isDefined(channel.messagesTotalCount)) {
        channel.messagesTotalCount++;
      }
      messages = this.sortMessages(newMessages);
      const lastMessage = messages[messages.length - 1];
      channel!.lastMessage = lastMessage;
      channel!.messages = messages;

      this.channels = this.sortChannels(this.channels);
    });
  };

  private updateUnreadMessagesCount = (
    channelId: number,
    model: { inc?: number; zero?: boolean }
  ) => {
    const channel = this.channels.find((x) => x.id === channelId);
    if (!isDefined(channel)) {
      return;
    }

    runInAction(() => {
      let newRealUnreadMessagesCount = this.realUnreadMessagesCount;
      if (isDefined(model.zero)) {
        newRealUnreadMessagesCount -= channel.unreadMessagesCount;
        channel.unreadMessagesCount = 0;
      }
      if (isDefined(model.inc)) {
        newRealUnreadMessagesCount += model.inc;
        channel.unreadMessagesCount =
          channel.unreadMessagesCount + model.inc < 0
            ? 0
            : channel.unreadMessagesCount + model.inc;
      }
      this.realUnreadMessagesCount = newRealUnreadMessagesCount;
      this.unreadMessagesCount =
        this.realUnreadMessagesCount < 0 ? 0 : this.realUnreadMessagesCount;
      this.updateChannel(channel);
    });
  };

  private tryReceiveMessagesFromChannel = (channel: ChatFullModel) => {
    const otherMessages = (channel.messages ?? []).filter(
      (x) => x.sendUserId !== this.userStore.user.value.id
    );
    const needReceive = otherMessages.some((x) => !x.received);

    otherMessages.forEach((m) => {
      if (!m.received) {
        m.received = true;
        m.receivedDate = new Date();
      }
    });

    if (needReceive) {
      this.receiveChannelMessages();
      this.updateChannel(channel);
    }
  };

  private tryReadMessagesFromChannel = (channel: ChatFullModel) => {
    const otherMessages = (channel.messages ?? []).filter(
      (x) => x.sendUserId !== this.userStore.user.value.id
    );
    const needRead = otherMessages.some((x) => !x.read);

    otherMessages.forEach((m) => {
      if (!m.read) {
        m.read = true;
        m.readDate = new Date();
      }
    });

    if (needRead) {
      this.readChannelMessages();
      this.updateChannel(channel);
    }
  };

  private updateChannel = (channel: ChatFullModel) => {
    const index = this.channels.findIndex((x) => x.id === channel.id);
    if (index === -1) return;

    runInAction(() => {
      this.channels = this.sortChannels(
        replaceAt(this.channels, index, channel)
      );
    });
  };

  private updateMessage = (message: MessageFullModel) => {
    const channel = this.channels.find((x) => x.id === message.channelId);
    if (!isDefined(channel)) return;

    const index = channel.messages?.findIndex((x) => x.guid === message.guid);
    if (!isDefined(index) || index === -1) return;

    channel.messages = this.sortMessages(
      replaceAt(channel.messages!, index, message)
    );

    this.updateChannel(channel);
  };

  private onReceivedMessage = (model: MessageModel) => {
    if (model.sendUserId === this.userStore.user.value.id) {
      return;
    }

    this.chatService.receiveMessage(model.guid);

    this.createOrUpdateMessage(model.channelId, {
      ...model,
      correlationId: uuid(),
    });

    if (
      model.channelId !== this.channelId ||
      this.appStore.currentPage.page !== AppPage.Chat
    ) {
      this.updateUnreadMessagesCount(model.channelId, { inc: 1 });
    } else {
      this.chatService.readMessage(model.guid);
    }
  };

  private onMarkReceivedMessage = (model: MessageActionModel) => {
    const message = this.channels
      .find((x) => x.id === model.channelId)
      ?.messages?.find((x) => x.guid === model.guid);
    if (!isDefined(message)) return;

    if (!message.received) {
      message.received = true;
      message.receivedDate = new Date();
      this.updateMessage(message);
    }
  };

  private onMarkReadMessage = (model: MessageActionModel) => {
    const message = this.channels
      .find((x) => x.id === model.channelId)
      ?.messages?.find((x) => x.guid === model.guid);
    if (!isDefined(message)) return;

    if (!message.read) {
      message.read = true;
      message.readDate = new Date();
      this.updateMessage(message);
    }
  };

  private sortMessages = (messages: MessageFullModel[]) =>
    orderBy(messages, (x) => moment(x.sendDate), "asc");

  private sortChannels = (channels: ChatFullModel[]) =>
    orderBy(
      channels,
      (x) => moment(x.lastMessage?.sendDate ?? x.createDate),
      "desc"
    );
}
