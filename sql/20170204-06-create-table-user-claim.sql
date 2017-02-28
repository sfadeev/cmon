-- Sequence: public.user_claim_id_seq

-- DROP SEQUENCE public.user_claim_id_seq;

CREATE SEQUENCE public.user_claim_id_seq
  INCREMENT 1
  MINVALUE 1
  MAXVALUE 9223372036854775807
  START 100
  CACHE 1;
ALTER TABLE public.user_claim_id_seq
  OWNER TO postgres;

-- Table: public.user_claim

-- DROP TABLE public.user_claim;

CREATE TABLE public.user_claim
(
  id integer NOT NULL,
  user_id bigint NOT NULL,
  claim_type character varying(32) NOT NULL,
  claim_value character varying(128) NOT NULL,
  CONSTRAINT pk_user_claim PRIMARY KEY (id),
  CONSTRAINT fk_user_claim_user_id FOREIGN KEY (user_id)
      REFERENCES public.users (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.user_claim
  OWNER TO postgres;
GRANT ALL ON TABLE public.user_claim TO postgres;
GRANT SELECT, UPDATE, INSERT, DELETE ON TABLE public.user_claim TO cmon;

-- Index: public.ix_user_claim_user_id

-- DROP INDEX public.ix_user_claim_user_id;

CREATE INDEX ix_user_claim_user_id
  ON public.user_claim
  USING btree
  (user_id);
