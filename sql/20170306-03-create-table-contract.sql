-- Sequence: public.contract_id_seq

-- DROP SEQUENCE public.contract_id_seq;

CREATE SEQUENCE public.contract_id_seq
  INCREMENT 1
  MINVALUE 1
  MAXVALUE 9223372036854775807
  START 10000
  CACHE 1;
ALTER TABLE public.contract_id_seq
  OWNER TO postgres;
GRANT ALL ON SEQUENCE public.contract_id_seq TO postgres;

-- Table: public.contract

-- DROP TABLE public.contract;

CREATE TABLE public.contract
(
  id bigint NOT NULL DEFAULT nextval('contract_id_seq'::regclass),
  created_at timestamp without time zone,
  created_by character varying(128),
  modified_at timestamp without time zone,
  modified_by character varying(128),
  CONSTRAINT pk_contract PRIMARY KEY (id)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.contract
  OWNER TO postgres;
GRANT ALL ON TABLE public.contract TO postgres;
GRANT SELECT, UPDATE, INSERT, DELETE ON TABLE public.contract TO cmon;
