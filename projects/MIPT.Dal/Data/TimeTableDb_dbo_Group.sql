create table if not exists [Group]
(
    Id   int identity constraint Group_pk primary key,
    Name nvarchar(128)
);

INSERT INTO [Group] (Id, Name) VALUES (1, 'М05-13А');
INSERT INTO [Group] (Id, Name) VALUES (2, 'М05-13Б');