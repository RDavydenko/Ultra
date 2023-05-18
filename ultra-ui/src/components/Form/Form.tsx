import { Form } from "antd";
import { observer } from "mobx-react";
import React, { FC, useEffect, useMemo, useState } from "react";
import {
  getDefaultInputProvider,
  InputProvider,
  RootInputProvider,
  useEntityContext,
} from "src/infrastructure";
import {
  EntityConfiguration,
  EntityMethods,
  FieldConfiguration,
  FieldMethods,
} from "src/models";
import { isDefined } from "src/utils";

export class FormController {
  fields: FormInput[] = [];
  private readOnlyValues: Record<string, any> = {};

  getValue = (key: string) => {
    const provider = this.fields.find((x) => x.key === key)?.provider;
    if (isDefined(provider)) {
      return provider.getValue();
    } else {
      return this.readOnlyValues?.[key];
    }
  };

  getValues = () => {
    const obj: any = {};
    this.fields.forEach(
      (x) => (obj[x.fieldConfig.systemName] = x.provider.getValue())
    );
    return obj;
  };

  setValue = (key: string, value: any) => {
    const provider = this.fields.find((x) => x.key === key)?.provider;
    if (isDefined(provider)) {
      provider.setValue(value);
    } else {
      this.readOnlyValues[key] = value;
    }
  };

  setValues = (values: any) => {
    for (const key in values) {
      this.setValue(key, values[key]);
    }
  };

  reset() {
    for (const field of this.fields) {
      field.provider.setValue(null);
    }
    this.readOnlyValues = {};
  }

  validate(): boolean {
    // TODO:
    return true;
  }

  buildProvider(): RootInputProvider {
    return {
      getValue: (sysName) => this.getValue(sysName),
      setValue: (sysName, value) => this.setValue(sysName, value),
    };
  }
}

export const useForm = () => {
  return useMemo(() => {
    return { form: new FormController() };
  }, []);
};

interface FormProps {
  mode: EntityMethods.Create | EntityMethods.Update;
  form: FormController;
  fields: FieldConfiguration[];
  entityConfig: EntityConfiguration;
}

interface FormInput {
  key: string;
  fieldConfig: FieldConfiguration;
  provider: InputProvider;
}

const UltraForm: FC<FormProps> = ({ mode, fields, entityConfig, form }) => {
  const [formFields, setFormFields] = useState<FormInput[]>([]);
  const { typesResolver } = useEntityContext();

  useEffect(() => {
    setFormFields(
      fields
        .filter((config) => typesResolver.isVisibleInDetailView(config))
        .map((fieldConfig) => {
          return {
            key: fieldConfig.systemName,
            fieldConfig: fieldConfig,
            provider: getDefaultInputProvider(form.buildProvider()),
          };
        })
    );
  }, [fields, form]);

  useEffect(() => {
    form.fields = formFields;
  }, [formFields]);

  const isDisabled = (config: FieldConfiguration): boolean => {
    const disabled = config.meta?.["disabled"];
    if (disabled) {
      return true;
    }

    if (mode === EntityMethods.Create) {
      return (
        config.methods.find((method) => method === FieldMethods.Created) ===
        undefined
      );
    }

    if (mode === EntityMethods.Update) {
      return (
        config.methods.find((method) => method === FieldMethods.Updated) ===
        undefined
      );
    }

    return false;
  };

  return (
    <Form layout="vertical" style={{ minHeight: 200 }}>
      {formFields.map((formField, i) => (
        <Form.Item
          key={i}
          label={
            formField.fieldConfig.displayName ??
            formField.fieldConfig.systemName
          }
          name={formField.fieldConfig.systemName}
          required={
            formField.fieldConfig.meta?.["validation.required"] ?? false
          }
        >
          {typesResolver.getDetailViewInput(formField.fieldConfig, {
            provider: formField.provider,
            config: formField.fieldConfig,
            entityConfig: entityConfig,
            disabled: isDisabled(formField.fieldConfig),
          })}
        </Form.Item>
      ))}
    </Form>
  );
};

export default observer(UltraForm);
