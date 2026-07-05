using Microsoft.EntityFrameworkCore;
using OnlineLibraryMiniProject.Application.Interfaces.Repositories;
using OnlineLibraryMiniProject.Domain.Entities;
using OnlineLibraryMiniProject.Domain.Entities.Enums;
using OnlineLibraryMiniProject.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibraryMiniProject.Persistence.Repositories
{
    public class ReservedItemRepository : IReservedItemRepository
    {
        private readonly AppDbContext _context;

        public ReservedItemRepository(AppDbContext context)
        {
            _context = context;
        }

        // 8. Reserve Book
        public void Create(ReservedItem item)
        {
            _context.ReservedItems.Add(item);
            _context.SaveChanges();
        }
        public void Delete(int id)
        {
            var item = _context.ReservedItems.FirstOrDefault(r => r.Id == id);
            if (item == null) return;

            _context.ReservedItems.Remove(item);
            _context.SaveChanges();
        }

        // Optional Şərt üçün: Bir FinCode eyni anda maksimum 3 aktiv (Confirmed və ya Started) kitab götürə bilsin
        public int GetActiveReservationCountByFin(string finCode)
        {
            return _context.ReservedItems
                           .Count(r => r.FinCode == finCode &&
                                      (r.Status == Status.Confirmed || r.Status == Status.Started));
        }

        // 9. Reservation List (Statusa görə sıralayırıq)
        public List<ReservedItem> GetAll()
        {
            return _context.ReservedItems
                           .Include(r => r.Book)
                           .OrderBy(r => r.Status)
                           .ToList();
        }

        // 10. Change Reservation Status
        public ReservedItem? GetById(int id)
        {
            return _context.ReservedItems.FirstOrDefault(r => r.Id == id);
        }

        public void UpdateStatus(ReservedItem item, Status newStatus)
        {
            item.Status = newStatus;
            _context.SaveChanges();
        }

        // 11. User's Reservations List
        public List<ReservedItem> GetByFinCode(string finCode)
        {
            return _context.ReservedItems
                           .Include(r => r.Book)
                           .Where(r => r.FinCode == finCode)
                           .ToList();
        }
        public void Update(ReservedItem item)
        {
            _context.ReservedItems.Update(item);
            _context.SaveChanges();
        }
    }
}
