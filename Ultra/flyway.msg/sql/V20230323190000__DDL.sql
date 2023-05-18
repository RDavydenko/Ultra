CREATE SCHEMA IF NOT EXISTS ult_msg;

CREATE TABLE IF NOT EXISTS ult_msg."CHANNEL" (
  "ID"                INTEGER PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
  "TYPE"              VARCHAR(32)                 NOT NULL CHECK("TYPE" IN ('PRIVATE', 'GROUP')),
  "NAME"              VARCHAR(256)                NOT NULL,
  "CREATE_DATE"       TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
  "CREATE_USER_ID"    INTEGER                     NOT NULL
);

CREATE TABLE IF NOT EXISTS ult_msg."CHANNEL_USER" (
  "CHANNEL_ID"        INTEGER     NOT NULL REFERENCES ult_msg."CHANNEL"("ID"),
  "USER_ID"           INTEGER     NOT NULL,
  "SILENCED"          BOOLEAN     NOT NULL DEFAULT FALSE,
  PRIMARY KEY ("CHANNEL_ID", "USER_ID")
);

CREATE TABLE IF NOT EXISTS ult_msg."MESSAGE" (
  "GUID"              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  "CHANNEL_ID"        INTEGER                     NOT NULL REFERENCES ult_msg."CHANNEL"("ID"),
  "SEND_USER_ID"      INTEGER                     NOT NULL,
  "SEND_DATE"         TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
  "TEXT"              TEXT                        NOT NULL
); 

CREATE TABLE IF NOT EXISTS ult_msg."MESSAGE_USER" (
  "MESSAGE_GUID"      UUID                        NOT NULL REFERENCES ult_msg."MESSAGE"("GUID"),
  "USER_ID"           INTEGER                     NOT NULL,
  "RECEIVED"		      BOOLEAN	                    NOT NULL DEFAULT FALSE,
  "RECEIVED_DATE"	    TIMESTAMP WITH TIME ZONE        NULL,
  "READ"              BOOLEAN                     NOT NULL DEFAULT FALSE,
  "READ_DATE"		      TIMESTAMP WITH TIME ZONE        NULL, 
  PRIMARY KEY ("MESSAGE_GUID", "USER_ID")
);
