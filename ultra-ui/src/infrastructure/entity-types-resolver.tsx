import moment from "moment";
import { Link } from "react-router-dom";
import UserNameTableView from "src/components/columns/UserNameTableView";
import UserIdInput from "src/components/inputs/custom-inputs/UserIdInput";
import LocationInput from "src/components/inputs/location-inputs/LocationInput";
import ParentReferenceInput from "src/components/inputs/reference-inputs/ParentReferenceInput";
import ReferenceChildrenInput from "src/components/inputs/reference-inputs/ReferenceChildrenInput";
import { FieldConfiguration, FieldType } from "src/models";
import { firstLetterToLower, isDefined, isForeignKeyField } from "src/utils";
import DateTimeInput from "../components/inputs/date-inputs/DateTimeInput";
import TimeInput from "../components/inputs/date-inputs/TimeInput";
import DecimalInput from "../components/inputs/number-inputs/DecimalInput";
import NumberInput from "../components/inputs/number-inputs/NumberInput";
import StringInput from "../components/inputs/string-inputs/StringInput";
import TextInput from "../components/inputs/string-inputs/TextInput";
import UndefinedInput from "../components/inputs/undefined-input/UndefinedInput";
import { InputProps } from "./interfaces";
import { routes } from "src/consts";
import { isDecimalType, isIntegerType } from "./entity-types.utils";

const stringType: EntityTypeNode = {
  condition: (config) => config.type === FieldType.String,
  detailViewInput: (props: InputProps) => <StringInput {...props} />,
  listViewColumn: (config) => ({
    ...getDefaultListColumnView(config),
    filter: true,
  }),
};

const textType: EntityTypeNode = {
  condition: (config) => config.type === FieldType.Text,
  detailViewInput: (props: InputProps) => <TextInput {...props} />,
};

const numberType: EntityTypeNode = {
  condition: (config) =>
    isIntegerType(config.type) && !isForeignKeyField(config),
  detailViewInput: (props: InputProps) => <NumberInput {...props} />,
  listViewColumn: (config) => ({
    ...getDefaultListColumnView(config),
    filter: true,
  }),
};

const decimalType: EntityTypeNode = {
  condition: (config) =>
    isDecimalType(config.type) && !isForeignKeyField(config),
  detailViewInput: (props: InputProps) => <DecimalInput {...props} />,
  listViewColumn: (config) => ({
    ...getDefaultListColumnView(config),
    filter: true,
  }),
};

const timeType: EntityTypeNode = {
  condition: (config) => config.type === FieldType.Time,
  detailViewInput: (props: InputProps) => <TimeInput {...props} />,
};

const dateTimeType: EntityTypeNode = {
  condition: (config) => config.type === FieldType.DateTime,
  detailViewInput: (props) => <DateTimeInput {...props} />,
  listViewColumn: (config) => {
    return {
      title: config.displayName ?? config.systemName,
      sorter: true,
      key: config.systemName,
      render: (x) =>
        isDefined(x[config.systemName])
          ? moment.utc(x[config.systemName]).format("DD.MM.YYYY HH:mm:ss")
          : "",
    };
  },
};

const referenceParentType: EntityTypeNode = {
  condition: (config) => config.type === FieldType.ReferenceParent,
  detailViewVisible: () => false,
  listViewColumn: (config) => {
    const referenceSysName = config.meta["foreignKey.type"];
    let displayField = "Id";
    if (config.meta["foreignKey.displayable"]) {
      displayField = config.meta["foreignKey.displayable"];
    }
    return {
      title: config.displayName ?? config.systemName,
      sorter: false,
      key: config.systemName,
      render: (x) =>
        isDefined(x[config.systemName]) ? (
          <Link
            target="_blank"
            rel="noopener noreferrer"
            to={routes.data.entities.byId.path(
              referenceSysName,
              x[config.systemName]["Id"]
            )}
          >
            {x[config.systemName][displayField]}
          </Link>
        ) : (
          ""
        ),
    };
  },
  listViewGraphQLProjection: (config) => {
    let displayField = "Id";
    if (config.meta["foreignKey.displayable"]) {
      displayField = config.meta["foreignKey.displayable"];
    }
    return `${config.systemName}: ${firstLetterToLower(config.systemName)} {
        Id: id
        ${displayField}: ${firstLetterToLower(displayField)}
      }\n`;
  },
  detailViewGraphQLProjection: (config) => {
    let displayField = "Id";
    if (config.meta["foreignKey.displayable"]) {
      displayField = config.meta["foreignKey.displayable"];
    }
    return `${config.systemName}: ${firstLetterToLower(config.systemName)} {
        Id: id
        DisplayName: ${firstLetterToLower(displayField)}
      }\n`;
  },
};

