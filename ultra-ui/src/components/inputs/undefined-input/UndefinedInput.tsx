import React, { FC, useState } from "react";
import { InputProps } from "src/infrastructure";

const UndefinedInput: FC<InputProps> = ({ provider }) => {
  const [value, setValue] = useState<any | null>();

  provider.getValue = () => value;
  provider.setValue = (val) => setValue(val);

  return (
    <div>Undefined Input</div>
  );
}

export default UndefinedInput;