using OnlineLibraryMiniProject.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibraryMiniProject.Domain.Entities
{
    public class ReservedItem : BaseEntity
    {
        public string FinCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public Status Status { get; set; }
    }
}
