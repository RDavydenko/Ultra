import { InputNumber } from "antd";
import React, { FC, useState } from "react";
import { InputProps } from "src/infrastructure/interfaces";

const NumberInput: FC<InputProps> = ({ provider, config, disabled }) => {
  const [value, setValue] = useState<number | null>();

  provider.getValue = () => value;
  provider.setValue = (val) => setValue(val);

  const min = config.meta?.["range.min"];
  const max = config.meta?.["range.max"];
  const required = config.meta?.["validation.required"] ?? false;

  return (
    <InputNumber
      style={{ width: "100%" }}
      value={value}
      onChange={(v) => setValue(v)}
      min={min}
      max={max}
      placeholder={`Введите ${config.displayName ?? config.systemName}`}
      disabled={disabled}
      required={required}
    />
  );
};

export default NumberInput;
