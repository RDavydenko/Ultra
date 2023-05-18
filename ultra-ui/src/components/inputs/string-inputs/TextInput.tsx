import React, { FC, useState } from "react";
import { Input } from "antd";
import { InputProps } from "src/infrastructure";
import { isDefined } from "src/utils";

const TextInput: FC<InputProps> = ({ provider, config, disabled }) => {
  const [value, setValue] = useState<string | null>();

  provider.getValue = () => value;
  provider.setValue = (val) => setValue(val);

  const min = config.meta?.["range.min"];
  const max = config.meta?.["range.max"];
  const required = config.meta?.["validation.required"] ?? false;

  return (
    <Input.TextArea
      value={value ?? undefined}
      onChange={(e) => setValue(e.target.value)}
      disabled={disabled}
      minLength={min}
      maxLength={max}
      placeholder={`Введите ${config.displayName ?? config.systemName}`}
      showCount={isDefined(min) || isDefined(max)}
      required={required}
    />
  );
};

export default TextInput;
