import urljoin from "url-join";
import {
  AppConfig,
  ChatFullModel,
  CollectionPage,
  MessageCorrelationModel,
  MessageFullModel,
  PageModel,
  SendMessageModel,
} from "src/models";
import { authorizedRequest } from "../httpService";
import ChatService from "./chat-service.types";
import { Dummy } from "../interfaces";

export class ChatHttpService implements ChatService {
  constructor(private appConfig: AppConfig) {}

  getChannels = (model?: PageModel) =>
    authorizedRequest<CollectionPage<ChatFullModel>>({
      method: "GET",
      params: model,
      url: urljoin(this.appConfig.msgApiUrl, "Channels"),
    });

  getChannel = (channelId: number) =>
    authorizedRequest<ChatFullModel>({
      method: "GET",
      url: urljoin(this.appConfig.msgApiUrl, "Channels", channelId.toString()),
    });

  getMessages = (channelId: number, model?: PageModel) =>
    authorizedRequest<CollectionPage<MessageFullModel>>({
      method: "GET",
      params: model,
      url: urljoin(
        this.appConfig.msgApiUrl,
        "Channels",
        channelId.toString(),
        "messages"
      ),
    });

  sendMessage = (channelId: number, model?: SendMessageModel) =>
    authorizedRequest<MessageCorrelationModel>({
      method: "POST",
      data: model,
      url: urljoin(
        this.appConfig.msgApiUrl,
        "Channels",
        channelId.toString(),
        "send"
      ),
    });

  readMessageFromChannel = (channelId: number) =>
    authorizedRequest<Dummy>({
      method: "POST",
      url: urljoin(
        this.appConfig.msgApiUrl,
        "Channels",
        channelId.toString(),
        "read"
      ),
    });

  receiveMessageFromChannel = (channelId: number) =>
    authorizedRequest<Dummy>({
      method: "POST",
      url: urljoin(
        this.appConfig.msgApiUrl,
        "Channels",
        channelId.toString(),
        "receive"
      ),
    });

  readMessage = (messageGuid: string) =>
    authorizedRequest<Dummy>({
      method: "POST",
      url: urljoin(this.appConfig.msgApiUrl, "Messages", messageGuid, "read"),
    });

  receiveMessage = (messageGuid: string) =>
    authorizedRequest<Dummy>({
      method: "POST",
      url: urljoin(
        this.appConfig.msgApiUrl,
        "Messages",
        messageGuid,
        "receive"
      ),
    });

  getUnreadMessagesCount = () =>
    authorizedRequest<number>({
      method: "GET",
      url: urljoin(this.appConfig.msgApiUrl, "Channels", "unreadMessagesCount"),
    });
}
