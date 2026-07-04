using OnlineLibraryMiniProject.Application.Interfaces.Repositories;
using OnlineLibraryMiniProject.Application.Interfaces.Services;
using OnlineLibraryMiniProject.Domain.Entities;
using OnlineLibraryMiniProject.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibraryMiniProject.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _books;
        private readonly IAuthorRepository _authors;

        public BookService(IBookRepository books, IAuthorRepository authors)
        {
            _books = books;
            _authors = authors;
        }

        // 1. Create Method
        public void Create(Book book)
        {
            if (book is null)
                throw new ArgumentNullException(nameof(book), "Kitab melumatı boş ola bilmez.");

            if (string.IsNullOrWhiteSpace(book.Name))
                throw new ArgumentException("Kitabın adı boş ola bilmez!", nameof(book.Name));

            if (book.PageCount <= 0)
                throw new ArgumentException("Kitabın sehifə sayı 0 ve ya menfi ola bilmez!", nameof(book.PageCount));

            var author = _authors.GetById(book.AuthorId);
            if (author is null)
                throw new InvalidOperationException("Daxil etdiyiniz ID-ye sahib müellif tapılmadı!");

            book.Name = book.Name.Trim();
            book.Author = author;

            _books.Create(book);
        }

        // 2. Delete Method
        public void Delete(int id)
        {
            var book = _books.GetById(id);
            if (book is null)
                throw new InvalidOperationException("Kitab tapılmadı.");

            // Əgər kitab hazırda aktiv rezervasiyadadırsa (Confirmed və ya Started) silinə bilməz
            var hasActive = book.ReservedItems.Any(r => r.Status == Status.Confirmed || r.Status == Status.Started);
            if (hasActive)
                throw new InvalidOperationException("Kitab hazırda aktiv rezervasiyada (istifadede) olduğu üçün siline bilmez!");

            _books.Delete(id);
        }

        // 3. GetAll Method
        public List<Book> GetAll()
        {
            return _books.GetAll();
        }

        // 4. GetById Method (İnterfeysə tam uyğun olaraq 2 parametrli)
        public Book? GetById(int id, bool withDate = false)
        {
            var book = _books.GetById(id);
            if (book is null) return null;

            if (!withDate)
            {
                book.ReservedItems = new List<ReservedItem>();
            }
            return book;
        }

        // 5. GetBookReservInfo Method (Sənin kodunda unudulan, interfeysin tələb etdiyi metod)
        public List<ReservedItem> GetBookReservInfo(int bookId)
        {
            var book = _books.GetById(bookId);
            return book?.ReservedItems.OrderByDescending(r => r.StartDate).ToList()
                   ?? new List<ReservedItem>();
        }
    }
}

