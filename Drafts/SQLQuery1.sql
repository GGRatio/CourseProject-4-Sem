USE fitness_db;
SELECT * FROM Subscriptions;


INSERT INTO Subscriptions (Name, DurationDays, Condition, Price)
VALUES 
('Разовое', 30,NULL,15),
('5 Посещений', 30, 'До 16:00', 40),
('5 посещений', 30, 'Студенческий', 45),
('5 посещений', 30, Null, 60),
('Безлимит', 30, NULL, 90),
('Безлимит', 30, 'До 16:00', 75),
('Безлимит', 30, 'Студенческий', 80),
('Безлимит', 60, NULL, 170),
('Безлимит', 90, NULL, 250);