﻿// <auto-generated />
using System;
using MIWE.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MIWE.Data.Migrations
{
    [DbContext(typeof(WorkerContext))]
    [Migration("20210124160109_Unique")]
    partial class Unique
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MIWE.Data.Entities.JobSchedule", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsRunning")
                        .HasColumnType("bit");

                    b.Property<Guid>("MainJob")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("NextJobs")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Scheduling")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("JobSchedules");
                });

            modelBuilder.Entity("MIWE.Data.Instance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CPUThreshold")
                        .HasColumnType("int");

                    b.Property<string>("IP")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDown")
                        .HasColumnType("bit");

                    b.Property<bool>("IsMaster")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Instances");
                });

            modelBuilder.Entity("MIWE.Data.Job", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsRunning")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("OSType")
                        .HasColumnType("int");

                    b.Property<string>("PluginPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PluginType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Jobs");
                });

            modelBuilder.Entity("MIWE.Data.JobSession", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DateEnd")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateStart")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("EntityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("InstanceId")
                        .HasColumnType("int");

                    b.Property<bool>("IsSuccess")
                        .HasColumnType("bit");

                    b.Property<string>("ResultContentType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ResultPath")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("JobSessions");
                });
#pragma warning restore 612, 618
        }
    }
}
