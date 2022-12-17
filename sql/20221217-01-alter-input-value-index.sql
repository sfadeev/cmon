-- CREATE INDEX ix_device_input_date ON public.input_value USING btree (device_id, input_no, created_at)
-- drop index ix_device_input_date;

CREATE INDEX ix_device_input_date ON public.input_value USING btree (device_id, created_at);
