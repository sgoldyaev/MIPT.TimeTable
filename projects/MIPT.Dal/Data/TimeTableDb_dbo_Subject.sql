create table if not exists Subject
(
    Id    int identity constraint Subject_pk primary key,
    Title nvarchar(128)
);

INSERT INTO Subject (Id, Title) VALUES (1, 'Философия');
INSERT INTO Subject (Id, Title) VALUES (2, 'Английский язык');
INSERT INTO Subject (Id, Title) VALUES (3, 'Промышленное программирование');
INSERT INTO Subject (Id, Title) VALUES (4, 'VR');
INSERT INTO Subject (Id, Title) VALUES (5, 'Нейронные сети');