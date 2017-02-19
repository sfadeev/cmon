-- Table: public.user_login

-- DROP TABLE public.user_login;

CREATE TABLE public.user_login
(
  user_id bigint NOT NULL,
  login_provider character varying(36) NOT NULL,
  provider_key character varying(128) NOT NULL,
  provider_display_name character varying(128),
  CONSTRAINT pk_user_login PRIMARY KEY (login_provider, provider_key),
  CONSTRAINT fk_user_login_user_id FOREIGN KEY (user_id)
      REFERENCES public.users (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.user_login
  OWNER TO postgres;
GRANT ALL ON TABLE public.user_login TO postgres;
GRANT SELECT, UPDATE, INSERT, DELETE ON TABLE public.user_login TO cmon;

-- Index: public.ix_user_login_user_id

-- DROP INDEX public.ix_user_login_user_id;

CREATE INDEX ix_user_login_user_id
  ON public.user_login
  USING btree
  (user_id);

