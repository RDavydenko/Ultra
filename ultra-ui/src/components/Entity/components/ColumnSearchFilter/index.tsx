import { SearchOutlined } from "@ant-design/icons";
import { ColumnType } from "antd/lib/table";
import { FieldConfiguration } from "src/models";
import SearchCheckedDropdown from "./SearchCheckedDropdown";

export const getColumnSearchProps = (
  field: FieldConfiguration
): ColumnType<any> => ({
  filterDropdown: ({
    setSelectedKeys,
    selectedKeys,
    confirm,
    clearFilters,
    close,
  }) => (
    <SearchCheckedDropdown
      field={field}
      setSelectedKeys={setSelectedKeys}
      selectedKeys={selectedKeys}
      confirm={confirm}
      clearFilters={clearFilters}
      close={close}
    />
  ),
  filterIcon: (filtered: boolean) => (
    <SearchOutlined style={{ color: filtered ? "#1890ff" : undefined }} />
  ),
  // onFilterDropdownOpenChange: (visible) => {
  //   if (visible) {
  //     setTimeout(() => searchInput.current?.select(), 100);
  //   }
  // },
});
