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

# Stored Procedures
In this project, we are testing out how to access and call stored procedures from the Entity Data Model.

There are a few methods available:

## Querying data:
- To query data using `DbSet` properties we can use the `FromSql()` method.
    - It uses an interpolated string that represents the SQL query.
    - Example calling a parameterless stored precedure that returns Tools ending with the letter `r`.
        ```c#
         var toolsThatEndWithR = context.Tools
            .FromSql($"CALL sp_getRTools()")
            .ToList();
        ```
## Inserting, adding, deleting data:
- To perform an action on the database we can use the `DbContext.Database` method `ExecuteSql()`.
    - Often we want to pass an argument to a parametirized procedure, in this case we want to use the `MySqlParameter` object to safely inject an input.
    - When using interpolation to pass an input, it is important to protect against SQL injection.
    - Example
        ```c#
        // Stored procedure - Add a new brand
        // MySqlParameter takes 
        // 1. The name of the parameter on the stored procedure
        // 2. The value to use as argument
        // We can then use parameter when callling the procedure.
        var parameter = new MySqlParameter("@catName", "Clamps");
        context.Database.ExecuteSql($"CALL sp_addCategory({parameter})");
        ```

## Other methods
- `FromSqlInterpolated` - Same as `FromSql`
- `ExecuteSqlInterpolated` - Same as `ExecuteSql`
- `FromSqlRaw` - Unsafe, should avoid, its only useful when building SQL dynamically by concatenation: `"WHERE Name = '" + userInput + "'"`, introduces SQL injection risks.
- `ExecuteSqlRaw` - Same principle as `FromSqlRaw`

# Basic Stored Procedure syntax

- `backticks` - Used for identifiers, such as a procedure name, we can use it if the name of the procedure contains spaces/special characters, for example `sp getRTools` even though here we are using `sp_getRTools`, so we dont need backticks.
- `single/double quotes` - Used for values in `VALUES`, `WHERE` conditions etc.
- `DELIMITER` - MySQL uses `;` by default, however when creating procedures, we end an internal statement with `;`, so to avoid conflict we change the delimiter to end the procedure with a different symbol, `//` is commonly used.
    ```sql
    DELIMITER //               -- from now on, wait for // instead of ; to end the statement

    CREATE PROCEDURE sp_addCategory(IN catName LONGTEXT)
    BEGIN
        INSERT INTO Categories(Name) VALUES (catName);   -- this is fine is fine now, client ignores it
    END //                     -- client sees //, sends everything above to server as one piece

    DELIMITER ;                -- switch back to default delimiter
    ```


## Create a parameterless procedure
```sql
CREATE PROCEDURE sp_getRTools()
BEGIN
SELECT * FROM Tools
WHERE Name Like "%R";
```

## Create a parameterised procedure
```sql
DELIMITER //
CREATE PROCEDURE sp_addCategory(IN catName longtext)
BEGIN
    INSERT INTO Categories(Name)
    VALUES(catName);
END //

DELIMITER ;
```

## Dropping a stored procedure
```sql
DROP PROCEDURE sp_addCategory;
```

## List stored procedures
```sql
SHOW PROCEDURE STATUS WHERE Db = DATABASE();
```

List a specific procedure and its definition code.
```sql
SHOW CREATE PROCEDURE sp_getRTools;
```

# Usage
1. Clone the repo
2. Since this application is a scaffold of an existing database, the original database schema is required.
    - You can implement the database using the [code-first](https://github.com/yosang/csharp-efcore-code-first) example.
3. Run the application with `dotnet run`

# Author
[Yosmel Chiang](https://github.com/yosang)