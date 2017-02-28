-- Sequence: public.role_claim_id_seq

-- DROP SEQUENCE public.role_claim_id_seq;

CREATE SEQUENCE public.role_claim_id_seq
  INCREMENT 1
  MINVALUE 1
  MAXVALUE 9223372036854775807
  START 100
  CACHE 1;
ALTER TABLE public.role_claim_id_seq
  OWNER TO postgres;

-- Table: public.role_claim

-- DROP TABLE public.role_claim;

CREATE TABLE public.role_claim
(
  id integer NOT NULL DEFAULT nextval('role_claim_id_seq'::regclass),
  role_id bigint NOT NULL,
  claim_type character varying(32) NOT NULL,
  claim_value character varying(128) NOT NULL,
  CONSTRAINT pk_role_claim PRIMARY KEY (id),
  CONSTRAINT fk_role_claim_role_id FOREIGN KEY (role_id)
      REFERENCES public.roles (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.role_claim
  OWNER TO postgres;
GRANT ALL ON TABLE public.role_claim TO postgres;
GRANT SELECT, UPDATE, INSERT, DELETE ON TABLE public.role_claim TO cmon;

-- Index: public.ix_role_claim_role_id

-- DROP INDEX public.ix_role_claim_role_id;

CREATE INDEX ix_role_claim_role_id
  ON public.role_claim
  USING btree
  (role_id);

