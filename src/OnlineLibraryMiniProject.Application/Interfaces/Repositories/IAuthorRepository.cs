using OnlineLibraryMiniProject.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibraryMiniProject.Application.Interfaces.Repositories
{
    public interface IAuthorRepository
    {
        public void Create(Author author);
        public List<Author> GetAll();
        public void Delete(int id);
        public Author? GetById(int id);
    }
}
