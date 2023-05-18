export interface MessageModel {
  guid: string;
  text: string;
  channelId: number;
  sendUserId: number;
  sendDate: Date;
}

export interface MessageCorrelationModel extends MessageModel {
  correlationId: string;
}

export interface MessageFullModel extends MessageModel {
  sendUserName: string;
  received: boolean;
  receivedDate?: Date;
  read: boolean;
  readDate?: Date;

  correlationId?: string;
}

export interface SendMessageModel {
  text: string;
  correlationId?: string;
}

export interface MessageActionModel {
  channelId: number;
  guid: string;
  actorId: number;
}
