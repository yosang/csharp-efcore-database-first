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

# Concepts covered
## Overview
- Stored procedures
- Eager, Lazy and Explicit loading

## Stored Procedures
In this project, we are testing out how to access and call stored procedures from the Entity Data Model.

There are a few methods available:

### Querying data:
- To query data using `DbSet` properties we can use the `FromSql()` method.
    - It uses an interpolated string that represents the SQL query.
    - Example calling a parameterless stored precedure that returns Tools ending with the letter `r`.
        ```c#
         var toolsThatEndWithR = context.Tools
            .FromSql($"CALL sp_getRTools()")
            .ToList();
        ```
### Inserting, adding, deleting data:
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

### Other methods
- `FromSqlInterpolated` - Same as `FromSql`
- `ExecuteSqlInterpolated` - Same as `ExecuteSql`
- `FromSqlRaw` - Unsafe, should avoid, its only useful when building SQL dynamically by concatenation: `"WHERE Name = '" + userInput + "'"`, introduces SQL injection risks.
- `ExecuteSqlRaw` - Same principle as `FromSqlRaw`

## Basic Stored Procedure syntax

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

### Create a parameterless procedure
```sql
CREATE PROCEDURE sp_getRTools()
BEGIN
SELECT * FROM Tools
WHERE Name Like "%R";
```

### Create a parameterised procedure
```sql
DELIMITER //
CREATE PROCEDURE sp_addCategory(IN catName longtext)
BEGIN
    INSERT INTO Categories(Name)
    VALUES(catName);
END //

DELIMITER ;
```

### Dropping a stored procedure
```sql
DROP PROCEDURE sp_addCategory;
```

### List stored procedures
```sql
SHOW PROCEDURE STATUS WHERE Db = DATABASE();
```

List a specific procedure and its definition code.
```sql
SHOW CREATE PROCEDURE sp_getRTools;
```

## Eager Loading
We can load related entitites immediately by using a single LINQ query together with the main entity model.
- We simply use `Include()` and `ThenInclude()` to bring in associated models.
    - `Include()` - Brings in the first child
    - `Include()` - Brings in the child of the first child.

For example if we want to branch down to a child model of `Tools`, say `Brands`:
```c#
    var toolsAndBrands = context.Tools.Include(t => t.Brand);
    foreach (var t in toolsAndBrands)
    {
        Console.WriteLine($"Id: {t.Id} - Name: {t.Name} - Brand: {t.Brand?.Name}");
    }
```

Lets now say that `Brands` has `Suppliers`:

```c#
    var toolsAndBrandsAndSuppliers = context.Tools.Include(t => t.Brand).ThenInclude(b => b.Supplier);
    foreach (var t in toolsAndBrandsAndSuppliers)
    {
        Console.WriteLine($"Id: {t.Id} - Name: {t.Name} - Brand: {t.Brand?.Name} - Supplier: {t.Brand?.Supplier?.Name}");
    }
```

Its possible to use multple Includes, say we want to bring both `Tools`, `Brands` and `Categories`
```c#
    var everything = context.Tools.Include(t => t.Brand).Include(t => t.Categories);
    foreach (var t in everything)
    {
        Console.WriteLine($"Id: {t.Id} - Name: {t.Name} - Brand: {t.Brand?.Name} - Supplier: {t.Categories?.Name}");
    }
```

## Lazy Loading
Lazy Loading happens when we access an entity model and explicitly access a related model through its navigation property.

In order for this to work we need a few things first:
- A `virtual` navigation property on `Tools`.
    ```c#
        public virtual Category Category { get; set; } = null!;
    ```
- The package `Microsoft.EntityFrameworkCore.Proxies`.
- And the `UseLazyLoadingProxies()` configuration in `DbContext`.
    ```c#
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseMySQL("server=localhost;database=testdb;user=testuser;password=p@ssword")
            .UseLazyLoadingProxies();
    ```

Example: 
```c#
    var onlyTools = context.Tools.ToList();

    foreach (var t in onlyTools)
    {
        Console.WriteLine($"Name: {t.Name} - Category: {t.Category?.Name}"); // Lazy loading through navigation property
    }
```

As we can see, unlike `Eager Loading`, where we have to specify in the query that we want related data through `Include()` and `ThenInclude()` before we can access the data.

In `Lazy Loading` we dont have to do that, data is related data is loaded WHEN we enumerate through the navigation properties.

## Explicit Loading
A different approach that provides more control, we have to explicitly load the related data with `.Load`, before we can use it. 

- Unlike `lazy loading`, `explicit loading` is similar, but it doesnt require extra setup and we can disable `lazy loading` all together.
- For single entities `public Brand Brand { get; set; } = null!;`, we `.Load` using `.Reference`.
- For collections of entities `public ICollection<Tool> Tools { get; set; } = new List<Tool>();`, we `.Load` using `.Collection`

Notice that none of the entitie models have `virtual`, which means they are `lazy loading` disabled.

```c#
        // Explicit Loading
        var tool = context.Tools.FirstOrDefault();
        if (tool != null)
        {
            context.Entry(tool).Reference(t => t.Brand).Load();
            Console.WriteLine($"Name: {tool.Name} - Brand: {tool.Brand?.Name}"); // Loads the name of the brand for the first tool retrieved (Explicit loading)
        }

        // Similarly for a list of tools
        var tools = context.Tools.ToList();
        foreach (var t in tools)
        {
            context.Entry(t).Reference(t => t.Brand).Load();
            Console.WriteLine($"Name: {t.Name} - Brand: {t.Brand?.Name}"); // Loads the name of the brand for the first tool retrieved (Explicit loading)
        }

        // We are using Reference because our navigation property is a single entity, if it were a collection we would use
        // .Collection instead:
        // The relationship model here is One-To-Many, a Brand has many Tools
        var brand = context.Brands.FirstOrDefault();
        if (brand != null)
        {
            context.Entry(brand).Collection(t => t.Tools).Load();
            Console.WriteLine($"Name: {brand.Name}");

            foreach (var t in brand.Tools)
            {
                Console.WriteLine($"Tools name: {t.Name}");
            }
        }
```

# Usage
1. Clone the repo
2. Since this application is a scaffold of an existing database, the original database schema is required.
    - You can implement the database using the [code-first](https://github.com/yosang/csharp-efcore-code-first) example.
3. Run the application with `dotnet run`

# Author
[Yosmel Chiang](https://github.com/yosang)