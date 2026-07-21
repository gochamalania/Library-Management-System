using Application.Interfaces;
using Application.Services;
using Repository.Implementations;
using Repository.Interfaces;
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
        
        // 2. Repositories (memory/file layer)
        IUserRepository userRepository = new TxtUserRepository(usersPath);
        IBookRepository bookRepository = new TxtBookRepository(booksPath);
        IBorrowRepository borrowRepository = new TxtBorrowRepository(borrowsPath);
        
        // 3. Services (business logic layer)
        IAuthService authService = new AuthService(userRepository);
        ILibraryService libraryService = new LibraryService(bookRepository, borrowRepository, userRepository);
        
        // 4. UI (launching the main menu)
        LoginMenu loginMenu = new LoginMenu(authService, libraryService);
        loginMenu.Show();
    }
}