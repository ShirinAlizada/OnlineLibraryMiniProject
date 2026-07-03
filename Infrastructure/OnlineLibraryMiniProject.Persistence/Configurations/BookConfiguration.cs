using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLibraryMiniProject.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibraryMiniProject.Persistence.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            // Primary Key təyin edirik
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(b => b.PageCount)
                   .IsRequired();

            // Əlaqə: Bir Müəllifin çoxlu Kitabı ola bilər (Author 1 -> Many Books)
            builder.HasOne(b => b.Author)
                   .WithMany(a => a.Books)
                   .HasForeignKey(b => b.AuthorId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
