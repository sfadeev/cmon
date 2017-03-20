-- Table: public.contract_user

-- DROP TABLE public.contract_user;

CREATE TABLE public.contract_user
(
  contact_id bigint NOT NULL,
  user_name character varying(128) NOT NULL,
  role character varying(16) NOT NULL,
  CONSTRAINT pk_contract_user PRIMARY KEY (contact_id, user_name),
  CONSTRAINT fk_contract_user_contract_id FOREIGN KEY (contact_id)
      REFERENCES public.contract (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.contract_user
  OWNER TO postgres;
GRANT ALL ON TABLE public.contract_user TO postgres;
GRANT SELECT, UPDATE, INSERT, DELETE ON TABLE public.contract_user TO cmon;

-- Index: public.ix_contract_user_user_name

-- DROP INDEX public.ix_contract_user_user_name;

CREATE INDEX ix_contract_user_user_name
  ON public.contract_user
  USING btree
  (user_name COLLATE pg_catalog."default");

