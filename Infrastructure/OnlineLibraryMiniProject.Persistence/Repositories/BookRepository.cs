using OnlineLibraryMiniProject.Domain.Entities;
using OnlineLibraryMiniProject.Domain.Entities.Enums;
using OnlineLibraryMiniProject.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using OnlineLibraryMiniProject.Application.Interfaces.Repositories;

namespace OnlineLibraryMiniProject.Persistence.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;

        public BookRepository(AppDbContext context)
        {
            _context = context;
        }

        // 1. Create Book
        public void Create(Book book)
        {
            _context.Books.Add(book);
            _context.SaveChanges();
        }

        // 2. Delete Book
        public void Delete(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null) return;

            _context.Books.Remove(book);
            _context.SaveChanges();
        }

        // 3. Get Book by Id (Rezervasiya tarixçəsi ilə birgə)
        public Book? GetByIdWithHistory(int id)
        {
            return _context.Books
                           .Include(b => b.ReservedItems)
                           .FirstOrDefault(b => b.Id == id);
        }

        // 4. Show All Books (Müəllif adı ilə birgə)
        public List<Book> GetAll()
        {
            return _context.Books.Include(b => b.Author).ToList();
        }

        public bool Exists(int id)
        {
            return _context.Books.Any(b => b.Id == id);
        }
        public Book? GetById(int id)
        {
            return _context.Books
                           .Include(b => b.Author)
                           .Include(b => b.ReservedItems)
                           .FirstOrDefault(b => b.Id == id);
        }
    }
}
