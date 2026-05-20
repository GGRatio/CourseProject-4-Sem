USE Energy_DB
Select * FROM Users
SELECT * FROM Trainers
SELECT * FROM UserTrainers

UPDATE Users SET PasswordHash = 'WZRHGrsBESr8wYFZ9sx0tPURuZgG2lmzyvWpwXPKz8U=' 
WHERE Role = 'Trainer';

SELECT * FROM GroupClasses

SELECT Id, Name, Instructor FROM GroupClasses WHERE Instructor LIKE '%Екатерина%';


USE Energy_DB;

-- Хэш для пароля "12345" (получен из твоего примера: Test с паролем WZRHGrsBESr8wYFZ9sx0tPURuZgG2lmzyvWpwXPKz8U=)
DECLARE @hash VARCHAR(255) = 'WZRHGrsBESr8wYFZ9sx0tPURuZgG2lmzyvWpwXPKz8U=';

INSERT INTO Users (Login, PasswordHash, Email, Role, FirstName, LastName, Phone)
VALUES 
('ivanov', @hash, 'ivanov@mail.com', 'User', 'Иван', 'Иванов', '+375291234567'),
('petrov', @hash, 'petrov@mail.com', 'User', 'Петр', 'Петров', '+375292345678'),
('sidorov', @hash, 'sidorov@mail.com', 'User', 'Сидор', 'Сидоров', '+375293456789'),
('kozlov', @hash, 'kozlov@mail.com', 'User', 'Козьма', 'Козлов', '+375294567890'),
('morozov', @hash, 'morozov@mail.com', 'User', 'Мороз', 'Морозов', '+375295678901'),
('volkov', @hash, 'volkov@mail.com', 'User', 'Волк', 'Волков', '+375296789012'),
('zaitsev', @hash, 'zaitsev@mail.com', 'User', 'Зайц', 'Зайцев', '+375297890123'),
('lisitsyn', @hash, 'lisitsyn@mail.com', 'User', 'Лис', 'Лисицын', '+375298901234'),
('medvedev', @hash, 'medvedev@mail.com', 'User', 'Медведь', 'Медведев', '+375299012345'),
('orlov', @hash, 'orlov@mail.com', 'User', 'Орел', 'Орлов', '+375291123456'),
('sokolov', @hash, 'sokolov@mail.com', 'User', 'Сокол', 'Соколов', '+375292234567'),
('golubev', @hash, 'golubev@mail.com', 'User', 'Голубь', 'Голубев', '+375293345678'),
('vorobiev', @hash, 'vorobiev@mail.com', 'User', 'Воробей', 'Воробьев', '+375294456789'),
('sinitsyn', @hash, 'sinitsyn@mail.com', 'User', 'Синица', 'Синицын', '+375295567890'),
('soloviev', @hash, 'soloviev@mail.com', 'User', 'Соловей', 'Соловьев', '+375296678901'),
('sorokin', @hash, 'sorokin@mail.com', 'User', 'Сорока', 'Сорокин', '+375297789012'),
('kurochkin', @hash, 'kurochkin@mail.com', 'User', 'Курок', 'Курочкин', '+375298890123'),
('galkin', @hash, 'galkin@mail.com', 'User', 'Галка', 'Галкин', '+375299901234'),
('grachev', @hash, 'grachev@mail.com', 'User', 'Грач', 'Грачев', '+375291234567'),
('zhuravlev', @hash, 'zhuravlev@mail.com', 'User', 'Журавль', 'Журавлев', '+375292345678');



-- Клиенты выбирают тренеров
-- Тренер 7 (Анна Иванова) - Йога
INSERT INTO UserTrainers (UserId, TrainerId, SelectedDate)
SELECT Id, 7, GETDATE() FROM Users WHERE Login IN ('ivanov', 'petrov', 'sidorov', 'kozlov', 'morozov');

-- Тренер 8 (Дмитрий Петров) - Силовые
INSERT INTO UserTrainers (UserId, TrainerId, SelectedDate)
SELECT Id, 8, GETDATE() FROM Users WHERE Login IN ('volkov', 'zaitsev', 'lisitsyn', 'medvedev', 'orlov');

-- Тренер 9 (Екатерина Сидорова) - Пилатес
INSERT INTO UserTrainers (UserId, TrainerId, SelectedDate)
SELECT Id, 9, GETDATE() FROM Users WHERE Login IN ('sokolov', 'golubev', 'vorobiev', 'sinitsyn', 'soloviev');

