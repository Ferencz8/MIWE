using Microsoft.EntityFrameworkCore;
using MIWE.Data.Dtos;
using MIWE.Data.Entities;
using System;

namespace MIWE.Data
{
    public class WorkerContext : DbContext
    {
        public WorkerContext()
        {

        }

        public WorkerContext(DbContextOptions<WorkerContext> options)
            : base(options)
        {

        }

        public DbSet<JobSession> JobSessions { get; set; }

        public DbSet<Job> Jobs { get; set; }

        public DbSet<JobSchedule> JobSchedules { get; set; }

        public DbSet<Instance> Instances { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-4VJUTBN;Database=Worker;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<JobSessionDto>().HasNoKey();
            modelBuilder.Entity<JobScheduleDto>().HasNoKey();
        }
    }
}
