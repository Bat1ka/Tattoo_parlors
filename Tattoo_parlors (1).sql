-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Хост: MySQL-8.2
-- Время создания: Июн 20 2025 г., 10:40
-- Версия сервера: 8.2.0
-- Версия PHP: 8.2.18

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- База данных: `Tattoo_parlors`
--

-- --------------------------------------------------------

--
-- Структура таблицы `Appointment`
--

CREATE TABLE `Appointment` (
  `id` int NOT NULL,
  `tattooArtistId` int NOT NULL,
  `date` datetime NOT NULL,
  `slotNumber` int NOT NULL,
  `userId` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `Appointment`
--

INSERT INTO `Appointment` (`id`, `tattooArtistId`, `date`, `slotNumber`, `userId`) VALUES
(13, 6, '2025-06-23 00:00:00', 1, 4);

-- --------------------------------------------------------

--
-- Структура таблицы `Message`
--

CREATE TABLE `Message` (
  `id` int NOT NULL,
  `content` text NOT NULL,
  `senderId` int NOT NULL,
  `receiverId` int DEFAULT NULL,
  `timestamp` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `Message`
--

INSERT INTO `Message` (`id`, `content`, `senderId`, `receiverId`, `timestamp`) VALUES
(30, 'r+vJo2iyk5/fEbruhuVjZ+ANu7CAxdONArgyQSEX6QSf0l1cVcbgHB38FEFbu5guXI06E205WmQ4ZCqe0oVAScepOlIHpmGuMpMGDf6TT+24jjliACtlegeDPMRomI2l', 4, 5, '2025-06-16 23:11:20'),
(31, 'togWpPhG1cCLKyoQFIpZGJ+8shQW2al2bhzDVr8PPG6tCpaICPyYlJ1AG5KoHwo+BJn4+9ZTC7tC13cmJqPUiw==', 5, 4, '2025-06-16 23:11:50');

-- --------------------------------------------------------

--
-- Структура таблицы `SalonInfo`
--

CREATE TABLE `SalonInfo` (
  `id` int NOT NULL,
  `name` varchar(40) NOT NULL,
  `address` varchar(40) NOT NULL,
  `phone_number` varchar(40) NOT NULL,
  `telegram` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `instagram` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `vk` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `glav_info` text NOT NULL,
  `o_nas_info` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `SalonInfo`
--

INSERT INTO `SalonInfo` (`id`, `name`, `address`, `phone_number`, `telegram`, `instagram`, `vk`, `glav_info`, `o_nas_info`) VALUES
(4, 'Чернильный Лабиринт', 'ул. Кручево 16', '+375 29 111 11 11', '', '', '', 'это место, где воплощается ваше воображение в уникальные дизайны. Здесь работают талантливые специалисты, способные создать настоящие произведения искусства на коже.', 'В салоне \"Чернильный Лабиринт\" татуировка превращается в живое произведение искусства. Мы — не просто мастера, мы творцы историй, где каждая линия и штрих отражают уникальность вашей личности. Наша миссия — сделать так, чтобы ваше воображение нашло отражение на коже, превращая каждую работу в эксклюзивный арт-объект.\r\n\r\nНаш коллектив — это команда талантливых художников-татуировщиков, объединённых страстью к творчеству и стремлением к совершенству. Мы работаем в различных стилях: от чистого минимализма до сложных, насыщенных деталями эскизов. Каждый из наших мастеров имеет свой индивидуальный подход, что позволяет подобрать именно тот дизайн, который подчеркнёт вашу индивидуальность и станет символом внутренней силы.\r\n\r\nБезопасность и комфорт наших клиентов — наш приоритет. Мы используем инновационные технологии и строго соблюдаем все санитарные нормы, чтобы каждая сессия проходила в атмосфере доверия и профессионализма. Наши инструменты и краски соответствуют самым высоким стандартам, что гарантирует безупречный результат и бережное отношение к вашему здоровью.\r\n\r\nМы верим, что татуировка — это не просто рисунок на теле, а знак важного жизненного этапа, отражение души и смелости. Приглашаем вас окунуться в мир искусства, где каждая работа рассказывает уникальную историю и становится вашим личным символом. Добро пожаловать в \"Чернильный Лабиринт\" — место, где начинается ваше путешествие по миру вдохновения.');

-- --------------------------------------------------------

--
-- Структура таблицы `ScheduleTemplate`
--

CREATE TABLE `ScheduleTemplate` (
  `id` int NOT NULL,
  `tattooArtistId` int NOT NULL,
  `slotNumber` int NOT NULL,
  `startTime` time NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `ScheduleTemplate`
--

INSERT INTO `ScheduleTemplate` (`id`, `tattooArtistId`, `slotNumber`, `startTime`) VALUES
(9, 6, 1, '16:30:00'),
(10, 6, 2, '17:32:00');

-- --------------------------------------------------------

--
-- Структура таблицы `Services`
--

CREATE TABLE `Services` (
  `id` int NOT NULL,
  `name` varchar(40) NOT NULL,
  `price` varchar(20) NOT NULL,
  `description` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `Services`
--

INSERT INTO `Services` (`id`, `name`, `price`, `description`) VALUES
(2, 'Тату', 'Зависит от эскиза', 'Цена обсуждается с тату мастером ');

-- --------------------------------------------------------

--
-- Структура таблицы `Sketch`
--

CREATE TABLE `Sketch` (
  `id` int NOT NULL,
  `tattooistId` int NOT NULL,
  `imagePath` text NOT NULL,
  `description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `Sketch`
--

INSERT INTO `Sketch` (`id`, `tattooistId`, `imagePath`, `description`) VALUES
(14, 6, '/images/sketches/d70c128b-2824-45ad-9fb6-a77e55ac3c3b_2fa758ef5f60e37c13a04b5d3df64f1a.jpg', '1'),
(15, 6, '/images/sketches/9eef61bb-3f15-4d8c-9366-70b027969d0a_334ef89a99e2f396cada5d806b7e75f7.jpg', '2'),
(16, 6, '/images/sketches/dddb00d7-62e1-48fa-b6a2-2a2ff9e4a475_04297e1f149906b7bc65fc3f506c4df7.jpg', '3'),
(17, 6, '/images/sketches/e42ae596-c025-4d19-a1b1-3f6844388601_612394594dd9bc013a1546c4531682fa.jpg', '4'),
(18, 6, '/images/sketches/0431f6e0-128e-47b1-83af-d45f2175274b_e40c52740be9fbb22c4394892126df55.jpg', '5'),
(19, 7, '/images/sketches/f0af765c-dc5e-484b-a7b9-b287aba443ca_10_5eTLP80.2e16d0ba.fill-360x360-c100.format-jpeg.jpg', '1'),
(20, 7, '/images/sketches/430c40fa-0503-4fb5-a5b3-d3380c2eff5a_1564906951137078568.jpg', '2'),
(21, 7, '/images/sketches/ec537694-9144-4b59-827d-cf5c5f457897_hiGvg_8vmPs.jpg', '3'),
(22, 8, '/images/sketches/7acdae1f-c8a8-4c98-8c75-31ae45290ade_5a9f3cd1d5b10f83fb75.jpg', 'Данный эскиз объединяет минимализм с тщательно проработанными геометрическими элементами и экспрессивными акцентами. Чистота линий и строгие контуры создают ощущение гармонии и контроля, отражая внутреннюю силу и индивидуальность владельца. В центре композиции располагается стилизованный символ, который можно интерпретировать как знак вечного потока времени, перемен или личного пути, что придаёт татуировке глубокий смысл.\r\n\r\nКаждая линия и изгиб выверены до мельчайших деталей: резкие углы плавно сменяются мягкими кривыми, подчеркивая динамику и энергию переходов. Такой баланс делает эскиз универсальным — он отлично подойдёт как для компактного акцента на запястье, так и для полноценного произведения искусства на спине или руке. Слияние индустриальной эстетики с элементами фэнтези и мифологии придает работе современный, смелый характер, позволяющий владельцу выразить свою уникальную историю через символы и образы.');

-- --------------------------------------------------------

--
-- Структура таблицы `Tattooist`
--

CREATE TABLE `Tattooist` (
  `id` int NOT NULL,
  `name` varchar(20) NOT NULL,
  `biography` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `Tattooist`
--

INSERT INTO `Tattooist` (`id`, `name`, `biography`) VALUES
(6, 'Яна Проторская', ' талантливая тату-мастер, известная своим вниманием к деталям и индивидуальным подходом к каждому клиенту. Её стиль — это сочетание художественной выразительности и минималистичных форм, благодаря чему татуировки выглядят эстетично и со вкусом. Работы Яны отличают чистота линий, аккуратность и глубокий смысл, заложенный в каждом эскизе.\r\n\r\nПочему выбирают Яну:\r\n\r\nИндивидуальный подход к каждому клиенту\r\n\r\nЭскизы, созданные с нуля под запрос\r\n\r\nСоблюдение всех норм стерильности\r\n\r\nПриятная атмосфера в процессе работы'),
(7, 'Максим Куликов', 'Максим начал свою карьеру как уличный художник-граффитист. В 23 года решил перенести свои работы с городских стен на кожу. Учился татуировке в Германии, вдохновлялся европейской школой графики. Его фирменный стиль — яркие, выразительные образы с элементами фэнтези и мифологии. Клиенты ценят его за детализированную прорисовку и аккуратную линию. Открыл собственную студию \"Fox & Ink\" в центре Питера.'),
(8, 'Ольга Сереброва', 'Ольга начинала как иллюстратор и дизайнер, увлечённый ботаникой. Её путь в тату начался с маленькой машинки, подаренной подругой на день рождения. Сегодня её стиль узнаваем — тончайшие линии, органика, элементы японской эстетики. Часто сотрудничает с косметологами и делает татуировки после мастэктомии. Ведёт блог об этике и психологии тату-мастера, популярный среди начинающих художников.'),
(9, 'Артём Власов', 'Артём более 15 лет работает в сфере татуировки. Прошёл путь от любителя до признанного мастера восточного направления. Несколько лет жил в Токио, где обучался традиционному японскому тату (ирэдзуми) у местных мастеров. В его работах часто присутствуют драконы, карпы, самураи и элементы природы. Известен своим философским подходом: каждый эскиз — это символ и история. Руководит школой для молодых татуировщиков.'),
(10, 'Денис Малинов', 'Самый молодой из команды, Денис — настоящий экспериментатор. Его тату — это взрыв цвета, формы и эмоций. Учился в художественном училище, увлекался стрит-артом и цифровым искусством. Постепенно перешёл в тату-индустрию, совмещая традиционные техники с дерзким сюрреализмом. Часто работает на международных фестивалях и обожает нестандартные запросы — от тату на голове до трансформации шрамов в арт.');

-- --------------------------------------------------------

--
-- Структура таблицы `Users`
--

CREATE TABLE `Users` (
  `id` int NOT NULL,
  `name` varchar(20) NOT NULL,
  `surname` varchar(20) NOT NULL,
  `password` text NOT NULL,
  `numer` text NOT NULL,
  `email` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `role` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `Users`
--

INSERT INTO `Users` (`id`, `name`, `surname`, `password`, `numer`, `email`, `role`) VALUES
(4, 'Александр', 'Яхимович', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', '80291673348', 'sasaacsgo@gmail.com', 0),
(5, 'Админ', 'Админ', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', '80291673348', 'sasaahimovic@gmail.com', 1);

--
-- Индексы сохранённых таблиц
--

--
-- Индексы таблицы `Appointment`
--
ALTER TABLE `Appointment`
  ADD PRIMARY KEY (`id`),
  ADD KEY `tattooArtistId` (`tattooArtistId`),
  ADD KEY `userId` (`userId`);

--
-- Индексы таблицы `Message`
--
ALTER TABLE `Message`
  ADD PRIMARY KEY (`id`),
  ADD KEY `senderId` (`senderId`),
  ADD KEY `receiverId` (`receiverId`);

--
-- Индексы таблицы `SalonInfo`
--
ALTER TABLE `SalonInfo`
  ADD PRIMARY KEY (`id`);

--
-- Индексы таблицы `ScheduleTemplate`
--
ALTER TABLE `ScheduleTemplate`
  ADD PRIMARY KEY (`id`),
  ADD KEY `tattooArtistId` (`tattooArtistId`);

--
-- Индексы таблицы `Services`
--
ALTER TABLE `Services`
  ADD PRIMARY KEY (`id`);

--
-- Индексы таблицы `Sketch`
--
ALTER TABLE `Sketch`
  ADD PRIMARY KEY (`id`),
  ADD KEY `tattooistId` (`tattooistId`);

--
-- Индексы таблицы `Tattooist`
--
ALTER TABLE `Tattooist`
  ADD PRIMARY KEY (`id`);

--
-- Индексы таблицы `Users`
--
ALTER TABLE `Users`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT для сохранённых таблиц
--

--
-- AUTO_INCREMENT для таблицы `Appointment`
--
ALTER TABLE `Appointment`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- AUTO_INCREMENT для таблицы `Message`
--
ALTER TABLE `Message`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=32;

--
-- AUTO_INCREMENT для таблицы `SalonInfo`
--
ALTER TABLE `SalonInfo`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT для таблицы `ScheduleTemplate`
--
ALTER TABLE `ScheduleTemplate`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT для таблицы `Services`
--
ALTER TABLE `Services`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=18;

--
-- AUTO_INCREMENT для таблицы `Sketch`
--
ALTER TABLE `Sketch`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=23;

--
-- AUTO_INCREMENT для таблицы `Tattooist`
--
ALTER TABLE `Tattooist`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT для таблицы `Users`
--
ALTER TABLE `Users`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- Ограничения внешнего ключа сохраненных таблиц
--

--
-- Ограничения внешнего ключа таблицы `Appointment`
--
ALTER TABLE `Appointment`
  ADD CONSTRAINT `appointment_ibfk_1` FOREIGN KEY (`tattooArtistId`) REFERENCES `Tattooist` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  ADD CONSTRAINT `appointment_ibfk_2` FOREIGN KEY (`userId`) REFERENCES `Users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT;

--
-- Ограничения внешнего ключа таблицы `Message`
--
ALTER TABLE `Message`
  ADD CONSTRAINT `message_ibfk_1` FOREIGN KEY (`senderId`) REFERENCES `Users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  ADD CONSTRAINT `message_ibfk_2` FOREIGN KEY (`receiverId`) REFERENCES `Users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT;

--
-- Ограничения внешнего ключа таблицы `ScheduleTemplate`
--
ALTER TABLE `ScheduleTemplate`
  ADD CONSTRAINT `scheduletemplate_ibfk_1` FOREIGN KEY (`tattooArtistId`) REFERENCES `Tattooist` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT;

--
-- Ограничения внешнего ключа таблицы `Sketch`
--
ALTER TABLE `Sketch`
  ADD CONSTRAINT `sketch_ibfk_1` FOREIGN KEY (`tattooistId`) REFERENCES `Tattooist` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
