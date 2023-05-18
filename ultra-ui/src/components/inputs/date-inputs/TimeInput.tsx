import React, { FC, useState } from "react";
import { TimePicker } from "antd";
import moment from "moment";
import { InputProps } from "src/infrastructure/interfaces";
import { isDefined } from "src/utils";

const TimeInput: FC<InputProps> = ({ provider, config, disabled }) => {
  const [value, setValue] = useState<moment.Moment | null>();

  provider.getValue = () => value?.toISOString(); // TODO
  provider.setValue = (v) => isDefined(v) ? setValue(moment(v)) : setValue(null);

  return (
    <TimePicker
      style={{ width: "100%" }}
      showNow
      value={value}
      onChange={(v) => setValue(v)}
      disabled={disabled}
    />
  );
};

export default TimeInput;