const referenceChildrenType: EntityTypeNode = {
  condition: (config) => config.type === FieldType.ReferenceChildren,
  listViewColumn: (config) => {
    return {
      title: config.displayName ?? config.systemName,
      sorter: false,
      key: config.systemName,
      render: (x) => `Количество: ${x[config.systemName].length}`,
    };
  },
  detailViewInput: (props) => {
    return <ReferenceChildrenInput {...props} />;
  },
  listViewGraphQLProjection: (config) => {
    return `${config.systemName}: ${firstLetterToLower(config.systemName)} {
      Id: id
    }\n`;
  },
  detailViewGraphQLProjection: (config) => {
    return `${config.systemName}: ${firstLetterToLower(config.systemName)} {
      Id: id
      DisplayName: ${firstLetterToLower(
        config.meta["foreignKey.displayable"] ?? "Id"
      )}
    }\n`;
  },
};

const userIdType: EntityTypeNode = {
  condition: (config) => config.type === FieldType.UserId,
  detailViewInput: (props) => <UserIdInput {...props} />,
  listViewColumn: (config) => {
    return {
      title: config.displayName ?? config.systemName,
      sorter: true,
      key: config.systemName,
      render: (x) => <UserNameTableView userId={x[config.systemName]} />,
    };
  },
};

const foreignKeyType: EntityTypeNode = {
  condition: (config) => isForeignKeyField(config),
  detailViewInput: (props: InputProps) => {
    const relatedFieldSystemName = props.entityConfig.fields.find(
      (field) => field.meta["foreignKey.path"] === props.config.systemName
    )?.systemName;
    return (
      <ParentReferenceInput {...props} referenceTo={relatedFieldSystemName} />
    );
  },
  // detailViewInput: (props) => <UserIdInput {...props} />,
  listViewVisible: () => false,
};

const locationType: EntityTypeNode = {
  condition: (config) => config.type === FieldType.Location,
  listViewVisible: () => false,
  listViewGraphQLProjection: () => "",

  detailViewInput: (props) => {
    return <LocationInput {...props} />;
  },
  detailViewGraphQLProjection: (config) => {
    return `${config.systemName}: ${firstLetterToLower(config.systemName)} {
      type
      coordinates
    }\n`;
  },
};

const undefinedType: EntityTypeNode = {
  condition: (config) => true,
  listViewVisible: () => false,
  detailViewVisible: () => true,
  detailViewInput: (props) => <UndefinedInput {...props} />,
  listViewGraphQLProjection: () => "",
};

// TODO

// const smthInput: UniversalInput = {
//   condition: (config) => config.type === FieldType.TimePeriod,
//   render: (props: InputProps) => <SmthInput {...props} />,
// };

// if (config.type === FieldType.TimePeriod) {
//   return (
//     <Form.Item
//       label={config.displayName ?? config.systemName}
//       name={config.systemName}
//     >
//       <TimePicker.RangePicker
//         defaultValue={value}
//         style={{ width: "100%" }}
//       />
//     </Form.Item>
//   );
// }

// const smthInput: UniversalInput = {
//   condition: (config) => config.type === FieldType.Date,
//   render: (props: InputProps) => <SmthInput {...props} />,
// };

// if (config.type === FieldType.Date) {
//   return (
//     <Form.Item
//       label={config.displayName ?? config.systemName}
//       name={config.systemName}
//     >
//       <DatePicker defaultValue={value} showToday style={{ width: "100%" }} />
//     </Form.Item>
//   );
// }

// const smthInput: UniversalInput = {
//   condition: (config) => config.type === FieldType.DatePeriod,
//   render: (props: InputProps) => <SmthInput {...props} />,
// };

