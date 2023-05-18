import { notification } from "antd";

type NotificationType = "success" | "info" | "warning" | "error";
interface NotifyModel {
  type: NotificationType;
  title: string;
  text: string;
}

export class NotificationStore {
  private notify({ type, title, text }: NotifyModel) {
    notification[type]({
      message: title,
      description: text,
    });
  }

  info = (text: string, title = "Информация") => {
    this.notify({
      type: "info",
      title,
      text,
    });
  };

  success = (text: string, title = "Успех") => {
    this.notify({
      type: "success",
      title,
      text,
    });
  };

  warning = (text: string, title = "Предупреждение") => {
    this.notify({
      type: "warning",
      title,
      text,
    });
  };

  error = (text: string, title = "Произошла ошибка") => {
    this.notify({
      type: "error",
      title,
      text,
    });
  };
}
