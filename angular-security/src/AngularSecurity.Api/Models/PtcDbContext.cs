﻿using AngularSecurity.Api.EntityClasses;
using Microsoft.EntityFrameworkCore;

namespace AngularSecurity.Api.Models
{
    public partial class PtcDbContext : DbContext
    {
        public PtcDbContext(DbContextOptions<PtcDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public DbSet<UserBase> Users { get; set; }
        public DbSet<UserClaim> Claims { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
