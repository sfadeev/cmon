-- Table: public.user_role

-- DROP TABLE public.user_role;

CREATE TABLE public.user_role
(
  user_id bigint NOT NULL,
  role_id bigint NOT NULL,
  CONSTRAINT pk_user_role PRIMARY KEY (user_id, role_id)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.user_role
  OWNER TO postgres;
GRANT ALL ON TABLE public.user_role TO postgres;
GRANT SELECT, UPDATE, INSERT, DELETE ON TABLE public.user_role TO cmon;
