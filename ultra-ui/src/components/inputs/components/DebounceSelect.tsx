import { Select, SelectProps, Spin } from "antd";
import { debounce } from "lodash";
import React, { FC, useEffect, useMemo, useRef, useState } from "react";
import { isDefined, isPromise } from "src/utils";

export interface ValueType {
  value: number;
  label: string;
}

export class SelectController {
  private readonly subscribers: ((
    value: number | null,
    label?: string
  ) => void)[] = [];

  clear = () => {};
  getValue = (): [number | null, string | undefined] => [null, undefined];
  setValue = (value: number, label?: string) => {};
  subscribeOnChange = (
    onChange: (value: number | null, label?: string) => void
  ) => {
    this.subscribers.push(onChange);
  };
  invokeChanged = (value: number | null, label?: string) => {
    for (let i = 0; i < this.subscribers.length; i++) {
      this.subscribers[i](value, label);
    }
  };
}

export const useSelect = () => {
  return useMemo(() => {
    return { controller: new SelectController() };
  }, []);
};

interface DebounceSelectProps
  extends Omit<SelectProps<ValueType | ValueType[]>, "children"> {
  fetchOptions: (search?: string) => Promise<ValueType[]>;
  fetchDisplayName?: (value: number) => Promise<string | undefined>;
  fetchByClick?: boolean;
  debounceTimeout?: number;
  controller?: SelectController;
}

const DebounceSelect: FC<DebounceSelectProps> = ({
  fetchOptions,
  fetchDisplayName,
  debounceTimeout = 400,
  fetchByClick,
  controller,
  ...props
}) => {
  const [option, setOption] = useState<ValueType | undefined>();
  const [onFisrtClickLoaded, setOnFirstClickLoaded] = useState(false);
  const [displayNameFetched, setDisplayNameFetched] = useState(false);
  const [fetching, setFetching] = useState(false);
  const [options, setOptions] = useState<ValueType[]>([]);
  const fetchRef = useRef(0);

  if (isDefined(controller)) {
    controller.clear = () => {
      setOptions([]);
      setOption(undefined);
      setDisplayNameFetched(false);
      controller.invokeChanged(null, undefined);
    };
    controller.getValue = () => {
      return [option?.value ?? null, option?.label];
    };
    controller.setValue = (value, label) => {
      if (!isDefined(value)) {
        setOption(undefined);
        controller.invokeChanged(null, undefined);
      } else {
        if (!options.some((o) => o.value === value)) {
          setOptions([...options, { value, label: label ?? "" }]);
        }
        setOption({ value, label: label ?? "" });

        if (!isDefined(label) || label === "") {
          setDisplayNameFetched(false);
        }
        controller.invokeChanged(value, label);
      }
    };
  }

  useEffect(() => {
    if (
      !displayNameFetched &&
      isDefined(option?.value) &&
      (!isDefined(option?.label) || option?.label === "") &&
      isDefined(fetchDisplayName)
    ) {
      setDisplayNameFetched(true);

      setTimeout(() => {
        fetchDisplayName(option!.value).then((displayName) => {
          const newOption = {
            value: option!.value,
            label: displayName ?? String(option!.value),
          };
          controller?.invokeChanged(
            Number(newOption.value),
            String(newOption.label)
          );
          setOption(newOption);
          setOptions([
            newOption,
            ...options.filter((x) => x.value !== newOption.value),
          ]);
        });
      }, 100);
    }
  }, [option, displayNameFetched]); // componentDidUpdate

  const onClick = () => {
    if (fetchByClick && !onFisrtClickLoaded) {
      setOnFirstClickLoaded(true);

      setFetching(true);

      fetchOptions().then((newOptions) => {
        setOptions(newOptions);
        setFetching(false);
      });
    }
  };

  const debounceFetcher = useMemo(() => {
    const loadOptions = (value: string) => {
      fetchRef.current += 1;
      const fetchId = fetchRef.current;

      setOptions([]);
      setFetching(true);

      fetchOptions(value).then((newOptions) => {
        if (fetchId !== fetchRef.current) {
          // for fetch callback order
          return;
        }

        setOptions(newOptions);
        setFetching(false);
      });
    };

    return debounce(loadOptions, debounceTimeout);
  }, [fetchOptions, debounceTimeout]);

  return (
    <Select
      {...props}
      value={option}
      onChange={(val, option) => {
        // TODO: добавить поддержку, если понадобится
        if (!Array.isArray(option)) {
          if (!isDefined(option?.value)) {
            setOption(undefined);
            controller?.invokeChanged(null, undefined);
          } else {
            setOption({
              value: Number(option.value),
              label: String(option.label),
            });
            controller?.invokeChanged(
              Number(option.value),
              String(option.label)
            );
          }
        }
        props.onChange?.(val, option);
      }}
      onClick={(e) => {
        if (!isDefined(props.disabled) || !props.disabled) {
          onClick();
        }
        props.onClick?.(e);
      }}
      filterOption={false}
      onSearch={debounceFetcher}
      notFoundContent={fetching ? <Spin size="small" /> : null}
      options={options}
      filterSort={(a, b) =>
        String(a.label)?.localeCompare(String(b?.label)) ?? 0
      }
    />
  );
};

export default DebounceSelect;
