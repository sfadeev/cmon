ALTER TABLE public.device
  ADD COLUMN name character varying(64);
ALTER TABLE public.device
  ADD COLUMN status integer NOT NULL DEFAULT 0;
ALTER TABLE public.device
  ADD COLUMN contract_id bigint;
ALTER TABLE public.device
  ADD COLUMN hash bytea;
ALTER TABLE public.device
  ADD COLUMN config json;

ALTER TABLE public.device
  ADD CONSTRAINT fk_device_contract_id FOREIGN KEY (contract_id) REFERENCES public.contract (id)
   ON UPDATE NO ACTION ON DELETE NO ACTION;
CREATE INDEX fki_device_contract_id
  ON public.device(contract_id);
