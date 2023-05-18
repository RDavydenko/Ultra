export interface EntityWithRelatedLinks {
  entity: any;
  linksToAddOrUpdate?: RelatedLink[];
  linksToDelete?: RelatedLink[];
}

export interface RelatedLink {
  entityId: number;
  entitySystemName: string;
  entityPropertyName: string;
}
