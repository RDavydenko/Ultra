import React, { FC, useState } from "react";
import { DatePicker } from "antd";
import moment from "moment";
import { InputProps } from "src/infrastructure/interfaces";
import { isDefined } from "src/utils";

const TimeInput: FC<InputProps> = ({ provider, config, disabled }) => {
  const [value, setValue] = useState<moment.Moment | null>();

  provider.getValue = () => value?.utc(true).toISOString(true); // TODO: проверить локальное время
  provider.setValue = (v) =>
    isDefined(v) ? setValue(moment.utc(v)) : setValue(null);

  return (
    <DatePicker
      style={{ width: "100%" }}
      showToday
      showTime
      value={value}
      onChange={(v) => setValue(v ?? undefined)}
      disabled={disabled}
    />
  );
};

export default TimeInput;
