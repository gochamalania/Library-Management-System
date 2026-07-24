using Application.Interfaces;
using Application.Services;
using Repository.Implementations;
using Core.Interfaces;
using UI.Menus;
using System;
using System.IO;

namespace UI;

class Program
{
    static void Main(string[] args)
    {
        // 1. File addresses
        string dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
    
        string usersPath = Path.Combine(dataFolder, "users.txt");
        string booksPath = Path.Combine(dataFolder, "books.txt");
        string borrowsPath = Path.Combine(dataFolder, "borrows.txt");
        
        
        // 2 Repositories by json (memory/file layer)
        IUserRepository userRepository = new JsonUserRepository("users.json");
        IBookRepository bookRepository = new JsonBookRepository("books.json");
        IBorrowRepository borrowRepository = new JsonBorrowRepository("borrows.json");
        ILoggerService logger = new JsonLoggerService("logs.json");
        
        // 3. Services (business logic layer)
        ILoggerService userLogger = new JsonLoggerService("users.json");
        IAuthService authService = new AuthService(userRepository, logger);
        ILibraryService libraryService = new LibraryService(bookRepository, borrowRepository, userRepository);
        
        // 4. UI (launching the main menu)
        LoginMenu loginMenu = new LoginMenu(authService, libraryService);
        loginMenu.Show();
        
    }
}