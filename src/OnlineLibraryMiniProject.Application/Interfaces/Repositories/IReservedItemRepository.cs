using OnlineLibraryMiniProject.Domain.Entities;
using OnlineLibraryMiniProject.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibraryMiniProject.Application.Interfaces.Repositories
{
    public interface IReservedItemRepository
    {
        public void Create(ReservedItem item);
        public void Delete(int id);
        public List<ReservedItem> GetAll();
        public ReservedItem? GetById(int id);
        public void Update(ReservedItem item);
    }
}
