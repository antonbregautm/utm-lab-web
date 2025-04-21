-- Скрипт для инициализации ролей пользователей
-- Перед выполнением скрипта убедитесь, что БД GameStore и таблица Roles созданы

USE GameStore;
GO

-- Проверяем, существует ли таблица Roles
IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL
BEGIN
    -- Проверяем наличие записей в таблице
    IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Roles)
    BEGIN
        -- Вставляем базовые роли
        DECLARE @AdminId UNIQUEIDENTIFIER = NEWID();
        DECLARE @UserId UNIQUEIDENTIFIER = NEWID();
        DECLARE @ManagerId UNIQUEIDENTIFIER = NEWID();
        
        INSERT INTO dbo.Roles (Id, Name, Description)
        VALUES 
            (@AdminId, 'Admin', 'Администратор системы с полными правами'),
            (@UserId, 'User', 'Обычный пользователь системы'),
            (@ManagerId, 'Manager', 'Менеджер с расширенными правами');
            
        PRINT 'Базовые роли успешно добавлены в таблицу Roles';
    END
    ELSE
    BEGIN
        PRINT 'Таблица Roles уже содержит записи';
    END
END
ELSE
BEGIN
    PRINT 'Таблица Roles не существует. Сначала создайте таблицу.';
END 