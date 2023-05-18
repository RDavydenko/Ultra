export {};

declare global {
  interface Window {
    redirectToAuth?: boolean;
    identityServerApiUrl?: string;
  }
}
