import { makeAutoObservable, runInAction } from "mobx";
import { AppConfig } from "src/models/AppConfig";
import { NotificationStore } from "../notification-store";
// import urljoin from "url-join";

export class ConfigurationStore implements AppConfig {
  loading = true;

  /** API's */
  apiUrl = "";
  authApiUrl = "";
  msgApiUrl = "";
  msgHubsUrl = "";
  graphqlUrl = "";
  redirectToAuth = false;

  constructor(private readonly notificationStore: NotificationStore) {
    makeAutoObservable(this);
  }

  async loadConfiguration(
    callback: (appConfig: AppConfig) => Promise<void>
  ): Promise<AppConfig | null> {
    try {
      const appJsonResponse = await fetch("/config/app.json");
      const appJson: AppConfig = await appJsonResponse.json();
      /** API's */
      this.apiUrl = appJson.apiUrl;
      this.authApiUrl = appJson.authApiUrl;
      this.msgApiUrl = appJson.msgApiUrl;
      this.msgHubsUrl = appJson.msgHubsUrl;
      this.graphqlUrl = appJson.graphqlUrl;

      window.redirectToAuth = appJson.redirectToAuth;

      const appConfig = this as AppConfig;
      await callback(appConfig);
      return appConfig;
    } catch (ex) {
      this.notificationStore.error(
        `Ошибка загрузки конфигурации клиентского приложения: ${
          (ex as { message: string }).message
        }`
      );
      console.error(ex);
      return null;
    } finally {
      runInAction(() => (this.loading = false));
    }
  }
}
