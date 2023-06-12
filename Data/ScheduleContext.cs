using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestAplication.Models;

namespace TestAplication.Data
{
    public class ScheduleContext : DbContext
    {
        public ScheduleContext (DbContextOptions<ScheduleContext> options)
            : base(options)
        {
        }

        public DbSet<TestAplication.Models.Schedule> Schedule { get; set; }
        public DbSet<TestAplication.Models.UserModel> Users { get; set; }
        public DbSet<TestAplication.Models.RoleModel> Roles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Schedule>().ToTable("Schedule");
            modelBuilder.Entity<UserModel>().ToTable("Users");
            modelBuilder.Entity<RoleModel>().ToTable("Roles");
        }
    }
}
