import React from "react";
import { AppConfig } from "src/models/AppConfig";
import {
  ChatHttpService,
  ChatSignalRService,
  EntitiesHttpService,
  GraphqlHttpService,
  UserHttpService,
} from "src/services";
import { AuthHttpService } from "src/services/auth-service";
import { ChatStore } from "./chat-store";
import { CollectionStore } from "./collection-store";
import { ConfigurationStore } from "./configuration-store";
import { EntityStore } from "./entity-store";
import { MapStore } from "./map-store/map-store";
import { NotificationStore } from "./notification-store";
import { UserStore } from "./user-store";
import { NavigationStore } from "./navigation-store";
import { AppStore } from "./app-store";

interface AppContext {
  configurationStore: ConfigurationStore;
  notificationStore: NotificationStore;
  entityStore: EntityStore;
  userStore: UserStore;
  collectionStore: CollectionStore;
  mapStore: MapStore;
  chatStore: ChatStore;
  navigationStore: NavigationStore;
  appStore: AppStore;
  signalRService: ChatSignalRService;
}

const notificationStore = new NotificationStore();
const configurationStore = new ConfigurationStore(notificationStore);
const appStore = new AppStore();
const navigationStore = new NavigationStore(appStore);

const appContext = {
  configurationStore,
  notificationStore,
  navigationStore,
  appStore,
} as AppContext;

configurationStore.loadConfiguration(async (cfg: AppConfig) => {
  window.identityServerApiUrl = cfg.authApiUrl;

  const entityService = new EntitiesHttpService(cfg);
  const graphqlService = new GraphqlHttpService(cfg);
  const userService = new UserHttpService(cfg);
  const authService = new AuthHttpService(cfg);
  const chatService = new ChatHttpService(cfg);
  const chatSignalRService = ChatSignalRService.getInstance(cfg.msgHubsUrl);

  appContext.signalRService = chatSignalRService;

  appContext.entityStore = new EntityStore(
    cfg,
    notificationStore,
    entityService,
    graphqlService
  );

  appContext.userStore = new UserStore(
    notificationStore,
    userService,
    authService
  );

  appContext.collectionStore = new CollectionStore(
    notificationStore,
    graphqlService,
    userService
  );

  appContext.mapStore = new MapStore(
    notificationStore,
    graphqlService,
    entityService
  );

  appContext.chatStore = new ChatStore(
    notificationStore,
    chatService,
    appContext.userStore,
    appStore,
    chatSignalRService
  );
});

const storesContext: React.Context<AppContext> =
  React.createContext(appContext);

export const useStores = () => React.useContext(storesContext);

export * from "./entity-store";
export * from "./utils";
export * from "./collection-store";
export * from "./map-store";
export * from "./navigation-store";

export {
  ConfigurationStore,
  NotificationStore,
  EntityStore,
  UserStore,
  MapStore,
  NavigationStore,
};
