-- Table: public.data_protection_key

-- DROP TABLE public.data_protection_key;

CREATE TABLE public.data_protection_key
(
  id character varying(36) NOT NULL,
  data text NOT NULL,
  created_at timestamp without time zone NOT NULL,
  CONSTRAINT pk_data_protection_key PRIMARY KEY (id)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.data_protection_key
  OWNER TO postgres;
GRANT ALL ON TABLE public.data_protection_key TO postgres;
GRANT SELECT, UPDATE, INSERT, DELETE ON TABLE public.data_protection_key TO cmon;
