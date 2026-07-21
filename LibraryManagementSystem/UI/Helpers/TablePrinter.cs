namespace UI.Helpers;
using Core.Models;
using System;
using System.Collections.Generic;

public class TablePrinter
{
    // Print books as a table
    public static void PrintBooks(List<Book> books)
    {
        if (books == null || books.Count == 0)
        {
            Console.WriteLine("No books found.");
            return;
        }

        string line = new string('-', 82);
        Console.WriteLine(line);
        Console.WriteLine($"| {"ID", -4} | {"Title", -25} | {"Author", -18} | {"ISBN", -15} | {"Stock", -6} |");
        Console.WriteLine(line);

        foreach (var b in books)
        {
            string title = b.Title.Length > 23 ? b.Title.Substring(0, 20) + "..." : b.Title;
            string author = b.Author.Length > 16 ? b.Author.Substring(0, 13) + "..." : b.Author;
            string copies = $"{b.AvailableCopies}/{b.TotalCopies}";

            Console.WriteLine($"| {b.Id, -4} | {title, -25}, | {author, -18} | {b.ISBN,-15} | {copies,-6} |");
        }

        Console.WriteLine(line);
    }
}