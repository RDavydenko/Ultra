\connect ult;
\echo 'Connected database ult';

-- schemes
CREATE SCHEMA IF NOT EXISTS ult_auth;
\echo 'Created scheme ult.ult_auth';
CREATE SCHEMA IF NOT EXISTS ult_crm;
\echo 'Created scheme ult.ult_crm';
CREATE SCHEMA IF NOT EXISTS ult_msg;
\echo 'Created scheme ult.ult_msg';


-- extensions
CREATE EXTENSION IF NOT EXISTS postgis SCHEMA ult_crm;
\echo 'Installed extension Postgis to scheme ult.ult_crm';

