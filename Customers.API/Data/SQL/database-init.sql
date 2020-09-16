CREATE OR REPLACE FUNCTION create_admin_role(IN schem TEXT, role_name TEXT) RETURNS VOID
LANGUAGE plpgsql AS $$

BEGIN

	IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = role_name) THEN
		EXECUTE format('CREATE ROLE %I WITH NOLOGIN NOSUPERUSER INHERIT NOCREATEDB NOCREATEROLE NOREPLICATION', role_name);

		EXECUTE format('GRANT USAGE ON SCHEMA %I TO %I', schem, role_name);
		EXECUTE format('GRANT ALL ON ALL SEQUENCES IN SCHEMA %I TO %I WITH GRANT OPTION', schem, role_name);
		EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA %I GRANT ALL ON SEQUENCES TO %I WITH GRANT OPTION', schem, role_name);
		EXECUTE format('GRANT ALL ON ALL TABLES IN SCHEMA %I TO %I WITH GRANT OPTION', schem, role_name);
		EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA %I GRANT ALL ON TABLES TO %I WITH GRANT OPTION', schem, role_name);
		EXECUTE format('GRANT ALL ON ALL FUNCTIONS IN SCHEMA %I TO %I WITH GRANT OPTION', schem, role_name);
		EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA %I GRANT ALL ON FUNCTIONS TO %I WITH GRANT OPTION', schem, role_name);

		RAISE NOTICE 'Created Role % and granted permission.', role_name;
	ELSE
		EXECUTE format('GRANT USAGE ON SCHEMA %I TO %I', schem, role_name);
		EXECUTE format('GRANT ALL ON ALL SEQUENCES IN SCHEMA %I TO %I WITH GRANT OPTION', schem, role_name);
		EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA %I GRANT ALL ON SEQUENCES TO %I WITH GRANT OPTION', schem, role_name);
		EXECUTE format('GRANT ALL ON ALL TABLES IN SCHEMA %I TO %I WITH GRANT OPTION', schem, role_name);
		EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA %I GRANT ALL ON TABLES TO %I WITH GRANT OPTION', schem, role_name);
		EXECUTE format('GRANT ALL ON ALL FUNCTIONS IN SCHEMA %I TO %I WITH GRANT OPTION', schem, role_name);
		EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA %I GRANT ALL ON FUNCTIONS TO %I WITH GRANT OPTION', schem, role_name);

		RAISE NOTICE 'Role % already exist. Not creating but applied grants again.', role_name;
	END IF;


END;
$$;

CREATE OR REPLACE FUNCTION setup_user(IN username TEXT, IN pwd TEXT, IN _role TEXT) RETURNS VOID
LANGUAGE plpgsql AS $$
BEGIN
	IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = username) THEN
    	EXECUTE format('CREATE USER %I WITH PASSWORD %L INHERIT', username, pwd);
		EXECUTE format('GRANT %I to %I', _role, username);
		RAISE NOTICE 'Created user "%"', username;
	ELSE
		RAISE NOTICE 'User % already exist. Not creating.', username;
	END IF;
END;
$$;


CREATE SCHEMA IF NOT EXISTS core; 
SELECT create_admin_role('core', 'dhc_admins');
SELECT setup_user('dhc_api_admin','<replace>','dhc_admins');