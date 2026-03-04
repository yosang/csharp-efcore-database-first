using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using test.Models;

public class Program
{
    public static void Main()
    {
        using var context = new TestdbContext();

        // Simple query to test the database
        // var tools = context.Tools
        //                     .Include(t => t.Brand)
        //                     .Include(t => t.Category)
        //                     .ToList();

        // foreach (var t in tools)
        // {
        //     Console.WriteLine($"Id: {t.Id} - Name: {t.Name} - Brand: {t.Brand?.Name} - Category: {t.Category?.Name}");
        // }

        // Stored Procedure - Get tools that end with an r
        // DELIMITER //
        // CREATE PROCEDURE sp_getRTools()
        // BEGIN
        // SELECT * FROM Tools
        // WHERE Name Like "%R";
        // END //
        // var toolsThatEndWithR = context.Tools
        //     .FromSql($"CALL sp_getRTools()")
        //     .ToList();

        // foreach (var t in toolsThatEndWithR)
        // {
        //     Console.WriteLine($"Id: {t.Id} - Name: {t.Name}");
        // }

        // Stored procedure - Add a new category
        // MySqlParameter takes 
        // 1. The name of the parameter on the stored procedure
        // 2. The value to use as argument
        // We can then use parameter when callling the procedure.
        // var parameter = new MySqlParameter("@catName", "Clamps");
        // context.Database.ExecuteSql($"CALL sp_addCategory({parameter})");

        /* 
        UPDATE Categories
        SET Name = catName
        WHERE ID = catId;
        */
        // var catId = new MySqlParameter("@catId", "7");
        // var catName = new MySqlParameter("@catName", "Clamps");
        // context.Database.ExecuteSql($"CALL sp_UpdateCategoryById({catId}, {catName})");

        // Eager Loading
        // var toolsAndBrands = context.Tools.Include(t => t.Brand); // This is currently an implementation IQueryable<T>, and is not materialized yet

        // SELECT `t`.`ID`, `t`.`BrandID`, `t`.`CategoryID`, `t`.`Name`, `t`.`Price`, `b`.`ID`, `b`.`Name`
        // FROM `Tools` AS `t`
        // INNER JOIN `Brands` AS `b` ON `t`.`BrandID` = `b`.`ID`
        // Console.WriteLine(toolsAndBrands.ToQueryString()); // From the query string we can see that this is just a blueprint, no execution yet.

        // toolsAndBrands now becomes an IEnumerable<T> since we are enumerating it with foreach, meaning the querystring above is executed
        // foreach (var t in toolsAndBrands)
        // {
        //     Console.WriteLine($"Id: {t.Id} - Name: {t.Name} - Brand: {t.Brand?.Name}");
        // }

        // Say Brand had a foreign key with a model Supplier, in order to reach than, we would have to use
        // ThenInclude(t => t.Supplier) to bring that in

        // Lazy Loading
        // var onlyTools = context.Tools.ToList();

        // foreach (var t in onlyTools)
        // {
        //     Console.WriteLine($"Name: {t.Name} - Category: {t.Category?.Name}"); // Lazy loading through navigation property
        // }

        // Explicit Loading
        // var tool = context.Tools.FirstOrDefault();
        // if (tool != null)
        // {
        //     context.Entry(tool).Reference(t => t.Brand).Load();
        //     Console.WriteLine($"Name: {tool.Name} - Brand: {tool.Brand?.Name}"); // Loads the name of the brand for the first tool retrieved (Explicit loading)
        // }

        // Similarly for a list of tools
        // The relationship model here is One-To-One, a tool belong to one Brand
        // var tools = context.Tools.ToList();
        // foreach (var t in tools)
        // {
        //     context.Entry(t).Reference(t => t.Brand).Load();
        //     Console.WriteLine($"Name: {t.Name} - Brand: {t.Brand?.Name}"); // Loads the name of the brand for the first tool retrieved (Explicit loading)
        // }

        // Here we are using Reference because our navigation property is a single entity, if it were a collection we would use
        // .Collection instead:
        // The relationship model here is One-To-Many, a Brand has many Tools
        var brand = context.Brands.FirstOrDefault();
        if (brand != null)
        {
            context.Entry(brand).Collection(t => t.Tools).Load(); // Accessing tools related to this brand through Explicit Loading

            Console.WriteLine($"Name: {brand.Name}"); // Log the brand name

            foreach (var t in brand.Tools) // Log all the tools loaded through Explicit Loading
            {
                Console.WriteLine($"Tools name: {t.Name}");
            }
        }
    }
}