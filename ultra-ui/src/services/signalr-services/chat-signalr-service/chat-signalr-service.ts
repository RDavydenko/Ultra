import {
  HubConnection,
  HubConnectionBuilder,
  IHttpConnectionOptions,
} from "@microsoft/signalr";
import { getAccessToken } from "src/utils";
import { ChatEvent } from "./chat-signalr-service.types";

export class ChatSignalRService {
  private static instance: ChatSignalRService;

  private readonly connection: HubConnection;

  private subscribers: Record<string, Function[]> = {};

  private constructor(url: string) {
    this.connection = new HubConnectionBuilder()
      .withUrl(url, {
        accessTokenFactory: async () => getAccessToken(),
      } as IHttpConnectionOptions)
      .withAutomaticReconnect([
        1000,
        5 * 1000,
        10 * 1000,
        20 * 1000,
        60 * 1000,
        120 * 1000,
      ])
      .build();
    this.connection.start().catch((err) => console.error(err));

    this.connection.on(ChatEvent.OnReceivedMessage, (...args) => {
      this.invoke(ChatEvent.OnReceivedMessage, args);
    });
    this.connection.on(ChatEvent.OnMarkReadMessage, (...args) => {
      this.invoke(ChatEvent.OnMarkReadMessage, args);
    });
    this.connection.on(ChatEvent.OnMarkReceivedMessage, (...args) => {
      this.invoke(ChatEvent.OnMarkReceivedMessage, args);
    });
  }

  public reconnect = () => {
    this.connection
      .stop()
      .then((_) => this.connection.start().catch((err) => console.error(err)));
  };

  private invoke = (event: ChatEvent, args: any[]) => {
    if (this.subscribers[event]) {
      const handlers = this.subscribers[event];
      handlers.forEach((handler) => handler.call({}, ...args));
    }
  };

  public subscribe = (event: ChatEvent, handler: Function) => {
    if (!this.subscribers[event]) {
      this.subscribers[event] = [];
    }

    this.subscribers[event].push(handler);
  };

  public unsubscribe = (event: ChatEvent, handler: Function) => {
    if (this.subscribers[event]) {
      this.subscribers[event] = this.subscribers[event].filter(
        (x) => x !== handler
      );
    }
  };

  public dispose = () => {
    this.subscribers = {};
    this.connection.stop();
  };

  // public newMessage = (messages: string) => {
  //   this.connection
  //     .send("newMessage", "foo", messages)
  //     .then((x) => console.log("sent"));
  // };

  public static getInstance(msgHubsUrl: string): ChatSignalRService {
    console.log("getInstance");
    if (!ChatSignalRService.instance) {
      ChatSignalRService.instance = new ChatSignalRService(msgHubsUrl);
      console.log("instance created");
    }
    return ChatSignalRService.instance;
  }
}

export default ChatSignalRService.getInstance;
