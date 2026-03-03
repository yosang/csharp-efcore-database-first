using Microsoft.EntityFrameworkCore;
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

        // Stored Procedure
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

    }
}