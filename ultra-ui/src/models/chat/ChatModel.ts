import { MessageFullModel } from "./MessageModel";

export interface ChatModel {
  id: number;
  type: ChannelType;
  name: string;
  createDate: Date;
  createUserId: number;
  createUserName: string;
  userIds: number[];
}

export interface ChatFullModel extends ChatModel {
  silenced: boolean;
  lastMessage?: MessageFullModel;
  unreadMessagesCount: number;

  messages?: MessageFullModel[];
  messagesTotalCount?: number;
}

export enum ChannelType {
  Private = "PRIVATE",
  Group = "GROUP",
}
