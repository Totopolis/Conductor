CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE public.deployment (
    id uuid NOT NULL,
    created timestamp with time zone NOT NULL,
    number integer NOT NULL,
    state text NOT NULL,
    notes text NOT NULL,
    CONSTRAINT deployment_id PRIMARY KEY (id)
);

CREATE TABLE public.process (
    id uuid NOT NULL,
    name text NOT NULL,
    display_name text NOT NULL,
    description text NOT NULL,
    created timestamp with time zone NOT NULL,
    number integer NOT NULL,
    CONSTRAINT process_id PRIMARY KEY (id)
);

CREATE TABLE public.target (
    id uuid NOT NULL,
    deployment_id uuid NOT NULL,
    process_id uuid NOT NULL,
    revision_id uuid NOT NULL,
    parallel_count integer NOT NULL,
    buffer_size integer NOT NULL,
    CONSTRAINT target_id PRIMARY KEY (id),
    CONSTRAINT "FK_target_deployment_deployment_id" FOREIGN KEY (deployment_id) REFERENCES public.deployment (id) ON DELETE CASCADE
);

CREATE TABLE public.revision (
    id uuid NOT NULL,
    process_id uuid NOT NULL,
    created timestamp with time zone NOT NULL,
    number integer NOT NULL,
    is_draft boolean NOT NULL,
    release_notes text NOT NULL,
    content jsonb NOT NULL,
    CONSTRAINT revision_id PRIMARY KEY (id),
    CONSTRAINT "FK_revision_process_process_id" FOREIGN KEY (process_id) REFERENCES public.process (id) ON DELETE CASCADE
);

CREATE INDEX "IX_revision_process_id" ON public.revision (process_id);

CREATE INDEX "IX_target_deployment_id" ON public.target (deployment_id);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241004181555_InitialCreate', '8.0.8');

COMMIT;

