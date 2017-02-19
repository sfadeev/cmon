-- Sequence: public.users_id_seq

-- DROP SEQUENCE public.users_id_seq;

CREATE SEQUENCE public.users_id_seq
  INCREMENT 1
  MINVALUE 1
  MAXVALUE 9223372036854775807
  START 400001
  CACHE 1;
ALTER TABLE public.users_id_seq
  OWNER TO postgres;
GRANT ALL ON SEQUENCE public.users_id_seq TO postgres;

-- Table: public.users

-- DROP TABLE public.users;

CREATE TABLE public.users
(
  id bigint NOT NULL DEFAULT nextval('users_id_seq'::regclass),
  user_name character varying(128),
  first_name character varying(128),
  last_name character varying(128),
  email character varying(128),
  email_confirmed boolean NOT NULL DEFAULT false,
  phone_number character varying(12),
  phone_number_confirmed boolean NOT NULL DEFAULT false,
  password_hash text,
  security_stamp character varying(36),
  two_factor_enabled boolean NOT NULL DEFAULT false,
  lockout_enabled boolean NOT NULL DEFAULT false,
  lockout_end timestamp with time zone,
  access_failed_count integer NOT NULL DEFAULT 0,
  normalized_user_name character varying(128),
  normalized_email character varying(128),
  concurrency_stamp character varying(36) NOT NULL,
  CONSTRAINT pk_user_id PRIMARY KEY (id)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.users
  OWNER TO postgres;
GRANT ALL ON TABLE public.users TO postgres;
GRANT SELECT, UPDATE, INSERT, DELETE ON TABLE public.users TO cmon;

-- Index: public.ix_users_email

-- DROP INDEX public.ix_users_email;

CREATE INDEX ix_users_email
  ON public.users
  USING btree
  (normalized_email COLLATE pg_catalog."default");

-- Index: public.ix_users_user_name

-- DROP INDEX public.ix_users_user_name;

CREATE INDEX ix_users_user_name
  ON public.users
  USING btree
  (normalized_user_name COLLATE pg_catalog."default");
