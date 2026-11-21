USE [OrisDb];
GO

-- Обновление существующей таблицы перед заполнением начальными данными.
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF COL_LENGTH(N'dbo.Roles', N'TurnOrder') IS NULL
    ALTER TABLE dbo.Roles ADD TurnOrder INT NULL;

EXEC (N'
    UPDATE dbo.Roles
    SET TurnOrder = Id
    WHERE TurnOrder IS NULL OR TurnOrder <= 0;

    IF COLUMNPROPERTY(OBJECT_ID(N''dbo.Roles''), N''TurnOrder'', ''AllowsNull'') = 1
        ALTER TABLE dbo.Roles ALTER COLUMN TurnOrder INT NOT NULL;

    IF OBJECT_ID(N''dbo.CK_Roles_TurnOrder'', N''C'') IS NULL
        ALTER TABLE dbo.Roles ADD CONSTRAINT CK_Roles_TurnOrder CHECK (TurnOrder > 0);
');

COMMIT TRANSACTION;
GO

SET IDENTITY_INSERT dbo.Roles ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Id = 1)
    INSERT INTO dbo.Roles (Id, TurnOrder, Name, [Description], ImageURL)
    VALUES (1, 1, N'Убийца', N'Выводит любого персонажа из игры на один раунд', N'/images/roles/assassin.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Id = 2)
    INSERT INTO dbo.Roles (Id, TurnOrder, Name, [Description], ImageURL)
    VALUES (2, 2, N'Вор', N'Cпособен украсть золото у другого персонажа (кроме Ассасина и его жертвы)', N'/images/roles/thief.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Id = 3)
    INSERT INTO dbo.Roles (Id, TurnOrder, Name, [Description], ImageURL)
    VALUES (3, 3, N'Маг', N'Даёт возможность получить новые карты из колоды в обмен на свои, либо обменять все имеющиеся у себя в руке карты на все карты, имеющиеся в руке у другого игрока.', N'/images/roles/magician.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Id = 4)
    INSERT INTO dbo.Roles (Id, TurnOrder, Name, [Description], ImageURL)
    VALUES (4, 4, N'Король', N'Получает дополнительный доход со своих дворянских кварталов (жёлтого цвета) и позволяет владельцу получить корону и первым совершать выбор персонажа в следующем раунде.', N'/images/roles/king.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Id = 5)
    INSERT INTO dbo.Roles (Id, TurnOrder, Name, [Description], ImageURL)
    VALUES (5, 5, N'Епископ', N'Получает защиту зданий от разрушения в этот раунд и получает монету за каждый церковный (синий) квартал в своем городе.', N'/images/roles/bishop.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Id = 6)
    INSERT INTO dbo.Roles (Id, TurnOrder, Name, [Description], ImageURL)
    VALUES (6, 6, N'Торговец', N'Автоматически получает дополнительный денежный бонус в 1 монету и доход со своих кварталов зелёного цвета.', N'/images/roles/trader.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Id = 7)
    INSERT INTO dbo.Roles (Id, TurnOrder, Name, [Description], ImageURL)
    VALUES (7, 7, N'Архитектор', N'Автоматически получает дополнительные два квартала из колоды после совершения действия (то есть после взятия денег или карт кварталов перед постройкой) и может построить вплоть до трёх кварталов (остальные персонажи строят только один квартал в ход).', N'/images/roles/architect.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Id = 8)
    INSERT INTO dbo.Roles (Id, TurnOrder, Name, [Description], ImageURL)
    VALUES (8, 8, N'Военачальник', N'Получает 1 монету за каждый военный (красный) квартал. В конце своего хода может разрушить здание любого игрока (кроме Епископа) за плату, на 1 монету меньшую, чем стоимость постройки квартала. Кварталы стоимостью 1 можно разрушать бесплатно.', N'/images/roles/condotier.png');

SET IDENTITY_INSERT dbo.Roles OFF;
GO

SET IDENTITY_INSERT dbo.Districts ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 1)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (1, N'Поместье', N'', 3, 5, N'Желтый', N'/images/districts/manor.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 2)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (2, N'Замок', N'', 4, 4, N'Желтый', N'/images/districts/castle.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 3)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (3, N'Дворец', N'', 5, 3, N'Желтый', N'/images/districts/palace.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 4)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (4, N'Храм', N'', 1, 3, N'Синий', N'/images/districts/temple.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 5)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (5, N'Церковь', N'', 2, 3, N'Синий', N'/images/districts/church.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 6)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (6, N'Монастырь', N'', 3, 2, N'Синий', N'/images/districts/monastery.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 7)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (7, N'Собор', N'', 5, 2, N'Синий', N'/images/districts/cathedral.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 8)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (8, N'Таверна', N'', 1, 5, N'Зеленый', N'/images/districts/tavern.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 9)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (9, N'Рынок', N'', 2, 3, N'Зеленый', N'/images/districts/market.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 10)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (10, N'Торговая точка', N'', 2, 1, N'Зеленый', N'/images/districts/trading_post.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 11)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (11, N'Доки', N'', 3, 3, N'Зеленый', N'/images/districts/docks.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 12)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (12, N'Порт', N'', 4, 3, N'Зеленый', N'/images/districts/port.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 13)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (13, N'Муниципалитет', N'', 5, 1, N'Зеленый', N'/images/districts/town_hall.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 14)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (14, N'Сторожевая вышка', N'', 1, 3, N'Красный', N'/images/districts/watchtower.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 15)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (15, N'Тюрьма', N'', 3, 3, N'Красный', N'/images/districts/prison.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 16)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (16, N'Марсово поле', N'', 3, 2, N'Красный', N'/images/districts/battlefield.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 17)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (17, N'Крепость', N'', 4, 2, N'Красный', N'/images/districts/fortress.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 18)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (18, N'Квартал призраков', N'При подсчете победных очков этот квартал становится того цвета, который вы сами выберете. Свойство не действует, если квартал призраков построен на последнем ходу игры.', 2, 1, N'Фиолетовый', N'/images/districts/haunted_quarter.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 19)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (19, N'Башня', N'Башня не может быть уничтожена Военачальником', 3, 2, N'Фиолетовый', N'/images/districts/keep.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 20)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (20, N'Великая стена', N'Разрушение кварталов вашего города будет стоить Военачальнику на одно золото дороже', 6, 1, N'Фиолетовый', N'/images/districts/great_wall.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 21)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (21, N'Университет', N'Постройка этого квартала стоит 6 золота, но он приносит 8 очков в конце игры', 6, 1, N'Фиолетовый', N'/images/districts/university.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 22)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (22, N'Обсерватория', N'Если при совершении действия вы тянете карты, то тяните три карты, одну оставьте в руке, а остальные две положите вниз колоды', 5, 1, N'Фиолетовый', N'/images/districts/observatory.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 23)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (23, N'Кладбище', N'Когда Военачальник разрушает ваш квартал, можете заплатить одно золото и вернуть разрушенный квартал в руку. Свойство не действует, если вы сами Военачальник.', 5, 1, N'Фиолетовый', N'/images/districts/graveyard.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 24)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (24, N'Кузница', N'Один раз за ход вы можете за два золота взять в руку три карты из колоды районов', 5, 1, N'Фиолетовый', N'/images/districts/smithy.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 25)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (25, N'Школа магии', N'Для получения прибыли, школа может стать кварталом того цвета, какой вы сами выберете.', 6, 1, N'Фиолетовый', N'/images/districts/school_of_magic.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 26)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (26, N'Библиотека', N'Если вы на своем ходу при совершении действия тянете карты, оставьте в руке обе вытянутые карты', 6, 1, N'Фиолетовый', N'/images/districts/library.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 27)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (27, N'Лаборатория', N'Один раз за ход вы можете сбросить одну карту кварталов с руки и получить за это одно золото из банка', 5, 1, N'Фиолетовый', N'/images/districts/laboratory.png');

IF NOT EXISTS (SELECT 1 FROM dbo.Districts WHERE Id = 28)
    INSERT INTO dbo.Districts (Id, Name, [Description], Cost, Quantity, Color, ImageURL)
    VALUES (28, N'Ворота дракона', N'Постройка этого квартала стоит 6 золота, но он приносит 8 очков в конце игры', 6, 1, N'Фиолетовый', N'/images/districts/dragon_gate.png');

SET IDENTITY_INSERT dbo.Districts OFF;
GO

INSERT INTO dbo.GameRules (Title, Description, Category)
SELECT seed.Title, seed.Description, seed.Category
FROM (VALUES
(
    N'Выбор персонажей',
    N'В начале каждого раунда игроки по очереди выбирают персонажей. Каждый персонаж обладает уникальным свойством.',
    N'Ход игры'
),
(
    N'Получение ресурсов',
    N'В начале своего хода игрок либо получает 2 золотых, либо берёт 2 карты кварталов, оставляя себе одну.',
    N'Ход игры'
),
(
    N'Строительство кварталов',
    N'В свой ход игрок может построить один квартал, заплатив его стоимость золотом.',
    N'Строительство'
),
(
    N'Уникальность кварталов',
    N'В одном городе нельзя построить два одинаковых квартала.',
    N'Строительство'
),
(
    N'Особые свойства персонажей',
    N'Каждый персонаж обладает особым свойством, которое может применять в свой ход в соответствии с правилами.',
    N'Персонажи'
),
(
    N'Завершение игры',
    N'Когда игрок строит седьмой квартал, текущий раунд доигрывается до конца, после чего начинается подсчёт очков.',
    N'Завершение игры'
),
(
    N'Подсчёт очков',
    N'Игроки получают очки за стоимость построенных кварталов, разнообразие их видов и завершение строительства города.',
    N'Завершение игры'
)) AS seed(Title, Description, Category)
WHERE NOT EXISTS (SELECT 1 FROM dbo.GameRules AS existing WHERE existing.Title = seed.Title);
GO

SELECT N'Roles' AS TableName, COUNT(*) AS RowsCount FROM dbo.Roles
UNION ALL
SELECT N'Districts', COUNT(*) FROM dbo.Districts;

