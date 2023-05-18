import React, { FC, useState } from "react";
import { InputProps } from "src/infrastructure";

const LocationInput: FC<InputProps> = ({ provider }) => {
  const [value, setValue] = useState<any | null>();

  provider.getValue = () => value;
  provider.setValue = (value) => setValue(value);

  return <div>LocationInput</div>;
};

export default LocationInput;
