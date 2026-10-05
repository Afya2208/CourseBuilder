# CourseBuilder

Это платформа онлайн-курсов, на ней можно создавать и изучать образовательные курсы. 
На платформе есть отслеживание прогресса по курсам, аналитика по курсам для разработчиков курсов, 
импорт нескольких студентов-пользователей через .csv файлы, ведение учебных групп.

Роли пользователей: гости (неавторизованные пользователи), администраторы, разработчики курсов и студенты. 

Гости могут смотреть каталог курсов и наборов курсов, 
студенты приобретают курсы, наборы курсов и могут проходить их, 
разработчики курсов создают, редактируют курсы, ведут учебные группы (можно скачать отчет об успеваемости группы по курсам), 
администраторы управляют аккаунтами пользователей, рассматривают обращения пользователей.

Веб-сайт разработан на Vue.js 3 + TypeScript. Дизайн сделан с использованием Bootstrap 5 и BootstrapVueNext.
Backend на .NET 10.

Части системы и инструкции по запуску:
- База данных PostgreSQL ([Entites, Domain](https://github.com/Afya2208/CourseBuilder/tree/main/Models))
- [Web-API ASP.NET Core](https://github.com/Afya2208/CourseBuilder/tree/main/API)
- [Веб-сайт на Vue.js + TypeScript](https://github.com/Afya2208/CourseBuilder/tree/main/Website)

[Авто-тесты платформы на xUnit](https://github.com/Afya2208/CourseBuilder/tree/main/Tests)

## Стек технологий
* Backend: .NET 10, PostgreSQL 18, ASP.NET Core 10, EntityFramework Core 10
* Frontend: Vue.js 3, TypeScript, Bootstrap 5, BootstrapVueNext
* Testing: xUnit, Selenium C#

## Скриншоты работы

### Страница курсов, главная страница
<img src="Website%20Screenshots/1.png" width="1000" height="550">

### Подвал страницы курсов
<img src="Website%20Screenshots/1.2.png" width="1000" height="200">

### Страница наборов курсов
<img src="Website%20Screenshots/2.png" width="1000" height="600">

### Страница курса
<img src="Website%20Screenshots/4.png" width="1000" height="500">

### Страница занятия
<img src="Website%20Screenshots/5.png" width="1000" height="600">

### Задания занятия
<img src="Website%20Screenshots/6.png" width="1000" height="500">

### Страница занятия в режиме редактирования
<img src="Website%20Screenshots/7.png" width="900" height="550">

### Аналитика по курсам
<img src="Website%20Screenshots/8.png" width="900" height="600">

## Как запустить CourseBuilder API и Website

Для запуска CourseBuilder Website и Web-API требуется Node.js и .NET 10 на компьютере.

Сначала нужно скопировать репозиторий, вставив полный URL адрес репозитория:

```shell
git clone https://github.com/...
```

### Шаги, чтобы запустить CourseBuilder Web-API

1. Поменять настройки в файле appsettings.json:
- ConnectionStrings:Default - нужно указать данные для подключения к СУБД и доступное имя базы данных, так как позже надо по этим параметрам восстановить базу данных
- JWT - настройки токенов JWT
- Serilog - настройка логирования

2. Воссоздать базу данных из Domain.Entities. Строка подключения и имя будут использоваться из Web API проекта. Выполнить команды внутри папки Domain:
```shell
dotnet ef migrations add Init -s ../API
dotnet ef database update
```

3. Для запуска в корне папки API выполнить команду:

```shell
dotnet run
```
Или в корне CourseBuilder:
```shell
dotnet run --project API
```

### Шаги, чтобы запустить CourseBuilder Website:

1. Настроить и запустить Web-API, см. шаги выше
2. Для запуска в корне папки Website выполнить команды:

```shell
npm install
npm run dev
```