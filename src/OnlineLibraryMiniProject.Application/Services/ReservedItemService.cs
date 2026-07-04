using OnlineLibraryMiniProject.Application.Interfaces.Repositories;
using OnlineLibraryMiniProject.Application.Interfaces.Services;
using OnlineLibraryMiniProject.Domain.Entities;
using OnlineLibraryMiniProject.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibraryMiniProject.Application.Services
{
    public class ReservedItemService : IReservedItemService
    {
        private readonly IReservedItemRepository _reservations;
        private readonly IBookRepository _books;

        public ReservedItemService(IReservedItemRepository reservations, IBookRepository books)
        {
            _reservations = reservations;
            _books = books;
        }

        // 1. Create - Yeni Rezervasiya Yaradılması
        public void Create(ReservedItem item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item), "Rezervasiya melumatları boş ola bilmez!");

            if (string.IsNullOrWhiteSpace(item.FinCode))
                throw new ArgumentException("FIN Kod boş ola bilmez!", nameof(item.FinCode));

            item.FinCode = item.FinCode.Trim();

            if (item.EndDate <= item.StartDate)
                throw new ArgumentException("Qaytarılma tarixi götürülme tarixindən evvel ve ya eyni gün ola bilmez!");

            // Kitabın mövcudluğunu yoxlayırıq
            var book = _books.GetById(item.BookId);
            if (book is null)
                throw new InvalidOperationException("Rezerv edilmək istenilən kitab tapılmadı!");

            // Biznes Qaydası 1: Bir istifadəçi eyni anda maksimum 3 aktiv kitab rezerv edə bilər
            int activeReserv = _reservations.GetAll().Count(r =>
                string.Equals(r.FinCode, item.FinCode, StringComparison.OrdinalIgnoreCase) &&
                (r.Status == Status.Confirmed || r.Status == Status.Started));

            if (activeReserv >= 3)
                throw new InvalidOperationException("Bu FIN koda sahib istifadeçi eyni anda maksimum 3 aktiv kitab götüre biler!");

            // Biznes Qaydası 2: Kitabın seçilmiş tarixlərdə başqası tərəfindən rezerv olunub-olunmadığını yoxlayırıq
            bool hasReserv = _reservations.GetAll().Any(r =>
                r.BookId == item.BookId &&
                (r.Status == Status.Confirmed || r.Status == Status.Started) &&
                item.StartDate < r.EndDate && item.EndDate > r.StartDate);

            if (hasReserv)
                throw new InvalidOperationException("Bu kitab seçilmiş tarixlər aralığında artıq rezerv edilib!");

            // İlk yarananda status avtomatik Təsdiqlənmiş (Confirmed) olur
            item.Status = Status.Confirmed;
            _reservations.Create(item);
        }

        // 2. Delete - Rezervasiyanın Silinməsi
        public void Delete(int id)
        {
            var item = _reservations.GetById(id);
            if (item is null) return;

            // Biznes Qaydası: Əgər kitab artıq oxucuya verilibsə (Started), bu rezervasiya silinə bilməz
            if (item.Status == Status.Started)
                throw new InvalidOperationException("Oxucuya verilmiş (Aktiv) kitabların rezervasiyası siline bilmez!");

            _reservations.Delete(id);
        }

        // 3. GetAll - Bütün Rezervasiyaların Siyahısı (Statusa görə filtrləmə ilə)
        public List<ReservedItem> GetAll(Status? status = null)
        {
            var rb = _reservations.GetAll().AsQueryable();

            if (status.HasValue)
            {
                rb = rb.Where(r => r.Status == status.Value);
            }

            return rb.OrderByDescending(r => r.StartDate).ToList();
        }

        // 4. GetByUser - İstifadəçinin FIN koduna görə rezervasiyaları
        public List<ReservedItem> GetByUser(string finCode)
        {
            var fin = finCode?.Trim() ?? string.Empty;

            return _reservations.GetAll()
                                .Where(r => string.Equals(r.FinCode, fin, StringComparison.OrdinalIgnoreCase))
                                .OrderByDescending(r => r.StartDate)
                                .ToList();
        }

        // 5. Update - Rezervasiya Statusunun Dəyişdirilməsi (Dövlət Maşını / State Machine məntiqi)
        public void Update(int reservationId, Status newStatus)
        {
            var item = _reservations.GetById(reservationId);
            if (item is null) return;
            if (item.Status == newStatus) return;

            bool allowed = false;

            // Status keçid qaydalarının yoxlanılması
            if (item.Status == Status.Confirmed)
            {
                // Təsdiqlənmiş rezervasiya ya başlaya bilər (oxucu gəlib götürər), ya da ləğv edilə bilər
                allowed = (newStatus == Status.Started || newStatus == Status.Canceled);
            }
            else if (item.Status == Status.Started)
            {
                // Başlamış (oxucuda olan) kitab ya tamamlana bilər (geri qaytarılar), ya da ləğv edilə bilər
                allowed = (newStatus == Status.Completed || newStatus == Status.Canceled);
            }
            else
            {
                // Completed və ya Canceled statusunda olan rezervasiyanın statusu yenidən dəyişdirilə bilməz
                allowed = false;
            }

            if (!allowed)
                throw new InvalidOperationException("Bu status keçidi qeyri-qanunidir!");

            item.Status = newStatus;
            _reservations.Update(item);
        }
    }
}
