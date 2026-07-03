using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLibraryMiniProject.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibraryMiniProject.Persistence.Configurations
{
    public class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            // Primary Key təyin edirik
            builder.HasKey(a => a.Id);

            // Ad mütləqdir və maksimum 50 simvoldur
            builder.Property(a => a.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            // Surname? olduğu üçün bazada NULL (optional) ola bilər
            builder.Property(a => a.Surname)
                   .HasMaxLength(50);

            // Enum-ı bazaya rəqəm kimi yox, "Male", "Female" kimi yazsın deyə string-ə çeviririk
            builder.Property(a => a.Gender)
                   .HasConversion<string>()
                   .HasMaxLength(20);
        }
    }
}
