import { Button, Form, FormInstance, Input, Modal, Spin } from "antd";
import React, { FC } from "react";
import ParticlesBackground from "src/containers/ParticlesBackground";
import { LoginModel } from "src/models";
import { useStores } from "src/stores";
import { observer } from "mobx-react";

const LoginPage: FC = () => {
  const formRef = React.useRef<FormInstance>(null);
  const { userStore } = useStores();

  const submit = async () => {
    await formRef.current!.validateFields();
    const model = formRef.current!.getFieldsValue() as LoginModel;
    await userStore.login(model);
    if (userStore.user.loaded) {
      document.location.href = "/";
    }
  };

  return (
    <>
      <ParticlesBackground />
      <Modal
        centered
        closable={false}
        open={true}
        mask={false}
        title="Авторизация"
        footer={
          <Button type="primary" onClick={submit} disabled={userStore.loading}>
            Войти
          </Button>
        }
      >
        <Spin spinning={userStore.loading}>
          <Form ref={formRef} labelCol={{ span: 4 }}>
            <Form.Item
              label="Логин"
              name="username"
              rules={[
                { required: true, message: "Логин обязателен для заполнения" },
              ]}
            >
              <Input />
            </Form.Item>

            <Form.Item
              label="Пароль"
              name="password"
              rules={[
                { required: true, message: "Пароль обязателен для заполнения" },
              ]}
            >
              <Input.Password />
            </Form.Item>
          </Form>
        </Spin>
      </Modal>
    </>
  );
};

export default observer(LoginPage);
