﻿using Microsoft.EntityFrameworkCore;
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

        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(@"Server=tcp:miwe.database.windows.net,1433;Initial Catalog=Worker;Persist Security Info=False;User ID=feri;Password=Repeat44;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-J2GU11K;Database=Worker;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<JobSessionDto>().HasNoKey();            
            modelBuilder.Entity<JobScheduleLastSessionDto>().HasNoKey();
            modelBuilder.Entity<JobSchedulePipelineDto>().HasNoKey();


            modelBuilder.Entity<Job>().HasIndex(n => n.Name).IsUnique();

            //only for migration uncomment
            //modelBuilder.Ignore<JobSessionDto>();
            //modelBuilder.Ignore<JobScheduleLastSessionDto>();
            //modelBuilder.Ignore<JobSchedulePipelineDto>();
        }
    }
}
