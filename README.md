# MusicWorld

A .NET Core web app - project.

Used working environment and tools: DOT.NET Core 3.1, Entity Frameowork Core (EF Core), Visual Studio 2017/2019, Microsoft SQL Server Management Studio 18

Set up: The app connects to Microsoft SQL Server (.\express) by default. To change it, change the connection string file. To establish connections to your database go to Visual Studio > Package Manager Console and write the command update-database (make sure that the target project is MusicWorld.Data).
When starting the app, make sure that the target project is MusicWorld.Web.

Idea of the app: The very first registered user gets the role admin, every next – default user role. Admins can create performers, albums, songs. Every album has its performer. Every song is part of exactly one album. All users (including admins) can create their own catalogs (albums). Using the detailed view of your albums, you can easily add/remove songs from it.
