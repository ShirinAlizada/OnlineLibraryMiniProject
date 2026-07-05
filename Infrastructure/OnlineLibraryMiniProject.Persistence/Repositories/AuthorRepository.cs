using Microsoft.EntityFrameworkCore;
using OnlineLibraryMiniProject.Application.Interfaces.Repositories;
using OnlineLibraryMiniProject.Domain.Entities;
using OnlineLibraryMiniProject.Persistence.Contexts;

using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibraryMiniProject.Persistence.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly AppDbContext _context;

        public AuthorRepository(AppDbContext context)
        {
            _context = context;
        }

        // 5. Create Author
        public void Create(Author author)
        {
            _context.Authors.Add(author);
            _context.SaveChanges();
        }

        // 6. Show All Authors (Kitab sayı ilə birlikdə gətiririk)
        public List<Author> GetAll()
        {
            return _context.Authors.Include(a => a.Books).ToList();
        }

        // 7. Show Author's Books
        public Author? GetById(int authorId)
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
        public void Delete(int id)
        {
            var author = _context.Authors.FirstOrDefault(a => a.Id == id);
            if (author == null) return;

            _context.Authors.Remove(author);
            _context.SaveChanges();
        }
    }
}
