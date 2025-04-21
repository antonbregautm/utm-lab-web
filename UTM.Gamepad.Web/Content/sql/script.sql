-- Основной SQL-скрипт для инициализации базы данных
-- Этот скрипт создает базу данных, таблицы и заполняет их начальными данными

USE master;
GO

-- Создание базы данных, если она не существует
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'GameStore')
BEGIN
    CREATE DATABASE GameStore;
    PRINT 'База данных GameStore создана';
END
ELSE
BEGIN
    PRINT 'База данных GameStore уже существует';
END
GO

USE GameStore;
GO

-- Создание таблиц

-- Таблица ролей
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Roles')
BEGIN
    CREATE TABLE Roles (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        Name NVARCHAR(50) NOT NULL,
        Description NVARCHAR(200) NULL,
        UserId UNIQUEIDENTIFIER NULL
    );
    PRINT 'Таблица Roles создана';
END
ELSE
BEGIN
    PRINT 'Таблица Roles уже существует';
END

-- Таблица пользователей
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        FullName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NOT NULL,
        PasswordHash NVARCHAR(100) NOT NULL
    );
    PRINT 'Таблица Users создана';
END
ELSE
BEGIN
    PRINT 'Таблица Users уже существует';
END

-- Таблица продуктов
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
BEGIN
    CREATE TABLE Products (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        Name NVARCHAR(255) NOT NULL,
        Description NVARCHAR(MAX) NULL,
        Price DECIMAL(10, 2) NOT NULL,
        InStock INT NOT NULL DEFAULT 0,
        ImageUrl NVARCHAR(255) NULL
    );
    PRINT 'Таблица Products создана';
END
ELSE
BEGIN
    PRINT 'Таблица Products уже существует';
END

-- Таблица заказов
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders')
BEGIN
    CREATE TABLE Orders (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        UserId UNIQUEIDENTIFIER NOT NULL,
        OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
        TotalAmount DECIMAL(10, 2) NOT NULL,
        Status NVARCHAR(50) NOT NULL,
        FOREIGN KEY (UserId) REFERENCES Users(Id)
    );
    PRINT 'Таблица Orders создана';
END
ELSE
BEGIN
    PRINT 'Таблица Orders уже существует';
END

-- Таблица элементов заказа
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderItems')
BEGIN
    CREATE TABLE OrderItems (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        OrderId UNIQUEIDENTIFIER NOT NULL,
        ProductId UNIQUEIDENTIFIER NOT NULL,
        Quantity INT NOT NULL,
        Price DECIMAL(10, 2) NOT NULL,
        FOREIGN KEY (OrderId) REFERENCES Orders(Id),
        FOREIGN KEY (ProductId) REFERENCES Products(Id)
    );
    PRINT 'Таблица OrderItems создана';
END
ELSE
BEGIN
    PRINT 'Таблица OrderItems уже существует';
END

-- Таблица отзывов
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Reviews')
BEGIN
    CREATE TABLE Reviews (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        UserId UNIQUEIDENTIFIER NOT NULL,
        ProductId UNIQUEIDENTIFIER NOT NULL,
        Rating INT NOT NULL,
        Comment NVARCHAR(MAX) NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY (UserId) REFERENCES Users(Id),
        FOREIGN KEY (ProductId) REFERENCES Products(Id)
    );
    PRINT 'Таблица Reviews создана';
END
ELSE
BEGIN
    PRINT 'Таблица Reviews уже существует';
END

-- Создание индексов
-- Пользователи
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE UNIQUE INDEX IX_Users_Email ON Users(Email);
    PRINT 'Индекс IX_Users_Email создан';
END

-- Заказы
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Orders_UserId' AND object_id = OBJECT_ID('Orders'))
BEGIN
    CREATE INDEX IX_Orders_UserId ON Orders(UserId);
    PRINT 'Индекс IX_Orders_UserId создан';
END

-- Отзывы
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Reviews_UserId' AND object_id = OBJECT_ID('Reviews'))
BEGIN
    CREATE INDEX IX_Reviews_UserId ON Reviews(UserId);
    PRINT 'Индекс IX_Reviews_UserId создан';
END

-- Элементы заказа
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OrderItems_OrderId' AND object_id = OBJECT_ID('OrderItems'))
BEGIN
    CREATE INDEX IX_OrderItems_OrderId ON OrderItems(OrderId);
    PRINT 'Индекс IX_OrderItems_OrderId создан';
END

-- Наполнение данными: роли
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Roles)
BEGIN
    -- Вставляем базовые роли
    DECLARE @AdminRoleId UNIQUEIDENTIFIER = NEWID();
    DECLARE @UserRoleId UNIQUEIDENTIFIER = NEWID();
    DECLARE @ManagerRoleId UNIQUEIDENTIFIER = NEWID();
    
    INSERT INTO dbo.Roles (Id, Name, Description)
    VALUES 
        (@AdminRoleId, 'Admin', 'Администратор системы с полными правами'),
        (@UserRoleId, 'User', 'Обычный пользователь системы'),
        (@ManagerRoleId, 'Manager', 'Менеджер с расширенными правами');
        
    PRINT 'Базовые роли добавлены';
END
ELSE
BEGIN
    PRINT 'Роли уже существуют в базе данных';
END

-- Создание администратора системы
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Users WHERE Email = 'admin@gamepad.com')
BEGIN
    -- Получаем ID роли администратора
    DECLARE @AdminRoleId UNIQUEIDENTIFIER;
    SELECT @AdminRoleId = Id FROM dbo.Roles WHERE Name = 'Admin';
    
    IF @AdminRoleId IS NOT NULL
    BEGIN
        -- Создаем нового администратора
        DECLARE @AdminId UNIQUEIDENTIFIER = NEWID();
        
        -- SHA256 хеш для пароля 'admin123'
        INSERT INTO dbo.Users (Id, FullName, Email, PasswordHash)
        VALUES (@AdminId, 'Администратор системы', 'admin@gamepad.com', '3gVlZkJh1MPHK0N3/UPnl5MitdnbXsLlI/0oeXP1dRA=');
        
        -- Обновляем связь с ролью админа
        UPDATE dbo.Roles 
        SET UserId = @AdminId
        WHERE Id = @AdminRoleId;
        
        PRINT 'Создан администратор: admin@gamepad.com (пароль: admin123)';
    END
END
ELSE
BEGIN
    PRINT 'Администратор уже существует';
END

PRINT 'Скрипт инициализации базы данных успешно выполнен';
GO 