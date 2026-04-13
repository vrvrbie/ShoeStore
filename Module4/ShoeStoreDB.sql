-- =====================================================
-- МОДУЛЬ 1. РАЗРАБОТКА БАЗЫ ДАННЫХ
-- Студент: Салихова В.Д.
-- =====================================================

-- СОЗДАНИЕ ТАБЛИЦ

-- Роли пользователей
create table Roles (
    RoleID serial primary key,
    RoleName varchar(50) not null unique
);

-- Пользователи
create table Users (
    UserID serial primary key,
    UserLogin varchar(100) not null unique,
    UserPasswordHash varchar(255) not null,
    UserFullName varchar(150) not null,
    RoleID int not null references Roles(RoleID)
);

-- Категории товаров
create table Categories (
    CategoryID serial primary key,
    CategoryName varchar(100) not null unique
);

-- Производители
create table Manufacturers (
    ManufacturerID serial primary key,
    ManufacturerName varchar(150) not null unique
);

-- Поставщики
create table Suppliers (
    SupplierID serial primary key,
    SupplierName varchar(150) not null unique,
    SupplierPhone varchar(20),
    SupplierEmail varchar(100)
);

-- Товары
create table Products (
    ProductID serial primary key,
    ProductArticle varchar(20) not null unique,  
    ProductName varchar(200) not null,
    ProductUnit varchar(20) not null default 'шт',
    ProductPrice DECIMAL(10,2) not null check (ProductPrice >= 0),
    SupplierID int not null references Suppliers(SupplierID),
    ManufacturerID int not null references Manufacturers(ManufacturerID),
    CategoryID int not null references Categories(CategoryID),
    DiscountPercent decimal(5,2) not null default 0 check (DiscountPercent >= 0 and DiscountPercent <= 100),
    QuantityInStock int not null check (QuantityInStock >= 0),
    Description text,
    ImagePath varchar(500)
);

-- Пункты выдачи
create table PickupPoints (
    PointID serial primary key,
    Address varchar(300) not null unique
);

-- Заказы
create table Orders (
    OrderID serial primary key,
    OrderNumber int not null unique,  
    OrderDate date not null,
    DeliveryDate date,
    PickupPointID int not null references PickupPoints(PointID),
    UserID int not null references Users(UserID),
    OrderPickupCode int not null,  
    OrderStatus varchar(50) not null default 'Новый'
);

-- Состав заказа
create table OrderDetails (
    OrderID int not null,
    ProductID int not null,
    Quantity int not null check (Quantity > 0),
    PriceAtOrder decimal(10,2) not null check (PriceAtOrder >= 0),
    DiscountAtOrder decimal(5,2) not null default 0,
    primary key (OrderID, ProductID),
    foreign key (OrderID) references Orders(OrderID) on delete cascade,
    foreign key (ProductID) references Products(ProductID)
);

-- ЗАПОЛНЕНИЕ СПРАВОЧНИКОВ

insert into Categories (CategoryName) values
('Женская обувь'), ('Мужская обувь');

insert into Suppliers (SupplierName) values
('Kari'), ('Обувь для вас');

insert into Manufacturers (ManufacturerName) values
('Kari'), ('Marco Tozzi'), ('Рос'), ('Rieker'), ('Alessio Nesca'), ('CROSBY');

insert into Roles (RoleName) values
('Гость'), ('Авторизованный клиент'), ('Менеджер'), ('Администратор');

-- ИМПОРТ СОСТАВА ЗАКАЗОВ

INSERT INTO OrderDetails (OrderID, ProductID, Quantity, PriceAtOrder, DiscountAtOrder) VALUES
(1, 1, 2, 4990, 3),
(1, 2, 2, 3244, 2),
(2, 3, 1, 4499, 4),
(2, 4, 1, 5900, 2),
(3, 5, 10, 3800, 2),
(3, 6, 10, 4100, 3),
(4, 7, 5, 2700, 2),
(4, 8, 4, 1890, 4),
(5, 1, 2, 4990, 3),
(5, 2, 2, 3244, 2),
(6, 3, 1, 4499, 4),
(6, 4, 1, 5900, 2),
(7, 5, 10, 3800, 2),
(7, 6, 10, 4100, 3),
(8, 7, 5, 2700, 2),
(8, 8, 4, 1890, 4),
(9, 9, 5, 4300, 2),
(9, 10, 1, 2800, 3),
(10, 11, 5, 2156, 3),
(10, 12, 5, 1800, 2);

-- Модуль 4

create table OrderStatuses (
    StatusID serial primary key,
    StatusName varchar(50) not null unique
);

insert into OrderStatuses (StatusName) values 
('Новый'),
('В обработке'),
('Собран'),
('Выдан'),
('Отменён');

alter table Orders add column OrderStatusID int references OrderStatuses(StatusID);

update Orders set OrderStatusID = 4 where OrderStatus = 'Завершен';
update Orders set OrderStatusID = 1 where OrderStatus = 'Новый';