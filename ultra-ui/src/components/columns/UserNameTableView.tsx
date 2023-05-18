import { Spin } from "antd";
import React, { FC, useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { useStores } from "src/stores";
import { isDefined } from "src/utils";

const UserNameTableView: FC<{ userId?: number }> = ({ userId }) => {
  const [loading, setLoading] = useState(false);
  const [userName, setUserName] = useState(userId?.toString());
  const { userStore } = useStores();

  useEffect(() => {
    if (!isDefined(userId)) {
      return;
    }

    setLoading(true);
    userStore.getUserName(userId).then((userName) => {
      setUserName(userName.toString());
      setLoading(false);
    });
  }, [userId]);

  return (
    <Spin spinning={loading}>
      {userId && <Link to={`/profile/${userId}`}>{userName}</Link>}
    </Spin>
  );
};

export default UserNameTableView;
