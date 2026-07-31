# CourseBuilder
Это платформа онлайн-курсов, на ней можно создавать и изучать образовательные курсы. На платформе есть отслеживание прогресса по курсам, аналитика по курсам для разработчиков курсов, импорт нескольких студентов-пользователей через .csv файлы, ведение учебных групп, получение успеваемости группы по учебному курсу в формате .xlsx

Роли пользователей: администраторы, разработчики курсов и студенты. 

Гости могут смотреть каталог курсов, студенты могут приобретать курсы и проходить их, разработчики могут создавать и редактировать курсы, вести учебные группы, а администраторы управляют аккаунтами пользователей, рассматривают обращения пользователей.

Веб-сайт разработан на Vue.js 3 + TypeScript

Дизайн сделан с использованием Bootstrap 5 + BootstrapVueNext

Части системы:
- База данных PostgreSQL ([Entites и DTO](https://github.com/Afya2208/CourseBuilder/tree/main/Models))
- [Web-API ASP.NET Core](https://github.com/Afya2208/CourseBuilder/tree/main/API)
- [Веб-сайт на Vue.js + TypeScript](https://github.com/Afya2208/CourseBuilder/tree/main/Website)

[Авто-тесты платформы](https://github.com/Afya2208/CourseBuilder/tree/main/Tests)

## Tech Stack
* PostgreSQL 18
* Vue.js 3
* .NET 9
* ASP.NET Core 9
* EntityFramework Core 9
* XUnit
* Selenium C#

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
