DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'bi') THEN
        CREATE SCHEMA bi;
    END IF;
END $EF$;
CREATE TABLE IF NOT EXISTS bi."__biMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___biMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM bi."__biMigrationsHistory" WHERE "MigrationId" = '20250310202027_Actual_1') THEN
        IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'bi') THEN
            CREATE SCHEMA bi;
        END IF;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM bi."__biMigrationsHistory" WHERE "MigrationId" = '20250310202027_Actual_1') THEN
    CREATE TABLE bi.dbsource (
        id uuid NOT NULL,
        kind integer NOT NULL,
        name text NOT NULL,
        private_notes text NOT NULL,
        description text NOT NULL,
        connection_string text NOT NULL,
        schema_mode integer NOT NULL,
        schema text NOT NULL,
        state integer NOT NULL,
        state_changed timestamp with time zone NOT NULL,
        CONSTRAINT dbsource_id PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM bi."__biMigrationsHistory" WHERE "MigrationId" = '20250310202027_Actual_1') THEN
    INSERT INTO bi."__biMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250310202027_Actual_1', '9.0.1');
    END IF;
END $EF$;
COMMIT;

