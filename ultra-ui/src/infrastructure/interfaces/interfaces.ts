import React from "react";
import { EntityConfiguration, FieldConfiguration } from "src/models";

export interface InputProps {
  provider: InputProvider;
  config: FieldConfiguration;
  entityConfig: EntityConfiguration;
  disabled?: boolean;
}

export interface RootInputProvider {
  getValue: (sysName: string) => any;
  setValue: (sysName: string, value: any | null) => void;
}

export interface InputProvider {
  getValue: () => any;
  setValue: (value: any | null) => void;
  root: RootInputProvider;
}

export const getDefaultInputProvider = (
  root: RootInputProvider
): InputProvider => {
  return {
    getValue: () => undefined,
    setValue: () => {},
    root,
  };
};
