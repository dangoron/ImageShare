using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ImageShare.Models;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ImageShare.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().HasMany(_ => _.Posts).WithOne().OnDelete(DeleteBehavior.Restrict);
            builder.Entity<ApplicationUser>().HasMany(_ => _.Comments).WithOne().OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Comment>().HasOne(_ => _.User).WithMany().OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Post>().HasOne(_ => _.User).WithMany().OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Post>().HasMany(_ => _.Comments).WithOne().OnDelete(DeleteBehavior.Restrict);
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public virtual DbSet<Image> Images { get; set; }

        public virtual DbSet<Post> Posts { get; set; }

        public virtual DbSet<Comment> Comments { get; set; }
    }
}
