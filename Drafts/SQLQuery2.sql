USE Energy_DB;
GO

-- ============================================
-- 1. ОЧИСТКА
-- ============================================
DELETE FROM Reviews;
DELETE FROM ClassRegistrations;
DELETE FROM UserTrainers;
DELETE FROM Purchases;
DELETE FROM GroupClasses;
DELETE FROM Trainers;
DELETE FROM Subscriptions;
DELETE FROM Users;
GO

-- ============================================
-- 2. ПОЛЬЗОВАТЕЛИ
-- ============================================
-- Пароль "admin" = 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg='
-- Пароль "12345" = 'WZRHGrsBESr8wYFZ9sx0tPURuZgG2lmzyvWpwXPKz8U='

SET IDENTITY_INSERT Users ON;

INSERT INTO Users (Id, Login, PasswordHash, Email, Role, FirstName, LastName, Phone, TotalVisits)
VALUES 
(1, 'admin', 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', 'admin@fit.com', 'Admin', 'Админ', 'Админов', '+375291234567', 0),
(2, 'anna.ivanova', 'WZRHGrsBESr8wYFZ9sx0tPURuZgG2lmzyvWpwXPKz8U=', 'anna@fit.com', 'Trainer', 'Анна', 'Иванова', '+375291234568', 0),
(3, 'dmitry.petrov', 'WZRHGrsBESr8wYFZ9sx0tPURuZgG2lmzyvWpwXPKz8U=', 'dmitry@fit.com', 'Trainer', 'Дмитрий', 'Петров', '+375291234569', 0),
(4, 'ekaterina.sidorova', 'WZRHGrsBESr8wYFZ9sx0tPURuZgG2lmzyvWpwXPKz8U=', 'ekaterina@fit.com', 'Trainer', 'Екатерина', 'Сидорова', '+375291234570', 0),
(5, 'mikhail.kozlov', 'WZRHGrsBESr8wYFZ9sx0tPURuZgG2lmzyvWpwXPKz8U=', 'mikhail@fit.com', 'Trainer', 'Михаил', 'Козлов', '+375291234571', 0),
(6, 'ivanov', 'WZRHGrsBESr8wYFZ9sx0tPURuZgG2lmzyvWpwXPKz8U=', 'ivanov@mail.com', 'User', 'Иван', 'Иванов', '+375291111111', 5),
(7, 'petrov', 'WZRHGrsBESr8wYFZ9sx0tPURuZgG2lmzyvWpwXPKz8U=', 'petrov@mail.com', 'User', 'Петр', 'Петров', '+375292222222', 3),
(8, 'sidorov', 'WZRHGrsBESr8wYFZ9sx0tPURuZgG2lmzyvWpwXPKz8U=', 'sidorov@mail.com', 'User', 'Сидор', 'Сидоров', '+375293333333', 7),
(9, 'kozlov', 'WZRHGrsBESr8wYFZ9sx0tPURuZgG2lmzyvWpwXPKz8U=', 'kozlov@mail.com', 'User', 'Козьма', 'Козлов', '+375294444444', 2),
(10, 'morozov', 'WZRHGrsBESr8wYFZ9sx0tPURuZgG2lmzyvWpwXPKz8U=', 'morozov@mail.com', 'User', 'Мороз', 'Морозов', '+375295555555', 4),
(11, 'volkov', 'WZRHGrsBESr8wYFZ9sx0tPURuZgG2lmzyvWpwXPKz8U=', 'volkov@mail.com', 'User', 'Волк', 'Волков', '+375296666666', 6),
(12, 'zaitsev', 'WZRHGrsBESr8wYFZ9sx0tPURuZgG2lmzyvWpwXPKz8U=', 'zaitsev@mail.com', 'User', 'Зайц', 'Зайцев', '+375297777777', 1),
(13, 'lisitsyn', 'WZRHGrsBESr8wYFZ9sx0tPURuZgG2lmzyvWpwXPKz8U=', 'lisitsyn@mail.com', 'User', 'Лис', 'Лисицын', '+375298888888', 8),
(14, 'medvedev', 'WZRHGrsBESr8wYFZ9sx0tPURuZgG2lmzyvWpwXPKz8U=', 'medvedev@mail.com', 'User', 'Медведь', 'Медведев', '+375299999999', 3),
(15, 'orlov', 'WZRHGrsBESr8wYFZ9sx0tPURuZgG2lmzyvWpwXPKz8U=', 'orlov@mail.com', 'User', 'Орел', 'Орлов', '+375291234572', 5);

SET IDENTITY_INSERT Users OFF;
GO

-- ============================================
-- 3. АБОНЕМЕНТЫ
-- ============================================
SET IDENTITY_INSERT Subscriptions ON;

INSERT INTO Subscriptions (Id, Name, Price, DurationDays, Condition)
VALUES 
(1, 'Базовый', 50, 30, NULL),
(2, 'Премиум', 90, 30, NULL),
(3, '10 посещений', 70, 60, NULL),
(4, 'Студенческий', 45, 30, 'Студенческий'),
(5, 'Утренний', 40, 30, 'До 16:00');

SET IDENTITY_INSERT Subscriptions OFF;
GO

-- ============================================
-- 4. ТРЕНЕРЫ
-- ============================================
SET IDENTITY_INSERT Trainers ON;

INSERT INTO Trainers (Id, FirstName, LastName, Specialization, YearsOfExperience, Description, PhotoUrl)
VALUES 
(1, 'Анна', 'Иванова', 'Йога', 5, 'Сертифицированный инструктор по йоге. Помогает расслабиться и укрепить тело.', 'Images/anna_ivanova.jpg'),
(2, 'Дмитрий', 'Петров', 'Силовые тренировки', 8, 'Мастер спорта по пауэрлифтингу. Поможет набрать мышечную массу.', 'Images/dmitry_petrov.jpg'),
(3, 'Екатерина', 'Сидорова', 'Пилатес', 4, 'Реабилитолог. Восстановление после травм, улучшение осанки.', 'Images/ekaterina_sidorova.jpg'),
(4, 'Михаил', 'Козлов', 'Кроссфит', 3, 'Тренер по кроссфиту. Интенсивные тренировки для выносливости.', 'Images/mikhail_kozlov.jpg');

SET IDENTITY_INSERT Trainers OFF;
GO

-- ============================================
-- 5. ПОКУПКИ
-- ============================================
INSERT INTO Purchases (UserId, SubscriptionId, PurchaseDate, EndDate, IsActive)
VALUES 
(6, 1, '2026-05-01', '2026-06-01', 1),
(7, 2, '2026-05-05', '2026-06-05', 1),
(8, 3, '2026-05-10', '2026-07-10', 1),
(9, 4, '2026-05-15', '2026-06-15', 1),
(10, 1, '2026-05-20', '2026-06-20', 1),
(11, 2, '2026-05-25', '2026-06-25', 1),
(6, 1, '2026-03-01', '2026-04-01', 0),
(6, 2, '2026-04-05', '2026-05-05', 0),
(7, 3, '2026-02-10', '2026-04-10', 0),
(8, 1, '2026-01-15', '2026-02-15', 0),
(9, 2, '2026-02-20', '2026-03-20', 0),
(10, 4, '2026-03-25', '2026-04-25', 0);
GO

-- ============================================
-- 6. ГРУППОВЫЕ ЗАНЯТИЯ
-- ============================================
SET IDENTITY_INSERT GroupClasses ON;

INSERT INTO GroupClasses (Id, Name, Instructor, ClassDate, MaxParticipants, CurrentParticipants, DurationMinutes)
VALUES 
(1, 'Йога утро', 'Анна Иванова', DATEADD(day, 1, GETDATE()), 10, 2, 60),
(2, 'Йога вечер', 'Анна Иванова', DATEADD(day, 3, GETDATE()), 10, 1, 60),
(3, 'Силовая тренировка', 'Дмитрий Петров', DATEADD(day, 2, GETDATE()), 12, 3, 60),
(4, 'Пилатес', 'Екатерина Сидорова', DATEADD(day, 1, GETDATE()), 8, 2, 50),
(5, 'Кроссфит', 'Михаил Козлов', DATEADD(day, 2, GETDATE()), 10, 4, 45),
(6, 'Йога выходные', 'Анна Иванова', DATEADD(day, 5, GETDATE()), 10, 0, 60),
(7, 'Силовая вечер', 'Дмитрий Петров', DATEADD(day, 4, GETDATE()), 12, 1, 60),
(8, 'Йога утро', 'Анна Иванова', DATEADD(day, -5, GETDATE()), 10, 8, 60),
(9, 'Йога вечер', 'Анна Иванова', DATEADD(day, -7, GETDATE()), 10, 6, 60),
(10, 'Силовая тренировка', 'Дмитрий Петров', DATEADD(day, -3, GETDATE()), 12, 10, 60),
(11, 'Пилатес', 'Екатерина Сидорова', DATEADD(day, -10, GETDATE()), 8, 6, 50),
(12, 'Кроссфит', 'Михаил Козлов', DATEADD(day, -14, GETDATE()), 10, 9, 45);

SET IDENTITY_INSERT GroupClasses OFF;
GO

-- ============================================
-- 7. ЗАПИСИ НА ЗАНЯТИЯ
-- ============================================
INSERT INTO ClassRegistrations (UserId, GroupClassId, RegistrationDate, IsAttended, IsCanceled)
VALUES 
(6, 1, GETDATE(), 0, 0),
(7, 1, GETDATE(), 0, 0),
(8, 2, GETDATE(), 0, 0),
(9, 3, GETDATE(), 0, 0),
(10, 3, GETDATE(), 0, 0),
(11, 4, GETDATE(), 0, 0),
(12, 5, GETDATE(), 0, 0),
(13, 5, GETDATE(), 0, 0),
(6, 8, DATEADD(day, -5, GETDATE()), 1, 0),
(7, 8, DATEADD(day, -5, GETDATE()), 1, 0),
(8, 8, DATEADD(day, -5, GETDATE()), 0, 0),
(9, 9, DATEADD(day, -7, GETDATE()), 1, 0),
(10, 9, DATEADD(day, -7, GETDATE()), 1, 0),
(11, 10, DATEADD(day, -3, GETDATE()), 1, 0),
(12, 11, DATEADD(day, -10, GETDATE()), 1, 0),
(13, 11, DATEADD(day, -10, GETDATE()), 0, 0),
(14, 12, DATEADD(day, -14, GETDATE()), 1, 0);
GO

-- ============================================
-- 8. СВЯЗИ ПОЛЬЗОВАТЕЛЬ-ТРЕНЕР
-- ============================================
INSERT INTO UserTrainers (UserId, TrainerId, SelectedDate)
VALUES 
(6, 1, '2026-05-01'),
(7, 1, '2026-05-05'),
(8, 2, '2026-05-10'),
(9, 2, '2026-05-15'),
(10, 3, '2026-05-20'),
(11, 3, '2026-05-25'),
(12, 4, '2026-05-10'),
(13, 4, '2026-05-15');
GO

-- ============================================
-- 9. ОТЗЫВЫ
-- ============================================
INSERT INTO Reviews (UserId, TrainerId, Comment, CreatedAt)
VALUES 
(6, 1, 'Отличный тренер! Йога помогла расслабиться после работы. Спасибо!', '2026-05-15'),
(7, 1, 'Анна очень внимательная, всегда поправляет если делаю неправильно. Очень довольна!', '2026-05-20'),
(8, 2, 'Дмитрий настоящий профессионал! За месяц тренировок результаты уже видны.', '2026-05-18'),
(9, 2, 'Хороший тренер, но иногда бывает строгим. Зато мотивирует!', '2026-05-22'),
(10, 3, 'Екатерина помогла восстановиться после травмы спины. Очень благодарна!', '2026-05-25'),
(11, 3, 'Пилатес с Екатериной - это что-то невероятное. Стала гибче и здоровее.', '2026-05-28'),
(12, 4, 'Лучший тренер по кроссфиту! Тренировки интенсивные, но результат стоит того.', '2026-05-12'),
(13, 4, 'Михаил очень энергичный, заряжает своей энергией. Рекомендую!', '2026-05-19');
GO

-- ============================================
-- 10. ПРОВЕРКА
-- ============================================
SELECT '👥 Users' AS Пункт, COUNT(*) AS Количество FROM Users
UNION SELECT '🏋️ Trainers', COUNT(*) FROM Trainers
UNION SELECT '📋 Subscriptions', COUNT(*) FROM Subscriptions
UNION SELECT '💰 Purchases', COUNT(*) FROM Purchases
UNION SELECT '🧘 GroupClasses', COUNT(*) FROM GroupClasses
UNION SELECT '📝 ClassRegistrations', COUNT(*) FROM ClassRegistrations
UNION SELECT '🔗 UserTrainers', COUNT(*) FROM UserTrainers
UNION SELECT '⭐ Reviews', COUNT(*) FROM Reviews
ORDER BY Пункт;
GO