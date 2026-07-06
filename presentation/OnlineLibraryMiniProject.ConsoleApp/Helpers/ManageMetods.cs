using OnlineLibraryMiniProject.Application.Interfaces.Services;
using OnlineLibraryMiniProject.Domain.Entities;
using OnlineLibraryMiniProject.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace OnlineLibraryMiniProject.ConsoleApp.Helpers
{
    public class ManageMetods
    {
        private readonly IAuthorService _authors;
        private readonly IBookService _books;
        private readonly IReservedItemService _reservations;

        public ManageMetods(IAuthorService authors, IBookService books, IReservedItemService reservations)
        {
            _authors = authors;
            _books = books;
            _reservations = reservations;
        }


        //LINQ-ın .All() metodundan istifadə edir. Mətndəki bütün simvollar (c) tək-tək yoxlanılır: əgər simvol hərfdirsə (char.IsLetter),
        //boşluqdursa (' ') və ya defisdirsə ('-'), metod true qaytarır. Əgər bircə dənə də olsun rəqəm və ya fərqli simvol varsa, false qaytarır.
        private static bool IsLettersOnly(string s) => s.All(c => char.IsLetter(c) || c == ' ' || c == '-');



        // Enum-u ya rəqəmlə, ya da adı ilə oxumaq üçün ümumi metod
        //Generic (<TEnum>) strukturundadır, yəni istənilən Enum tipi üçün işləyə bilər.
        //İlk olaraq int.TryParse ilə yoxlayır ki, istifadəçi rəqəm yazıb ya yox.Əgər rəqəmdirsə və bu rəqəm 
        //Enum daxilində təyin olunubsa(Enum.IsDefined), həmin rəqəmi Enum dəyərinə çevirir(result = (TEnum)...) və true qaytarır.
        //Əgər rəqəm deyilsə, Enum.TryParse ilə mətni birbaşa Enum adlarına görə axtarır
        //(məsələn: "Active", "Pending"). true arqumenti böyük/kiçik hərf fərqini qorunmamasını təmin edir.
        
        private static bool TryReadEnum<TEnum>(string input, out TEnum result) where TEnum : struct, Enum
        {
            if (int.TryParse(input, out var n) && Enum.IsDefined(typeof(TEnum), n))
            {
                result = (TEnum)(object)n;
                return true;
            }
            return Enum.TryParse(input, true, out result);
        }



        // Enum seçimlərini ekrana real dəyərləri ilə çap edir
        private static void ShowEnumOptions<TEnum>() where TEnum : struct, Enum
        {
            foreach (TEnum v in Enum.GetValues(typeof(TEnum)))
                MenuManagement.RightWriteLine($"{Convert.ToInt32(v)} - {v}");
        }

        // 1. Create Book
        public void CreateBook()
        {
            int step = 0;
            string name = "";
            int page = 0;
            int authorId = 0;

            while (true)
            {
                MenuManagement.RightBegin("Create Book");

                //İstifadəçi hansı məlumatları artıq daxil edibsə, onlar hər dövrün başında
                //ekranda "Yadda saxlanılanlar" kimi vizual olaraq göstərilir.
                if (!string.IsNullOrWhiteSpace(name)) MenuManagement.RightWriteLine($"Name     : {name}");
                if (page > 0) MenuManagement.RightWriteLine($"Page     : {page}");
                if (authorId > 0) MenuManagement.RightWriteLine($"AuthorId : {authorId}");
                MenuManagement.RightWriteLine("");

                if (step == 0)
                {
                    //İstifadəçidən kitab adı soruşulur. "menu" yazsa, funksiyadan çıxır (return).
                    //Boş buraxsa, xəta verir və continue ilə dövrü yenidən başladır (addım dəyişmir).
                    //Düzgün yazsa, name dəyişəninə yazılır və step = 1 olur (növbəti addıma keçir).

                    var input = (MenuManagement.RightAsk("Book name (menu): ") ?? "").Trim();
                    if (input.Equals("menu", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); return; }
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        MenuManagement.RightError("Name must not be empty.");
                        MenuManagement.RightEnd(); continue;
                    }
                    name = input;
                    step = 1;
                }
                else if (step == 1)
                {
                   
                    //Istifadəçi "back" yazarsa, step = 0 edilir və continue vasitəsilə yenidən kitab adı soruşulan hissəyə qaytarılır.
                    var input = (MenuManagement.RightAsk("Page count (menu/back): ") ?? "").Trim();
                    if (input.Equals("menu", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); return; }
                    if (input.Equals("back", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); step = 0; continue; }

                    //Əgər istifadəçi səhv məlumat daxil edirsə (məsələn, mənfi rəqəm və ya hərf), xəta mesajı göstərilir və continue ilə dövr yenidən başlayır.
                    if (!int.TryParse(input, out page) || page <= 0)
                    {
                        MenuManagement.RightError("Input a positive number.");
                        MenuManagement.RightEnd(); continue;
                    }
                    step = 2;
                }
                else
                {
                    //İstifadəçi "back" yazarsa, step = 1 edilir və continue vasitəsilə yenidən səhifə sayı soruşulan hissəyə qaytarılır.
                    var authors = _authors.GetAll().OrderBy(a => a.Id).ToList();
                    if (authors.Count == 0)
                    {
                        //Əgər müəlliflər siyahısı boşdursa, istifadəçiyə xəbərdarlıq mesajı göstərilir və funksiyadan çıxılır.
                        MenuManagement.RightWarn("No authors. Create an author first.");
                        MenuManagement.RightEnd(); return;
                    }
                    MenuManagement.RightWriteLine("-- Authors --");
                    foreach (var a in authors)
                    {
                        //Müəllifin tam adı (ad + soyad) yaradılır. Əgər soyad boşdursa, yalnız ad göstərilir.
                        var full = string.IsNullOrWhiteSpace(a.Surname) ? a.Name : $"{a.Name} {a.Surname}";
                        MenuManagement.RightWriteLine($"ID:[{a.Id}] {full} | Books: {a.Books?.Count ?? 0}");
                    }
                    MenuManagement.RightWriteLine("");
                    //İstifadəçidən müəllif ID soruşulur. "menu" yazsa, funksiyadan çıxır (return).
                    //"back" yazsa, step = 1 olur və səhifə sayı soruşulan hissəyə qaytarılır.
                    var input = (MenuManagement.RightAsk("Author Id (menu/back): ") ?? "").Trim();
                    if (input.Equals("menu", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); return; }
                    if (input.Equals("back", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); step = 1; continue; }

                    //Əgər istifadəçi düzgün müəllif ID daxil edirsə və bu ID mövcud müəlliflər siyahısında varsa, kitab yaradılır.
                    if (int.TryParse(input, out authorId) && _authors.GetByID(authorId) != null)
                    {

                        //Kitab yaradılarkən try-catch bloku istifadə olunur ki, hər hansı bir xəta baş verərsə, istifadəçiyə xəta mesajı göstərilsin.
                        try
                        {
                            //Kitab yaradılır və məlumat bazasına əlavə olunur.
                            _books.Create(new Book { Name = name.Trim(), PageCount = page, AuthorId = authorId });
                            MenuManagement.RightSuccess("Book created.");
                        }
                        catch (Exception ex)
                        {
                            //Əgər kitab yaradılarkən hər hansı bir xəta baş verərsə, istifadəçiyə xəta mesajı göstərilir.
                            MenuManagement.RightError($"Error: {ex.Message}");
                        }
                        MenuManagement.RightEnd(); return;
                    }
                    else
                    {
                        MenuManagement.RightError("Author not found.");
                        MenuManagement.RightEnd(); continue;
                    }
                }
                MenuManagement.RightEnd();
            }
        }

        // 2. Delete Book
        public void DeleteBook()
        {
            while (true)
            {
                MenuManagement.RightBegin("Delete Book");

                //Kitablar siyahısı əldə edilir və adlarına görə sıralanır. Əgər siyahı boşdursa,
                //istifadəçiyə xəbərdarlıq mesajı göstərilir və funksiyadan çıxılır.
                var books = _books.GetAll().OrderBy(b => b.Id).ToList();
                if (books.Count == 0)
                {
                    MenuManagement.RightWarn("No books.");
                    MenuManagement.RightEnd(); return;
                }

                MenuManagement.RightWriteLine("-- Books --");
                foreach (var b in books)
                {
                    //Mövcud kitabları ekrana siyahı şəklində çıxarır. Burada incə bir məqam var: b.Author is null yoxlaması
                    //(NullReferenceException xətasının qarşısını almaq üçün).
                    //Əgər kitaba bağlı müəllif obyekti yüklənməyibsə, sadəcə AuthorId yazdırır.
                    //Yüklənibsə, müəllifin soyadı olub-olmamasını yoxlayıb tam adını formalaşdırır.
                    var author = b.Author is null
                        ? $"AuthorId={b.AuthorId}"
                        : (string.IsNullOrWhiteSpace(b.Author.Surname) ? b.Author.Name : $"{b.Author.Name} {b.Author.Surname}");
                    MenuManagement.RightWriteLine($"ID[{b.Id}] Name:{b.Name} | Pages:{b.PageCount} | Author:{author}");
                }
                MenuManagement.RightWriteLine("");

                //İstifadəçidən silmək istədiyi kitabın ID-si soruşulur. "menu" yazsa, funksiyadan çıxılır.
                var input = (MenuManagement.RightAsk("Book Id (menu): ") ?? "").Trim();
                if (input.Equals("menu", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); return; }

                //Əgər istifadəçi düzgün rəqəm daxil etməyibsə, xəta mesajı göstərilir və dövr yenidən başlayır.
                if (!int.TryParse(input, out var id))
                {
                    MenuManagement.RightError("Input a number.");
                    MenuManagement.RightEnd(); continue;
                }

                var book = _books.GetById(id);
                if (book is null)
                {
                    MenuManagement.RightError("Book not found.");
                    MenuManagement.RightEnd(); continue;
                }

                //Kitab silinərkən try-catch bloku istifadə olunur ki, hər hansı bir xəta baş verərsə, istifadəçiyə xəta mesajı göstərilsin.
                try
                {
                    _books.Delete(id);
                    MenuManagement.RightSuccess("Book deleted.");
                }
                catch (Exception ex)
                {
                    MenuManagement.RightError($"Error: {ex.Message}");
                }

                MenuManagement.RightEnd(); return;
            }
        }

        // 3. Get Book By Id
        public void GetBookById()
        {
            //İstifadəçi kitab ID-ni daxil edəcək və istəyə görə rezervasiya tarixçəsini də görmək imkanı olacaq.
            int step = 0;
            int id = 0;
            bool withHistory = false;
            Book? selected = null;

            while (true)
            {
                MenuManagement.RightBegin("Get Book By Id");

                var books = _books.GetAll().OrderBy(b => b.Id).ToList();
                if (books.Count == 0)
                {
                    MenuManagement.RightWarn("No books.");
                    MenuManagement.RightEnd(); return;
                }

                MenuManagement.RightWriteLine("-- Books --");
                //Mövcud kitabları ekrana siyahı şəklində çıxarır. Burada incə bir məqam var: b.Author is null yoxlaması
                foreach (var b in books)
                {

                    //Əgər kitaba bağlı müəllif obyekti yüklənməyibsə, sadəcə AuthorId yazdırır.
                    //Yüklənibsə, müəllifin soyadı olub-olmamasını yoxlayıb tam adını formalaşdırır.
                    var authorName = b.Author is null
                        ? $"AuthorId={b.AuthorId}"
                        : (string.IsNullOrWhiteSpace(b.Author.Surname) ? b.Author.Name : $"{b.Author.Name} {b.Author.Surname}");
                    MenuManagement.RightWriteLine($"ID[{b.Id}] Name:{b.Name} | Pages:{b.PageCount} | Author:{authorName}");
                }
                MenuManagement.RightWriteLine("");

                if (step == 0)
                {

                    //İstifadəçidən kitab ID soruşulur. "menu" yazsa, funksiyadan çıxılır.
                    var s = (MenuManagement.RightAsk("Book Id (menu): ") ?? "").Trim();
                    if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); return; }

                    if (!int.TryParse(s, out id))
                    {
                        MenuManagement.RightError("Input a number.");
                        MenuManagement.RightEnd(); continue;
                    }

                    selected = _books.GetById(id, withDate: true);
                    if (selected is null)
                    {
                        MenuManagement.RightError("Book not found.");
                        MenuManagement.RightEnd(); continue;
                    }

                    step = 1;
                    MenuManagement.RightEnd();
                    continue;
                }
                else
                {
                    //İstifadəçidən rezervasiya tarixçəsini görmək istəyib-istəmədiyi soruşulur. "menu" yazsa, funksiyadan çıxılır.
                    var s = (MenuManagement.RightAsk("Show reservation history? (y/n, menu/back): ") ?? "")
                        .Trim().ToLowerInvariant();
                    if (s == "menu") { MenuManagement.RightEnd(); return; }
                    if (s == "back") { MenuManagement.RightEnd(); step = 0; continue; }

                    if (s is "y" or "yes") withHistory = true;
                    else if (s is "n" or "no") withHistory = false;
                    else
                    {
                        MenuManagement.RightError("Invalid choice. Use y/n.");
                        MenuManagement.RightEnd(); continue;
                    }

                    //Seçilmiş kitabın məlumatları ekrana çıxarılır. Burada da müəllifin
                    //tam adı yaradılır və əgər müəllif obyekti null-dursa, sadəcə AuthorId göstərilir.
                    var author = selected!.Author is null
                        ? $"AuthorId={selected.AuthorId}"
                        : (string.IsNullOrWhiteSpace(selected.Author.Surname) ? selected.Author.Name : $"{selected.Author.Name} {selected.Author.Surname}");

                    MenuManagement.RightWriteLine($"[Book:{selected.Id}] Name:{selected.Name} | Pages:{selected.PageCount} | Author:{author}");

                    if (withHistory && selected.ReservedItems?.Any() == true)
                    {
                        //Rezervasiya tarixçəsi varsa, hər bir rezervasiya üçün məlumatlar ekrana çıxarılır. Burada da kitabın adı yoxlanılır:
                        MenuManagement.RightWriteLine("-- History --");
                        foreach (var r in selected.ReservedItems.OrderByDescending(x => x.StartDate))
                        {
                            var bname = r.Book is null ? $"BookId={r.BookId}" : r.Book.Name;
                            MenuManagement.RightWriteLine($"[ResID:{r.Id}] FIN:{r.FinCode} | {r.StartDate:yyyy-MM-dd} - {r.EndDate:yyyy-MM-dd} | {bname} | {r.Status}");
                        }
                    }

                    MenuManagement.RightEnd(); return;
                }
            }
        }

        // 4. Show All Books
        public void ShowAllBooks()
        {
            MenuManagement.RightBegin("All Books");
            var list = _books.GetAll();
            if (list.Count == 0)
            {
                MenuManagement.RightWarn("No books.");
                MenuManagement.RightEnd(); return;
            }
            foreach (var b in list.OrderBy(x => x.Id))
            {
                //Müəllifin tam adı yaradılır və əgər müəllif obyekti null-dursa, sadəcə AuthorId göstərilir.
                var author = b.Author is null
                    ? $"AuthorId={b.AuthorId}"
                    : (string.IsNullOrWhiteSpace(b.Author.Surname) ? b.Author.Name : $"{b.Author.Name} {b.Author.Surname}");
                MenuManagement.RightWriteLine($"[BookID:{b.Id}] Name:{b.Name} | Pages:{b.PageCount} | Author:{author}");
            }
            MenuManagement.RightEnd();
        }

        // 5. Create Author
        public void CreateAuthor()
        {
            int step = 0;
            string name = "";
            string? surname = null;
            Gender gender = default;

            while (true)
            {
                MenuManagement.RightBegin("Create Author");
                if (!string.IsNullOrWhiteSpace(name)) MenuManagement.RightWriteLine($"Name   : {name}");
                if (surname != null) MenuManagement.RightWriteLine($"Surname: {surname}");
                MenuManagement.RightWriteLine("");

                if (step == 0)
                {
                    var input = (MenuManagement.RightAsk("Name (menu): ") ?? "").Trim();
                    if (input.Equals("menu", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); return; }

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        MenuManagement.RightError("Name must not be empty.");
                        MenuManagement.RightEnd(); continue;
                    }
                    if (!IsLettersOnly(input))
                    {
                        //Əgər istifadəçi adın daxilində hərf olmayan simvol daxil edirsə, xəta mesajı göstərilir və dövr yenidən başlayır.
                        MenuManagement.RightError("Name must contain only letters.");
                        MenuManagement.RightEnd(); continue;
                    }
                    name = input;
                    step = 1;
                }
                else if (step == 1)
                {
                    var input = (MenuManagement.RightAsk("Surname (optional, menu/back): ") ?? "").Trim();
                    if (input.Equals("menu", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); return; }
                    if (input.Equals("back", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); step = 0; continue; }

                    //Əgər istifadəçi soyadını boş buraxırsa, surname null olaraq qalır. Əgər daxil edirsə, yalnız hərflərdən ibarət olub-olmadığı yoxlanılır.
                    if (string.IsNullOrWhiteSpace(input))
                        surname = null;
                    else
                    {
                        if (!IsLettersOnly(input))
                        {
                            MenuManagement.RightError("Surname must contain only letters.");
                            MenuManagement.RightEnd(); continue;
                        }
                        surname = input;
                    }
                    step = 2;
                }
                else
                {
                    MenuManagement.RightWriteLine("-- Gender options --");
                    ShowEnumOptions<Gender>();
                    MenuManagement.RightWriteLine("");

                    var s = (MenuManagement.RightAsk("Gender (number or name) (menu/back): ") ?? "").Trim();
                    if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); return; }
                    if (s.Equals("back", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); step = 1; continue; }

                    //Əgər istifadəçi düzgün Gender dəyəri daxil edərsə, onun dəyəri gender dəyişənə təyin olunur.
                    if (!TryReadEnum<Gender>(s, out gender))
                    {
                        MenuManagement.RightError("Wrong value.");
                        MenuManagement.RightEnd(); continue;
                    }

                    var author = new Author { Name = name.Trim(), Surname = surname?.Trim(), Gender = gender };
                    try
                    {
                        _authors.Create(author);
                        MenuManagement.RightSuccess("Author created.");
                    }
                    catch (Exception ex)
                    {
                        MenuManagement.RightError($"Error: {ex.Message}");
                    }
                    MenuManagement.RightEnd(); return;
                }

                MenuManagement.RightEnd();
            }
        }

        // 5.1 Delete Author
        public void DeleteAuthor()
        {
            while (true)
            {
                MenuManagement.RightBegin("Delete Author");

                var authors = _authors.GetAll().OrderBy(a => a.Id).ToList();
                if (authors.Count == 0)
                {
                    MenuManagement.RightWarn("No authors.");
                    MenuManagement.RightEnd(); return;
                }

                MenuManagement.RightWriteLine("-- Authors --");
                foreach (var a in authors)
                {
                    var full = string.IsNullOrWhiteSpace(a.Surname) ? a.Name : $"{a.Name} {a.Surname}";
                    MenuManagement.RightWriteLine($"ID:[{a.Id}] {full} | Books: {a.Books?.Count ?? 0}");
                }
                MenuManagement.RightWriteLine("");

                var input = (MenuManagement.RightAsk("Author Id (menu): ") ?? "").Trim();
                if (input.Equals("menu", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); return; }

                if (!int.TryParse(input, out var id))
                {
                    MenuManagement.RightError("Input a number.");
                    MenuManagement.RightEnd(); continue;
                }

                var author = _authors.GetByID(id);
                if (author is null)
                {
                    MenuManagement.RightError("Author not found.");
                    MenuManagement.RightEnd(); continue;
                }

                try
                {
                    _authors.Delete(id);
                    MenuManagement.RightSuccess("Author deleted.");
                }
                catch (Exception ex)
                {
                    MenuManagement.RightError($"Error: {ex.Message}");
                }

                MenuManagement.RightEnd(); return;
            }
        }

        // 6. Show All Authors
        public void ShowAllAuthors()
        {
            //Mövcud müəllifləri ekrana siyahı şəklində çıxarır. Əgər müəlliflər siyahısı boşdursa, istifadəçiyə xəbərdarlıq mesajı göstərilir.
            MenuManagement.RightBegin("All Authors");
            var list = _authors.GetAll();
            if (list.Count == 0)
            {
                MenuManagement.RightWarn("No authors.");
                MenuManagement.RightEnd(); return;
            }
            foreach (var a in list.OrderBy(x => x.Id))
            {
                var full = string.IsNullOrWhiteSpace(a.Surname) ? a.Name : $"{a.Name} {a.Surname}";
                MenuManagement.RightWriteLine($"ID:[{a.Id}] Name:{full} | Gender:{a.Gender} | Books: {a.Books?.Count ?? 0}");
            }
            MenuManagement.RightEnd();
        }

        // 7. Show Author's Books
        public void ShowAuthorsBooks()
        {
            while (true)
            {
                MenuManagement.RightBegin("Author's Books");

                var authors = _authors.GetAll().OrderBy(a => a.Id).ToList();
                if (authors.Count == 0)
                {
                    MenuManagement.RightWarn("No authors.");
                    MenuManagement.RightEnd(); return;
                }

                MenuManagement.RightWriteLine("-- Authors --");
                foreach (var a in authors)
                {
                    var full = string.IsNullOrWhiteSpace(a.Surname) ? a.Name : $"{a.Name} {a.Surname}";
                    MenuManagement.RightWriteLine($"ID:[{a.Id}] {full} | Books: {a.Books?.Count ?? 0}");
                }
                MenuManagement.RightWriteLine("");

                var s = (MenuManagement.RightAsk("Author Id (menu): ") ?? "").Trim();
                if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); return; }

                //Əgər istifadəçi düzgün rəqəm daxil etməyibsə, xəta mesajı göstərilir və dövr yenidən başlayır.
                if (!int.TryParse(s, out var authorId))
                {
                    MenuManagement.RightError("Wrong choice. Input a number.");
                    MenuManagement.RightEnd(); continue;
                }

                var author = _authors.GetByID(authorId);
                if (author is null)
                {
                    MenuManagement.RightError("Author not found.");
                    MenuManagement.RightEnd(); continue;
                }

                var books = _authors.GetBooksByAuthor(authorId);
                if (books.Count == 0)
                {
                    MenuManagement.RightWarn("This author has no books.");
                    MenuManagement.RightEnd(); return;
                }

                MenuManagement.RightWriteLine($"-- Books of {author.Name}{(string.IsNullOrWhiteSpace(author.Surname) ? "" : " " + author.Surname)} --");
                foreach (var b in books)
                {
                    MenuManagement.RightWriteLine($"[BookID:{b.Id}] Name:{b.Name} | Pages:{b.PageCount}");
                }

                MenuManagement.RightEnd(); return;
            }
        }

        // 8. Reserve Book
        public void ReserveBook()
        {
            int step = 0;
            int bookId = 0;
            string fin = "";
            DateTime start = default, end = default;

            while (true)
            {
                MenuManagement.RightBegin("Reserve Book");

                if (step == 0)
                {
                    var books = _books.GetAll().OrderBy(b => b.Id).ToList();
                    if (books.Count == 0)
                    {
                        MenuManagement.RightWarn("No books.");
                        MenuManagement.RightEnd(); return;
                    }

                    MenuManagement.RightWriteLine("-- Books --");
                    foreach (var b in books)
                        MenuManagement.RightWriteLine($"ID:[{b.Id}] Name:{b.Name} | Pages:{b.PageCount}");
                    MenuManagement.RightWriteLine("");

                    var s = (MenuManagement.RightAsk("Book Id (menu): ") ?? "").Trim();
                    if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); return; }

                    if (!int.TryParse(s, out bookId))
                    {
                        MenuManagement.RightError("Wrong choice. Input a number.");
                        MenuManagement.RightEnd(); continue;
                    }
                    var book = _books.GetById(bookId);
                    if (book is null)
                    {
                        MenuManagement.RightError("Book not found.");
                        MenuManagement.RightEnd(); continue;
                    }

                    step = 1;
                    MenuManagement.RightEnd();
                    continue;
                }
                else if (step == 1)
                {
                    var s = (MenuManagement.RightAsk("FinCode (menu/back): ") ?? "").Trim();
                    if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); return; }
                    if (s.Equals("back", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); step = 0; continue; }

                    if (string.IsNullOrWhiteSpace(s))
                    {
                        MenuManagement.RightError("FinCode must not be empty.");
                        MenuManagement.RightEnd(); continue;
                    }

                    fin = s;
                    step = 2;
                    MenuManagement.RightEnd();
                    continue;
                }
                else if (step == 2)
                {
                    var s = (MenuManagement.RightAsk("Start (yyyy-MM-dd) (menu/back): ") ?? "").Trim();
                    if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); return; }
                    if (s.Equals("back", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); step = 1; continue; }

                    if (!DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out start))
                    {
                        MenuManagement.RightError("Use yyyy-MM-dd.");
                        MenuManagement.RightEnd(); continue;
                    }
                    step = 3;
                    MenuManagement.RightEnd();
                    continue;
                }
                else
                {
                    var s = (MenuManagement.RightAsk("End (yyyy-MM-dd) (menu/back): ") ?? "").Trim();
                    if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); return; }
                    if (s.Equals("back", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); step = 2; continue; }

                    if (!DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out end))
                    {
                        MenuManagement.RightError("Use yyyy-MM-dd.");
                        MenuManagement.RightEnd(); continue;
                    }

                    var item = new ReservedItem
                    {
                        BookId = bookId,
                        FinCode = fin.Trim(),
                        StartDate = start,
                        EndDate = end
                    };
                    try
                    {
                        _reservations.Create(item);
                        MenuManagement.RightSuccess("Reservation created (Confirmed).");
                    }
                    catch (Exception ex)
                    {
                        MenuManagement.RightError($"Error: {ex.Message}");
                    }
                    MenuManagement.RightEnd(); return;
                }
            }
        }

        // 9. Reservation List
        public void ReservationList()
        {
            MenuManagement.RightBegin("Reservation List");

            var qw = (MenuManagement.RightAsk("Filter by status? (y/n, menu): ") ?? "").Trim().ToLowerInvariant();
            if (qw == "menu") { MenuManagement.RightEnd(); return; }

            System.Collections.Generic.List<ReservedItem> list;

            if (qw is "y" or "yes")
            {
                MenuManagement.RightWriteLine("-- Status options --");
                ShowEnumOptions<Status>();
                MenuManagement.RightWriteLine("");

                var s = (MenuManagement.RightAsk("Status (number or name, menu): ") ?? "").Trim();
                if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); return; }

                if (!TryReadEnum<Status>(s, out var status))
                {
                    MenuManagement.RightError("Invalid value.");
                    MenuManagement.RightEnd(); return;
                }
                list = _reservations.GetAll(status);
            }
            else
            {
                list = _reservations.GetAll();
            }

            if (list.Count == 0)
            {
                MenuManagement.RightWarn("No reservations.");
                MenuManagement.RightEnd(); return;
            }

            MenuManagement.RightWriteLine("");
            foreach (var r in list)
            {
                var book = r.Book is null ? $"BookId={r.BookId}" : r.Book.Name;
                MenuManagement.RightWriteLine($"[ResID:{r.Id}] FIN:{r.FinCode} | {r.StartDate:yyyy-MM-dd} - {r.EndDate:yyyy-MM-dd} | {book} | {r.Status}");
            }

            MenuManagement.RightEnd();
        }

        // 10. Change Reservation Status
        public void ChangeReservationStatus()
        {
            int step = 0;
            int selectedId = 0;
            Status currentStatus = default;

            while (true)
            {
                if (step == 0)
                {
                    MenuManagement.RightBegin("Change Reservation Status");

                    var list = _reservations.GetAll();
                    if (list.Count == 0)
                    {
                        MenuManagement.RightWarn("No reservations.");
                        MenuManagement.RightEnd(); return;
                    }

                    MenuManagement.RightWriteLine("-- Reservations --");
                    foreach (var r in list)
                    {
                        var book = r.Book is null ? $"BookId={r.BookId}" : r.Book.Name;
                        MenuManagement.RightWriteLine($"ID:[{r.Id}] FIN:{r.FinCode} | {r.StartDate:yyyy-MM-dd} - {r.EndDate:yyyy-MM-dd} | {book} | {r.Status}");
                    }
                    MenuManagement.RightWriteLine("");

                    var s = (MenuManagement.RightAsk("Reservation Id (menu): ") ?? "").Trim();
                    if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); return; }

                    if (!int.TryParse(s, out selectedId) || selectedId <= 0)
                    {
                        MenuManagement.RightError("Invalid Id.");
                        MenuManagement.RightEnd(); continue;
                    }

                    var selected = list.FirstOrDefault(r => r.Id == selectedId);
                    if (selected is null)
                    {
                        MenuManagement.RightError("Reservation not found.");
                        MenuManagement.RightEnd(); continue;
                    }

                    currentStatus = selected.Status;
                    step = 1;
                    MenuManagement.RightEnd();
                    continue;
                }
                else
                {
                    MenuManagement.RightBegin("Change Reservation Status");
                    MenuManagement.RightWriteLine($"Selected Reservation Id: {selectedId}");
                    MenuManagement.RightWriteLine($"Current Status         : {currentStatus}");
                    MenuManagement.RightWriteLine("");
                    MenuManagement.RightWriteLine("-- Status options --");
                    ShowEnumOptions<Status>();
                    MenuManagement.RightWriteLine("");

                    var ns = (MenuManagement.RightAsk("New Status (number or name) (menu/back): ") ?? "").Trim();
                    if (ns.Equals("menu", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); return; }
                    if (ns.Equals("back", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); step = 0; continue; }

                    if (!TryReadEnum<Status>(ns, out var newStatus))
                    {
                        MenuManagement.RightError("Invalid value.");
                        MenuManagement.RightEnd(); continue;
                    }
                    if (newStatus == currentStatus)
                    {
                        MenuManagement.RightError("No change: reservation is already in the selected status.");
                        MenuManagement.RightEnd(); continue;
                    }

                    try
                    {
                        _reservations.Update(selectedId, newStatus);
                        MenuManagement.RightSuccess("Reservation status updated.");
                    }
                    catch (Exception ex)
                    {
                        MenuManagement.RightError($"Error: {ex.Message}");
                    }

                    MenuManagement.RightEnd(); return;
                }
            }
        }

        // 11. User's Reservations List
        public void UsersReservationsList()
        {
            int step = 0;
            string fin = "";
            System.Collections.Generic.List<ReservedItem> list = null!;

            while (true)
            {
                if (step == 0)
                {
                    MenuManagement.RightBegin("User's Reservations");
                    var input = (MenuManagement.RightAsk("FinCode (menu): ") ?? "").Trim();
                    if (input.Equals("menu", StringComparison.OrdinalIgnoreCase)) { MenuManagement.RightEnd(); return; }

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        MenuManagement.RightError("Wrong input. Try again.");
                        MenuManagement.RightEnd(); continue;
                    }

                    fin = input;
                    list = _reservations.GetByUser(fin);
                    if (list.Count == 0)
                    {
                        MenuManagement.RightWarn("No reservations for this user. Try another FIN.");
                        MenuManagement.RightEnd(); continue;
                    }

                    step = 1;
                    MenuManagement.RightEnd();
                    continue;
                }
                else
                {
                    MenuManagement.RightBegin($"Reservations of {fin}");
                    foreach (var r in list)
                    {
                        var book = r.Book is null ? $"BookId={r.BookId}" : r.Book.Name;
                        MenuManagement.RightWriteLine($"[ResID:{r.Id}] FIN:{r.FinCode} | {r.StartDate:yyyy-MM-dd} - {r.EndDate:yyyy-MM-dd} | {book} | {r.Status}");
                    }
                    MenuManagement.RightEnd(); return;
                }
            }
        }
    }
}
