﻿-- Table: public.claim_type

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
GRANT SELECT, UPDATE, INSERT, DELETE ON TABLE public.claim_type TO cmon;
