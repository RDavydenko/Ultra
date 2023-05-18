export interface AppConfig {
  /**API's */
  apiUrl: string;
  authApiUrl: string;
  msgApiUrl: string;
  msgHubsUrl: string;
  graphqlUrl: string;

  redirectToAuth?: boolean;
}
