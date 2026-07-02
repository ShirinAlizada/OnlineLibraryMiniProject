using OnlineLibraryMiniProject.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibraryMiniProject.Application.Interfaces.Repositories
{
    public interface IBookRepository
    {
        public void Create(Book book);
        public List<Book> GetAll();
        public void Delete(int id);
        public Book? GetById(int id);
    }
}
