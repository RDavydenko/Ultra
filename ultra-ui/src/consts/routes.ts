/*
 * path - абсолютный путь или функция-builder для построения пути
 * route - формат пути для router'а
 */
export const routes = {
  login: {
    path: "/login",
    route: "/login",
  },
  home: {
    path: "/",
    route: "/",
  },
  data: {
    path: "/d",
    route: "/d",
    entities: {
      route: "/d/:sysName",
      path: (sysName: string) => `/d/${sysName}`,
      byId: {
        route: "/d/:sysName/:id",
        path: (sysName: string, id: number) => `/d/${sysName}/${id}`,
      },
    },
  },
  favorites: {
    path: "/favorites",
    route: "/favorites",
  },
  map: {
    path: "/map",
    route: "/map",
  },
  chat: {
    path: "/chat",
    route: "/chat",
    byId: {
      path: (id: number) => `/chat/${id}`,
    },
  },
  users: {
    path: "/users",
    route: "/users",
    byId: {
      route: "/users/:id",
      path: (id: number) => `/users/${id}`,
    },
  },
  settings: {
    path: "/settings",
    route: "/settings",
  },
  files: {
    path: "/files",
    route: "/files",
  },
};
