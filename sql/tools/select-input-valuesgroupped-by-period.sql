-- http://www.sqlines.com/postgresql/how-to/datediff
select
    input_num,
    period,

    MIN(P.Value) as MIN,
    AVG(P.Value) as Avg,
    MAX(P.Value) as MAX
from
    (select 
     t.input_num,

     '2017-01-01T00:00:00'::timestamp +
     ( ( (DATE_PART('day', t.created_at - '2017-01-01T00:00:00'::timestamp) * 24 + 
          DATE_PART('hour', t.created_at - '2017-01-01T00:00:00'::timestamp)) * 60 +
        DATE_PART('minute', t.created_at - '2017-01-01T00:00:00'::timestamp) ) / 30)::int * interval '30 minute' AS Period,

     t.Value
     from input_value t
     where device_id=0) as P

group by input_num, Period;

-- https://www.postgresql.org/docs/9.1/static/functions-srf.html
SELECT * FROM generate_series('2008-03-01 00:00'::timestamp,
                              '2008-03-04 12:00', '30 minute');