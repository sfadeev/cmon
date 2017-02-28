-- Table: public.user_token

-- DROP TABLE public.user_token;

CREATE TABLE public.user_token
(
  user_id bigint NOT NULL,
  login_provider character varying(36) NOT NULL,
  name character varying(128) NOT NULL,
  value text,
  CONSTRAINT pk_user_token PRIMARY KEY (user_id, login_provider, name)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.user_token
  OWNER TO postgres;
GRANT ALL ON TABLE public.user_token TO postgres;
GRANT SELECT, UPDATE, INSERT, DELETE ON TABLE public.user_token TO cmon;
