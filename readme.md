# Project
This is a follow up test project based on my [code-first](https://github.com/yosang/csharp-efcore-code-first) example where we reverse engineer an existing database and bring it into code with Entity Framework Core.

Here we are scaffolding our existing database, migrated by the `code-first` example.

The command used: 
```bash
dotnet ef dbcontext scaffold "server=localhost;database=testdb;user=testuser;password=p@ssword" MySql.EntityFrameworkCore -o Models --table Brands --table Categories --table Tools
```

To use this command the following packages were added:
- Mirosoft.EntityFrameworkCore.Design
- MySQL.EntityFrameworkCore

# Usage
1. Clone the repo
2. Since this application is a scaffold of an existing database, the original database schema is required.
    - You can implement the database using the [code-first](https://github.com/yosang/csharp-efcore-code-first) example.
3. Run the application with `dotnet run`

# Author
[Yosmel Chiang](https://github.com/yosang)