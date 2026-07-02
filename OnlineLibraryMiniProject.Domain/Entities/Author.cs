using OnlineLibraryMiniProject.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibraryMiniProject.Domain.Entities
{
    public class Author : BaseEntity
    {
        public string Name { get; set; }
        public string ?Surname { get; set; }
        public Gender Gender { get; set; }
        public List<Book> Books { get; set; } = new List<Book>();
        
    }
}
