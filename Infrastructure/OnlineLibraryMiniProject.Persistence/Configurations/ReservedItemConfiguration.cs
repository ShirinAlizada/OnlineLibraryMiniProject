using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLibraryMiniProject.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibraryMiniProject.Persistence.Configurations
{
    public class ReservedItemConfiguration : IEntityTypeConfiguration<ReservedItem>
    {
        public void Configure(EntityTypeBuilder<ReservedItem> builder)
        {
            // Primary Key təyin edirik
            builder.HasKey(r => r.Id);

            builder.Property(r => r.FinCode)
                   .IsRequired()
                   .HasMaxLength(7); // FinCode strukturu sabit 7 simvoldur

            builder.Property(r => r.StartDate)
                   .IsRequired();

            builder.Property(r => r.EndDate)
                   .IsRequired();

            // Status enum-ını string kimi qeyd edirik ("Confirmed", "Started" və s.)
            builder.Property(r => r.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20);

            // Əlaqə: Bir Kitabın çoxlu Rezervasiya tarixçəsi ola bilər (Book 1 -> Many ReservedItems)
            builder.HasOne(r => r.Book)
                   .WithMany(b => b.ReservedItems)
                   .HasForeignKey(r => r.BookId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
