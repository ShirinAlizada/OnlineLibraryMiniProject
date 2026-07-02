using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibraryMiniProject.Domain.Entities
{
    public class Book : BaseEntity
    {
        public string Name { get; set; }
        public int PageCount { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public List<ReservedItem> ReservedItems { get; set; } = new List<ReservedItem>();

    }
}
