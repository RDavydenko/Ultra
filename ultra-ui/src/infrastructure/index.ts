import React from "react";
import { EntityTypesResolver } from "./entity-types-resolver";

interface EntityContext {
  typesResolver: EntityTypesResolver;
}

const entityContext: EntityContext = {
  typesResolver: new EntityTypesResolver(),
};

const _entityContext: React.Context<EntityContext> =
  React.createContext(entityContext);

export const useEntityContext = () => React.useContext(_entityContext);

export * from "./metadata";
export * from "./interfaces";
export * from "./entity-types-resolver";
