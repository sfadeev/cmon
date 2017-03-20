ALTER TABLE public.device
   ALTER COLUMN id SET DEFAULT nextval('device_id_seq');
ALTER TABLE public.device
  ADD COLUMN created_at timestamp without time zone;
ALTER TABLE public.device
  ADD COLUMN created_by character varying(128);
ALTER TABLE public.device
  ADD COLUMN modified_at timestamp without time zone;
ALTER TABLE public.device
  ADD COLUMN modified_by character varying(128);
