# basket-service-event-sourcing

[Retrieving PostgreSQL bash]
docker exec -it [Container Id] psql -U [Username] -d [Database Name]

[Retrieving Tables]
basket-service=# \d
[Sample Tables that created by default]
                List of relations
 Schema |          Name          |   Type   |  Owner
--------+------------------------+----------+----------
 public | mt_doc_deadletterevent | table    | hanordev
 public | mt_event_progression   | table    | hanordev
 public | mt_events              | table    | hanordev
 public | mt_events_sequence     | sequence | hanordev
 public | mt_streams             | table    | hanordev
(5 rows)

[Get Streams]
select * from mt_streams;

