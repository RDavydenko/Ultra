export interface CollectionPage<T> {
  items: T[];
  pageInfo: PageInfo;
}

export interface PageInfo {
  pages: number;
  totalCount: number;
}
