using OnlineLibraryMiniProject.Application.Interfaces.Services;
using OnlineLibraryMiniProject.Application.Services;
using OnlineLibraryMiniProject.ConsoleApp.Helpers;
using OnlineLibraryMiniProject.Persistence.Contexts;
using OnlineLibraryMiniProject.Persistence.Repositories;

namespace OnlineLibraryMiniProject.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var db = new AppDbContext();

            var authorRepo = new AuthorRepository(db);
            var bookRepo = new BookRepository(db);
            var reservedRepo = new ReservedItemRepository(db);

            IAuthorService authorService = new AuthorService(authorRepo);
            IBookService bookService = new BookService(bookRepo, authorRepo);
            IReservedItemService reservedItemService = new ReservedItemService(reservedRepo, bookRepo);

            var manage = new ManageMetods(authorService, bookService, reservedItemService);

            MenuManagement.Run(manage);
        }
    }
}
