import {
  ChatFullModel,
  CollectionPage,
  MessageCorrelationModel,
  MessageFullModel,
  PageModel,
  SendMessageModel,
} from "src/models";
import { Dummy, HttpResponse } from "../interfaces";

export default interface ChatService {
  getChannels: (
    model?: PageModel
  ) => Promise<HttpResponse<CollectionPage<ChatFullModel>>>;
  getChannel: (channelId: number) => Promise<HttpResponse<ChatFullModel>>;
  getMessages: (
    channelId: number,
    model?: PageModel
  ) => Promise<HttpResponse<CollectionPage<MessageFullModel>>>;
  sendMessage: (
    channelId: number,
    model?: SendMessageModel
  ) => Promise<HttpResponse<MessageCorrelationModel>>;
  readMessageFromChannel: (channelId: number) => Promise<HttpResponse<Dummy>>;
  receiveMessageFromChannel: (
    channelId: number
  ) => Promise<HttpResponse<Dummy>>;
  readMessage: (messageGuid: string) => Promise<HttpResponse<Dummy>>;
  receiveMessage: (messageGuid: string) => Promise<HttpResponse<Dummy>>;
  getUnreadMessagesCount: () => Promise<HttpResponse<number>>;
}
