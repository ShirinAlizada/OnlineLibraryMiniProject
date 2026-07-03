using Microsoft.EntityFrameworkCore;
using OnlineLibraryMiniProject.Application.Interfaces.Repositories;
using OnlineLibraryMiniProject.Domain.Entities;
using OnlineLibraryMiniProject.Persistence.Contexts;

using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibraryMiniProject.Persistence.Repositories
{
    public class AuthorRepository
    {
        private readonly AppDbContext _context;

        public AuthorRepository(AppDbContext context)
        {
            _context = context;
        }

        // 5. Create Author
        public void Add(Author author)
        {
            _context.Authors.Add(author);
            _context.SaveChanges();
        }

        // 6. Show All Authors (Kitab sayı ilə birlikdə gətiririk)
        public List<Author> GetAllWithBooks()
        {
            return _context.Authors.Include(a => a.Books).ToList();
        }

        // 7. Show Author's Books
        public Author GetByIdWithBooks(int authorId)
        {
            return _context.Authors
                           .Include(a => a.Books)
                           .FirstOrDefault(a => a.Id == authorId);
        }

        // Author mövcuddurmu yoxlanışı (Kitab yaradanda lazım olacaq)
        public bool Exists(int id)
        {
            return _context.Authors.Any(a => a.Id == id);
        }
    }
}
