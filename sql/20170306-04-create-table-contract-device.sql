-- Table: public.contract_device

-- DROP TABLE public.contract_device;

CREATE TABLE public.contract_device
(
  contact_id bigint NOT NULL,
  device_id bigint NOT NULL,
  CONSTRAINT pk_contract_device PRIMARY KEY (contact_id, device_id),
  CONSTRAINT fk_contract_device_contract_id FOREIGN KEY (contact_id)
      REFERENCES public.contract (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION,
  CONSTRAINT fk_contract_device_device_id FOREIGN KEY (device_id)
      REFERENCES public.device (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.contract_device
  OWNER TO postgres;
GRANT ALL ON TABLE public.contract_device TO postgres;
GRANT SELECT, UPDATE, INSERT, DELETE ON TABLE public.contract_device TO cmon;
