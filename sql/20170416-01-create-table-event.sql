-- Sequence: public.event_id_seq

-- DROP SEQUENCE public.event_id_seq;

CREATE SEQUENCE public.event_id_seq
  INCREMENT 1
  MINVALUE 1
  MAXVALUE 9223372036854775807
  START 1
  CACHE 1;
ALTER TABLE public.event_id_seq
  OWNER TO postgres;
GRANT ALL ON SEQUENCE public.event_id_seq TO postgres;

-- Table: public.event

-- DROP TABLE public.event;

CREATE TABLE public.event
(
  id bigint NOT NULL DEFAULT nextval('event_id_seq'::regclass),
  device_id bigint NOT NULL,
  event_type character varying(32) NOT NULL,
  created_at timestamp without time zone NOT NULL,
  created_by character varying(128),
  external_id bigint,
  info json,
  CONSTRAINT pk_event_id PRIMARY KEY (id),
  CONSTRAINT fk_event_device_id FOREIGN KEY (device_id)
      REFERENCES public.device (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.event
  OWNER TO postgres;
GRANT ALL ON TABLE public.event TO postgres;
GRANT SELECT, UPDATE, INSERT, DELETE ON TABLE public.event TO cmon;

-- Index: public.ix_event_device_id_created_at

-- DROP INDEX public.ix_event_device_id_created_at;

CREATE INDEX ix_event_device_id_created_at
  ON public.event
  USING btree
  (device_id, created_at);

