-- Sequence: public.device_id_seq

-- DROP SEQUENCE public.device_id_seq;

CREATE SEQUENCE public.device_id_seq
  INCREMENT 1
  MINVALUE 1
  MAXVALUE 9223372036854775807
  START 100000
  CACHE 1;
ALTER TABLE public.device_id_seq
  OWNER TO postgres;
GRANT ALL ON SEQUENCE public.device_id_seq TO postgres;
