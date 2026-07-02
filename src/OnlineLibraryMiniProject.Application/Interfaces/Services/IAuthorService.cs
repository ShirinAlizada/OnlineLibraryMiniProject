using OnlineLibraryMiniProject.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibraryMiniProject.Application.Interfaces.Services
{
    public interface IAuthorService
    {
        void Create(Author author);
        void Delete(int id);
        List<Author> GetAll();
        List<Book> GetBooksByAuthor(int authorId);
        Author? GetByID(int id);
    }
}
