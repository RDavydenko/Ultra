import React, { FC } from "react";
import { Footer } from "antd/lib/layout/layout";

const UltraFooter: FC = () => {
  return (
    <Footer
      style={{
        textAlign: "center",
        borderTop: "1px solid rgba(0,0,0,0.12)",
        height: "40px",
        padding: 0,
        paddingTop: "10px",
        fontSize: "12px",
      }}
    >
      Powered by <a>Ultra Technologies</a>
    </Footer>
  );
};

export default UltraFooter;
