USE Energy_DB;
GO

-- Твой хэш для пароля 123456
UPDATE Users SET PasswordHash = 'jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=' WHERE Role IN ('User', 'Trainer');
GO

-- Админ (пароль admin)
UPDATE Users SET PasswordHash = 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=' WHERE Login = 'admin';
GO

-- Проверяем
SELECT Login, Role, PasswordHash FROM Users ORDER BY Id;
GO