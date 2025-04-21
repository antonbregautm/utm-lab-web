-- Скрипт для создания администратора системы
-- Предварительно должны быть созданы таблицы Users и Roles

USE GameStore;
GO

-- Проверяем существование таблиц
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL AND OBJECT_ID('dbo.Roles', 'U') IS NOT NULL
BEGIN
    -- Находим ID роли администратора
    DECLARE @AdminRoleId UNIQUEIDENTIFIER;
    SELECT @AdminRoleId = Id FROM dbo.Roles WHERE Name = 'Admin';
    
    -- Если роль админа не найдена, создаем её
    IF @AdminRoleId IS NULL
    BEGIN
        SET @AdminRoleId = NEWID();
        INSERT INTO dbo.Roles (Id, Name, Description)
        VALUES (@AdminRoleId, 'Admin', 'Администратор системы с полными правами');
        
        PRINT 'Создана роль Admin';
    END
    
    -- Проверяем, есть ли уже администратор
    IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Users u 
                   INNER JOIN dbo.Roles r ON r.UserId = u.Id 
                   WHERE r.Name = 'Admin')
    BEGIN
        -- Создаем ID для администратора
        DECLARE @AdminId UNIQUEIDENTIFIER = NEWID();
        
        -- Захешированный пароль 'admin123' (вместо этого лучше использовать C# для создания хеша)
        DECLARE @PasswordHash NVARCHAR(100) = '3gVlZkJh1MPHK0N3/UPnl5MitdnbXsLlI/0oeXP1dRA='; -- хеш для 'admin123'
        
        -- Вставляем администратора
        INSERT INTO dbo.Users (Id, FullName, Email, PasswordHash)
        VALUES (@AdminId, 'Администратор системы', 'admin@gamepad.com', @PasswordHash);
        
        -- Обновляем связь с ролью
        UPDATE dbo.Roles 
        SET UserId = @AdminId
        WHERE Id = @AdminRoleId;
        
        PRINT 'Создан административный пользователь (admin@gamepad.com / admin123)';
    END
    ELSE
    BEGIN
        PRINT 'Администратор уже существует в системе';
    END
END
ELSE
BEGIN
    PRINT 'Одна или несколько таблиц не существуют. Сначала выполните миграцию базы данных.';
END 