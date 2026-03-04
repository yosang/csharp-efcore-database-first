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
        var toolsThatEndWithR = context.Tools
            .FromSql($"CALL sp_getRTools()")
            .ToList();

        foreach (var t in toolsThatEndWithR)
        {
            Console.WriteLine($"Id: {t.Id} - Name: {t.Name}");
        }

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
        var catId = new MySqlParameter("catId", "7");
        var catName = new MySqlParameter("@catName", "Clamp Equipment");
        context.Database.ExecuteSql($"CALL sp_UpdateCategoryById({catId}, {catName})");

    }
}