// if (config.type === FieldType.DatePeriod) {
//   return (
//     <Form.Item
//       label={config.displayName ?? config.systemName}
//       name={config.systemName}
//     >
//       <DatePicker.RangePicker
//         defaultValue={value}
//         style={{ width: "100%" }}
//       />
//     </Form.Item>
//   );
// }

// const smthInput: UniversalInput = {
//   condition: (config) => config.type === FieldType.DateTimePeriod,
//   render: (props: InputProps) => <SmthInput {...props} />,
// };

// if (config.type === FieldType.DateTimePeriod) {
//   return (
//     <Form.Item
//       label={config.displayName ?? config.systemName}
//       name={config.systemName}
//     >
//       <DatePicker.RangePicker
//         defaultValue={value}
//         showTime
//         style={{ width: "100%" }}
//       />
//     </Form.Item>
//   );
// }

export type ListColumnView = {
  title: string;
  sorter?: boolean;
  key: string;
  filter?: boolean;
} & (
  | {
      dataIndex: string;
    }
  | { render: (record: any) => any }
);

const getDefaultListColumnView = (
  field: FieldConfiguration
): ListColumnView => {
  return {
    title: field.displayName ?? field.systemName,
    dataIndex: field.systemName,
    key: field.systemName,
    sorter: true,
    filter: false,
  };
};

export interface EntityTypeNode {
  condition: (config: FieldConfiguration) => boolean;
  detailViewVisible?: (config: FieldConfiguration) => boolean;
  detailViewInput?: (props: InputProps) => React.ReactNode;
  listViewVisible?: (config: FieldConfiguration) => boolean;
  listViewColumn?: (config: FieldConfiguration) => ListColumnView;
  listViewGraphQLProjection?: (config: FieldConfiguration) => string;
  detailViewGraphQLProjection?: (config: FieldConfiguration) => string;
}

export class EntityTypesResolver {
  private readonly internalNodes: EntityTypeNode[] = [
    numberType,
    decimalType,
    stringType,
    textType,
    timeType,
    dateTimeType,
    referenceParentType,
    referenceChildrenType,
    userIdType,
    foreignKeyType,
    locationType,
  ];
  private readonly customNodes: EntityTypeNode[] = [];

  get nodes() {
    return [...this.customNodes, ...this.internalNodes, undefinedType];
  }

  inject = (input: EntityTypeNode) => this.customNodes.push(input);

  isVisibleInDetailView = (config: FieldConfiguration) => {
    const node = this.find(config);
    if (isDefined(node)) {
      return node.detailViewVisible?.(config) ?? true;
    }
    return false;
  };

  getDetailViewInput = (
    config: FieldConfiguration,
    props: InputProps
  ): React.ReactNode => {
    const node = this.find(config);
    if (isDefined(node) && this.isVisibleInDetailView(config)) {
      return (
        node.detailViewInput?.(props) ?? undefinedType.detailViewInput!(props)
      );
    }
    return undefinedType.detailViewInput!(props);
  };

  isVisibleInListView = (config: FieldConfiguration) => {
    const node = this.find(config);
    if (isDefined(node)) {
      return node.listViewVisible?.(config) ?? true;
    }
    return false;
  };

  getListViewColumn = (
    config: FieldConfiguration
  ): ListColumnView | undefined => {
    const node = this.find(config);
    if (isDefined(node) && this.isVisibleInListView(config)) {
      return node.listViewColumn?.(config) ?? getDefaultListColumnView(config);
    }
    return undefined;
  };

  getListViewGraphQLProjection = (config: FieldConfiguration): string => {
    const node = this.find(config);
    const defaultGraphQLProjection = `${
      config.systemName
    }: ${firstLetterToLower(config.systemName)}\n`;
    if (isDefined(node)) {
      return (
        node.listViewGraphQLProjection?.(config) ?? defaultGraphQLProjection
      );
    }
    return "";
  };

  getDetailViewGraphQLProjection = (config: FieldConfiguration): string => {
    const node = this.find(config);
    const defaultGraphQLProjection = `${
      config.systemName
    }: ${firstLetterToLower(config.systemName)}\n`;
    if (isDefined(node)) {
      return (
        node.detailViewGraphQLProjection?.(config) ?? defaultGraphQLProjection
      );
    }
    return "";
  };

  private find = (config: FieldConfiguration): EntityTypeNode | undefined => {
    return this.nodes.find((x) => x.condition(config));
  };
}
