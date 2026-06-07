# AspKnP231
Після клонування репозиторію необхідно відкрити пакетну консоль
Tools - NuGet Package Manager - Package Manager Console
та ввести команду
`Update-Database`

У разі виникнення конфліктів з існуючою БД слід змінити рядок
підключення у файлі `appsettings.json`, наприклад, замість 
фрагменту `Initial Catalog=AspKnP231` зазначити `Initial Catalog=AspKnP231_2`
і знов виконати команду
`Update-Database`

Або, якщо не змінювати рядок підключення, видалити всі таблиці з існуючої БД `AspKnP231`