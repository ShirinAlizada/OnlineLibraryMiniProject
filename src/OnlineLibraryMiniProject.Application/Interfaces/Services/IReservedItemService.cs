using OnlineLibraryMiniProject.Domain.Entities;
using OnlineLibraryMiniProject.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibraryMiniProject.Application.Interfaces.Services
{
    public interface IReservedItemService
    {
        void Create(ReservedItem item);
        void Delete(int id);
        List<ReservedItem> GetAll(Status? status = null);
        void Update(int reservationId, Status newStatus);
        List<ReservedItem> GetByUser(string finCode);
    }
}
