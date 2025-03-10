Цель проекта DLForum — создать динамичный и удобный форум для обсуждений с использованием ASP.NET MVC Core 8. Форум должен обеспечивать регистрацию пользователей, создание тем, комментариев, а также возможность фильтрации контента с использованием категорий и тегов. Система должна быть безопасной, надежной и масштабируемой.

Установка
1.	Открыть любой браузер (Opera, Edge, Yandex Browser и т.д.)
2.	Зайти на сайт http://www.dlforum.somee.com
3.	Запуск завершен

Примеры использования
1.	Регистрация нового пользователя Пользователи могут зарегистрироваться, заполнив форму с логином, email и паролем. После успешной регистрации, они получают доступ к функционалу форума, таким как создание тем и комментариев.
2.	Создание новой темы Зарегистрированные пользователи могут создавать новые темы в выбранных категориях, указывая заголовок, описание и теги. После отправки темы она будет доступна для других пользователей.
3.	Добавление комментариев Пользователи могут оставлять комментарии под темами. Каждый комментарий сохраняется в базе данных и отображается на странице соответствующей темы.
4.	Переключение между темной и светлой темами Форум предоставляет возможность переключаться между темной и светлой темами. Переключение сохраняется в профиле пользователя для обеспечения постоянного предпочтения.
5.	Навигация по категориям Пользователи могут легко перемещаться между категориями форума и просматривать список доступных тем в каждой категории.

Зависимости
1.	ASP.NET Core 8.0
•	Для создания веб-приложений с использованием MVC, а также для работы с API и маршрутизацией.
2.	Entity Framework Core
•	Для работы с базой данных, миграциями и ORM (Object-Relational Mapping).
3.	SQL Server / PostgreSQL / SQLite (в зависимости от выбранной базы данных)
•	Для хранения данных пользователей, тем, комментариев и других элементов форума.
4.	JWT (JSON Web Token)
•	Для реализации аутентификации и авторизации пользователей.
5.	Bootstrap 
•	Для стилизации интерфейса форума и обеспечения адаптивного дизайна.
6.	Microsoft.Extensions.Logging
•	Для логирования событий и ошибок в приложении.
