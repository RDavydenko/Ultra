-- Роли
INSERT INTO ult_auth."ROLE" ("CODE", "NAME", "DESCRIPTION")
VALUES
  ('ADMIN', 'Администратор', 'Администратор'),
  ('EDITOR', 'Редактор', 'Редактор'),
  ('VIEWER', 'Наблюдатель', 'Наблюдатель')
ON CONFLICT DO NOTHING;

-- Разрешения
INSERT INTO ult_auth."PERMISSION" ("ENTITY", "ACTION", "DESCRIPTION")
VALUES
  ('DATA', 'VIEW', 'Просмотр данных'),
  ('DATA', 'CREATE_RECORD', 'Создание записей данных'),
  ('DATA', 'UPDATE_RECORD', 'Редактирование записей данных'),
  ('DATA', 'DELETE_RECORD', 'Удаление записей данных'),
  ('CHAT', 'VIEW', 'Просмотр сообщений в чате'),
  ('CHAT', 'SEND', 'Отправка сообщений в чат'),
  ('MAP', 'VIEW', 'Просмотр карты'),
  ('FILE', 'VIEW', 'Просмотр файлов'),
  ('FILE', 'UPLOAD', 'Загрузка файлов'),
  ('FILE', 'DELETE_RECORD', 'Удаление файлов'),
  ('USER', 'VIEW', 'Просмотр пользователей'),
  ('USER', 'CREATE_RECORD', 'Создание пользователей'),
  ('USER', 'UPDATE_RECORD', 'Редактирование пользователей'),
  ('USER', 'DELETE_RECORD', 'Удаление пользователей'),
  ('USER', 'CHANGE_PERMISSION', 'Изменение прав пользователей')
ON CONFLICT DO NOTHING;

-- Роли наблюдателя
INSERT INTO ult_auth."ROLE_PERMISSION" ("ROLE_ID", "PERMISSION_ID")
VALUES
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'VIEWER'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'DATA' AND "ACTION" = 'VIEW')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'VIEWER'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'CHAT' AND "ACTION" = 'VIEW')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'VIEWER'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'MAP' AND "ACTION" = 'VIEW')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'VIEWER'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'FILE' AND "ACTION" = 'VIEW')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'VIEWER'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'USER' AND "ACTION" = 'VIEW'))
ON CONFLICT DO NOTHING;

-- Роли редактора
INSERT INTO ult_auth."ROLE_PERMISSION" ("ROLE_ID", "PERMISSION_ID")
VALUES
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'EDITOR'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'DATA' AND "ACTION" = 'VIEW')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'EDITOR'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'DATA' AND "ACTION" = 'CREATE_RECORD')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'EDITOR'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'DATA' AND "ACTION" = 'UPDATE_RECORD')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'EDITOR'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'DATA' AND "ACTION" = 'DELETE_RECORD')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'EDITOR'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'CHAT' AND "ACTION" = 'VIEW')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'EDITOR'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'CHAT' AND "ACTION" = 'SEND')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'EDITOR'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'MAP' AND "ACTION" = 'VIEW')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'EDITOR'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'FILE' AND "ACTION" = 'VIEW')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'EDITOR'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'FILE' AND "ACTION" = 'UPLOAD')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'EDITOR'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'FILE' AND "ACTION" = 'DELETE_RECORD')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'EDITOR'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'USER' AND "ACTION" = 'VIEW'))
ON CONFLICT DO NOTHING;

-- Роли администратора
INSERT INTO ult_auth."ROLE_PERMISSION" ("ROLE_ID", "PERMISSION_ID")
VALUES
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'ADMIN'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'DATA' AND "ACTION" = 'VIEW')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'ADMIN'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'DATA' AND "ACTION" = 'CREATE_RECORD')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'ADMIN'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'DATA' AND "ACTION" = 'UPDATE_RECORD')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'ADMIN'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'DATA' AND "ACTION" = 'DELETE_RECORD')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'ADMIN'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'CHAT' AND "ACTION" = 'VIEW')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'ADMIN'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'CHAT' AND "ACTION" = 'SEND')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'ADMIN'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'MAP' AND "ACTION" = 'VIEW')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'ADMIN'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'FILE' AND "ACTION" = 'VIEW')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'ADMIN'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'FILE' AND "ACTION" = 'UPLOAD')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'ADMIN'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'FILE' AND "ACTION" = 'DELETE_RECORD')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'ADMIN'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'USER' AND "ACTION" = 'VIEW')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'ADMIN'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'USER' AND "ACTION" = 'CREATE_RECORD')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'ADMIN'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'USER' AND "ACTION" = 'UPDATE_RECORD')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'ADMIN'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'USER' AND "ACTION" = 'DELETE_RECORD')),
  ((SELECT "ID" FROM ult_auth."ROLE" WHERE "CODE" = 'ADMIN'), (SELECT "ID" FROM ult_auth."PERMISSION" WHERE "ENTITY" = 'USER' AND "ACTION" = 'CHANGE_PERMISSION'))
ON CONFLICT DO NOTHING;