-- Тренер 10 (Михаил Козлов) - Кроссфит
INSERT INTO UserTrainers (UserId, TrainerId, SelectedDate)
SELECT Id, 10, GETDATE() FROM Users WHERE Login IN ('sorokin', 'kurochkin', 'galkin', 'grachev', 'zhuravlev');








-- Занятия Анны Ивановой (Йога)
INSERT INTO GroupClasses (Name, Instructor, ClassDate, MaxParticipants, CurrentParticipants, DurationMinutes)
VALUES 
('Йога утро', 'Анна Иванова', DATEADD(day, 1, GETDATE()), 10, 3, 60),
('Йога вечер', 'Анна Иванова', DATEADD(day, 2, GETDATE()), 10, 2, 60),
('Йога выходные', 'Анна Иванова', DATEADD(day, 3, GETDATE()), 10, 0, 60);

-- Занятия Дмитрия Петрова (Силовые)
INSERT INTO GroupClasses (Name, Instructor, ClassDate, MaxParticipants, CurrentParticipants, DurationMinutes)
VALUES 
('Силовая тренировка', 'Дмитрий Петров', DATEADD(day, 1, GETDATE()), 12, 5, 60),
('Пауэрлифтинг', 'Дмитрий Петров', DATEADD(day, 2, GETDATE()), 8, 3, 90);

-- Занятия Екатерины Сидоровой (Пилатес)
INSERT INTO GroupClasses (Name, Instructor, ClassDate, MaxParticipants, CurrentParticipants, DurationMinutes)
VALUES 
('Пилатес утро', 'Екатерина Сидорова', DATEADD(day, 1, GETDATE()), 8, 4, 50),
('Пилатес вечер', 'Екатерина Сидорова', DATEADD(day, 2, GETDATE()), 8, 2, 50);

-- Занятия Михаила Козлова (Кроссфит)
INSERT INTO GroupClasses (Name, Instructor, ClassDate, MaxParticipants, CurrentParticipants, DurationMinutes)
VALUES 
('Кроссфит', 'Михаил Козлов', DATEADD(day, 1, GETDATE()), 10, 6, 45);






-- Клиенты записываются на занятия
DECLARE @classId1 INT = (SELECT Id FROM GroupClasses WHERE Name = 'Йога утро');
DECLARE @classId2 INT = (SELECT Id FROM GroupClasses WHERE Name = 'Силовая тренировка');
DECLARE @classId3 INT = (SELECT Id FROM GroupClasses WHERE Name = 'Пилатес утро');
DECLARE @classId4 INT = (SELECT Id FROM GroupClasses WHERE Name = 'Кроссфит');

-- Записи на Йогу
INSERT INTO ClassRegistrations (UserId, GroupClassId, RegistrationDate, IsAttended, IsCanceled)
SELECT Id, @classId1, GETDATE(), 0, 0 FROM Users WHERE Login IN ('ivanov', 'petrov', 'sidorov');

-- Записи на Силовую
INSERT INTO ClassRegistrations (UserId, GroupClassId, RegistrationDate, IsAttended, IsCanceled)
SELECT Id, @classId2, GETDATE(), 0, 0 FROM Users WHERE Login IN ('volkov', 'zaitsev', 'lisitsyn', 'medvedev', 'orlov');

-- Записи на Пилатес
INSERT INTO ClassRegistrations (UserId, GroupClassId, RegistrationDate, IsAttended, IsCanceled)
SELECT Id, @classId3, GETDATE(), 0, 0 FROM Users WHERE Login IN ('sokolov', 'golubev', 'vorobiev', 'sinitsyn');

-- Записи на Кроссфит
INSERT INTO ClassRegistrations (UserId, GroupClassId, RegistrationDate, IsAttended, IsCanceled)
SELECT Id, @classId4, GETDATE(), 0, 0 FROM Users WHERE Login IN ('sorokin', 'kurochkin', 'galkin', 'grachev', 'zhuravlev', 'soloviev');







-- Проверить пользователей
SELECT COUNT(*) AS UsersCount FROM Users WHERE Role = 'User';

-- Проверить записи к тренерам
SELECT COUNT(*) AS UserTrainersCount FROM UserTrainers;

-- Проверить групповые занятия
SELECT * FROM GroupClasses;

-- Проверить записи на занятия
SELECT COUNT(*) AS RegistrationsCount FROM ClassRegistrations;



--Пароли для входа:
--Роль	Логин	Пароль
--Админ	admin	admin
--Тренер	anna.ivanova	12345
--Тренер	dmitry.petrov	12345
--Тренер	ekaterina.sidorova	12345
--Тренер	mikhail.kozlov	12345
--Любой пользователь	ivanov, petrov, sidorov...	12345
