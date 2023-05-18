import { SearchOutlined } from "@ant-design/icons";
import { Button, Checkbox, Col, Empty, Input, Row, Space, Spin } from "antd";
import { FilterDropdownProps } from "antd/lib/table/interface";
import { sortBy, sortedUniq } from "lodash";
import { observer } from "mobx-react";
import React, { FC, useEffect, useMemo, useState } from "react";
import InfiniteScroll from "react-infinite-scroll-component";
import { FieldConfiguration } from "src/models";
import { useStores } from "src/stores";
import { isDefined, removeAt } from "src/utils";

const WIDTH = 300;

type SearchCheckedDropdownProps = {
  field: FieldConfiguration;
} & Pick<
  FilterDropdownProps,
  "setSelectedKeys" | "selectedKeys" | "clearFilters" | "close" | "confirm"
>;

const SearchCheckedDropdown: FC<SearchCheckedDropdownProps> = ({
  field,
  setSelectedKeys,
  selectedKeys,
  confirm,
  clearFilters,
  close,
}) => {
  const { entityStore, collectionStore } = useStores();
  const [values, setValues] = useState<any[]>([]);
  const [selectedValues, setSelectedValues] = useState<any[]>([]);
  const [initialTotal, setInitialTotal] = useState(0);
  const [q, setQ] = useState<string | undefined>();

  const visibleValues = useMemo(() => {
    if (isDefined(q)) {
      return values.filter((v) =>
        String(v).toLowerCase().includes(q.trim().toLowerCase())
      );
    }
    return values;
  }, [values, q]);

  const getTotal = () => {
    return initialTotal;
  };

  const setValuesInternal = (values: any[]) => {
    setSelectedValues(values);
    setSelectedKeys(values);
  };

  const fetchValues = async () => {
    const [items, total] = await collectionStore.fetchValuesForSearch(
      entityStore.sysName!,
      field.systemName
    );
    let values = (items as any[]).map((x) => x[field.systemName]);
    values = sortBy(sortedUniq(values), (v) => isDefined(v));
    setValues(values);
    setInitialTotal(total as number);
  };

  const check = (value: any) => {
    const index = selectedValues.findIndex((v) => v === value);
    let newCheckedValues: any[] = [];
    if (index !== -1) {
      newCheckedValues = removeAt(selectedValues, index);
    } else {
      newCheckedValues = [...selectedValues, value];
    }

    setValuesInternal(newCheckedValues);
  };

  const handleReset = () => {
    setValuesInternal([]);
    confirm({ closeDropdown: true });
  };

  const handleSearch = () => {
    confirm({ closeDropdown: true });
  };

  useEffect(() => {
    fetchValues();
  }, []);

  return (
    <div style={{ padding: 8 }} onKeyDown={(e) => e.stopPropagation()}>
      <Input
        placeholder={`Поиск ${field.displayName ?? field.systemName}`}
        onChange={(e) => setQ(e.target.value ?? undefined)}
        style={{ marginBottom: 8, display: "block", width: WIDTH }}
      />
      <div
        id={`scrollableDiv_${field.systemName}`}
        style={{
          maxHeight: "200px",
          overflow: "auto",
          width: WIDTH,
        }}
      >
        <InfiniteScroll
          dataLength={visibleValues.length}
          next={fetchValues}
          hasMore={values.length < getTotal()}
          loader={<Spin spinning />}
          scrollableTarget={`scrollableDiv_${field.systemName}`}
        >
          {visibleValues.length === 0 && (
            <Empty image={Empty.PRESENTED_IMAGE_SIMPLE} />
          )}
          {visibleValues.map((v) => (
            <Row key={v}>
              <Col span={24}>
                <Checkbox
                  checked={selectedValues.includes(v)}
                  style={{ width: "100%" }}
                  value={v}
                  onChange={() => check(v)}
                >
                  {!isDefined(v) ? (
                    <span style={{ color: "#ccc" }}>(Пусто)</span>
                  ) : (
                    v
                  )}
                </Checkbox>
              </Col>
            </Row>
          ))}
        </InfiniteScroll>
      </div>
      <Row gutter={10}>
        <Col span={12}>
          <Button
            type="primary"
            onClick={() => handleSearch()}
            icon={<SearchOutlined />}
            size="small"
            style={{ width: "100%" }}
          >
            Поиск
          </Button>
        </Col>
        <Col span={12}>
          <Button
            onClick={() => clearFilters && handleReset()}
            size="small"
            style={{ width: "100%" }}
          >
            Сбросить
          </Button>
        </Col>
      </Row>
    </div>
  );
};

export default observer(SearchCheckedDropdown);
