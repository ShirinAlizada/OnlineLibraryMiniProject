using OnlineLibraryMiniProject.Application.Interfaces.Repositories;
using OnlineLibraryMiniProject.Application.Interfaces.Services;
using OnlineLibraryMiniProject.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibraryMiniProject.Application.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authors;

        public AuthorService(IAuthorRepository authors)
        {
            _authors = authors;
        }

        // 1. Create - Yeni Müəllif Əlavə Etmək
        public void Create(Author author)
        {
            if (author is null)
                throw new ArgumentNullException(nameof(author), "Müellif melumatları boş ola bilmez.");

            if (string.IsNullOrWhiteSpace(author.Name))
                throw new ArgumentException("Müellifin adı boş ola bilmez!", nameof(author.Name));

            // Adı kənardakı boşluqlardan təmizləyirik
            author.Name = author.Name.Trim();

            // Soyad daxil edilibsə onu da təmizləyirik
            if (!string.IsNullOrWhiteSpace(author.Surname))
            {
                author.Surname = author.Surname.Trim();
            }

            _authors.Create(author);
        }

        // 2. Delete - Müəllifi Silmək
        public void Delete(int id)
        {
            var author = _authors.GetById(id);
            if (author is null)
                throw new InvalidOperationException("Müellif tapılmadı.");

            // Biznes Qaydası: Müəllifin bazada kitabları varsa, o silinə bilməz
            if (author.Books != null && author.Books.Any())
                throw new InvalidOperationException("Bu müellifin bazada kitabları mövcuddur. Müellifi silmek üçün evvelce onun kitablarını silmelisiniz!");

            _authors.Delete(id);
        }

        // 3. GetAll - Bütün Müəllifləri Siyahılamaq
        public List<Author> GetAll()
        {
            return _authors.GetAll();
        }

        // 4. GetBooksByAuthor - Müəllifin Kitablarını Siyahılamaq
        public List<Book> GetBooksByAuthor(int authorId)
        {
            var author = _authors.GetById(authorId);

            // Əgər müəllif tapılarsa, kitablarını adına görə sırayla gətir, tapılmazsa (və ya kitabı yoxdursa) boş list qaytar
            return author?.Books?.OrderBy(b => b.Name).ToList() ?? new List<Book>();
        }

        // 5. GetByID - ID-yə görə Müəllifi Tapmaq (İnterfeysə uyğun olaraq ID böyük hərflərlədir)
        public Author? GetByID(int id)
        {
            return _authors.GetById(id);
        }
    }
}
