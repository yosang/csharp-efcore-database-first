using Microsoft.EntityFrameworkCore;
using test.Models;

public class Program
{
    public static void Main()
    {
        using var context = new TestdbContext();

        var tools = context.Tools
                            .Include(t => t.Brand)
                            .Include(t => t.Category)
                            .ToList();

        foreach (var t in tools)
        {
            Console.WriteLine($"Id: {t.Id} - Name: {t.Name} - Brand: {t.Brand?.Name} - Category: {t.Category?.Name}");
        }
    }
}