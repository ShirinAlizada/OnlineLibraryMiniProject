using OnlineLibraryMiniProject.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibraryMiniProject.Application.Interfaces.Services
{
    public interface IBookService
    {
        void Create(Book book);
        void Delete(int id);
        Book? GetById(int id, bool withDate = false);
        List<Book> GetAll();
        List<ReservedItem> GetBookReservInfo(int bookId);
    }
}
