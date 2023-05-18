import { Layout } from "antd";
import React, { FC } from "react";
import UltraContent from "./components/UltraContent";
import UltraFooter from "./components/UltraFooter";
import UltraHeader from "./components/UltraHeader";
import UltraSidebar from "./components/UltraSidebar";

const UltraLayout: FC = () => {
  return (
    <Layout style={{ minHeight: "100vh" }}>
      <UltraHeader />
      <Layout hasSider>
        <UltraSidebar />
        <Layout>
          <UltraContent />
          <UltraFooter />
        </Layout>
      </Layout>
    </Layout>
  );
};

export default UltraLayout;
