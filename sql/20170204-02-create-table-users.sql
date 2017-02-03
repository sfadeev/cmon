CREATE SEQUENCE public.users_id_seq
  INCREMENT 1
  MINVALUE 1
  MAXVALUE 9223372036854775807
  START 400001
  CACHE 1;
ALTER TABLE public.users_id_seq
  OWNER TO postgres;
GRANT ALL ON SEQUENCE public.users_id_seq TO postgres;

-- DROP TABLE public.users;

CREATE TABLE public.users
(
  id bigint NOT NULL DEFAULT nextval('users_id_seq'::regclass),
  user_name character varying(128) NOT NULL,
  first_name character varying(128),
  last_name character varying(128),
  email character varying(128),
  email_confirmed boolean NOT NULL DEFAULT false,
  phone_number character varying(12),
  phone_number_confirmed boolean NOT NULL DEFAULT false,
  password_hash text,
  security_stamp text,
  two_factor_enabled boolean NOT NULL DEFAULT false,
  lockout_enabled boolean NOT NULL DEFAULT false,
  lockout_end_date_utc timestamp with time zone,
  access_failed_count bigint NOT NULL DEFAULT 0,
  CONSTRAINT pk_user_id PRIMARY KEY (id),
  CONSTRAINT uk_user_email UNIQUE (email),
  CONSTRAINT uk_user_name UNIQUE (user_name)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.users
  OWNER TO postgres;
GRANT ALL ON TABLE public.users TO postgres;

-- DROP TABLE public.user_login;

CREATE TABLE public.user_login
(
  user_id bigint NOT NULL,
  login_provider character varying(128) NOT NULL,
  provider_key character varying(128) NOT NULL,
  CONSTRAINT pk_user_login_id_login_provider PRIMARY KEY (user_id, login_provider),
  CONSTRAINT fk_user_login_user_id FOREIGN KEY (user_id)
      REFERENCES public.users (id) MATCH SIMPLE
      ON UPDATE RESTRICT ON DELETE RESTRICT
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.user_login
  OWNER TO postgres;
GRANT ALL ON TABLE public.user_login TO postgres;

-- DROP TABLE public.claim_type;

CREATE TABLE public.claim_type
(
  id bigint NOT NULL,
  code character varying(32) NOT NULL,
  uri character varying(256) NOT NULL,
  CONSTRAINT pk_claim_type_id PRIMARY KEY (id),
  CONSTRAINT uk_claim_type_code UNIQUE (code),
  CONSTRAINT uk_claim_type_uri UNIQUE (uri)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.claim_type
  OWNER TO postgres;
GRANT ALL ON TABLE public.claim_type TO postgres;

-- DROP TABLE public.user_claim;

CREATE TABLE public.user_claim
(
  user_id bigint NOT NULL,
  claim_type_id bigint NOT NULL,
  value character varying(128) NOT NULL,
  CONSTRAINT pk_user_claim_user_id_claim_type_id PRIMARY KEY (user_id, claim_type_id),
  CONSTRAINT fk_user_claim_claim_type_id FOREIGN KEY (claim_type_id)
      REFERENCES public.claim_type (id) MATCH SIMPLE
      ON UPDATE RESTRICT ON DELETE RESTRICT,
  CONSTRAINT fk_user_claim_user_id FOREIGN KEY (user_id)
      REFERENCES public.users (id) MATCH SIMPLE
      ON UPDATE RESTRICT ON DELETE RESTRICT
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.user_claim
  OWNER TO postgres;
GRANT ALL ON TABLE public.user_claim TO postgres;
