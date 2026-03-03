# Project
This is a follow up test project based on my [code-first](https://github.com/yosang/csharp-efcore-code-first) example where we "ideally" implement the database first, then bring it into code with Entity Framework Core.

Here we are scaffolding our existing database, migrated by the `code-first` example.

The command used: `dotnet ef dbcontext scaffold "server=localhost;database=testdb;user=testuser;password=p@ssword" MySql.EntityFrameworkCore -o Models --table Brands --table Categories --table Tools`

To use this command the following packages were added:
- Mirosoft.EntityFrameworkCore.Design
- MySQL.EntityFrameworkCore