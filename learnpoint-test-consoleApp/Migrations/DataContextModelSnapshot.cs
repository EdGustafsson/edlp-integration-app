﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using learnpoint_test_consoleApp;

namespace learnpoint_test_consoleApp.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.8");

            modelBuilder.Entity("learnpoint_test_consoleApp.Models.Resource", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("SourceIdId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("TargetIdId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("SourceIdId");

                    b.HasIndex("TargetIdId");

                    b.ToTable("Resources");
                });

            modelBuilder.Entity("learnpoint_test_consoleApp.Models.Resource+ExternalId", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("GuidId")
                        .HasColumnType("TEXT");

                    b.Property<int>("IType")
                        .HasColumnType("INTEGER");

                    b.Property<int>("IntId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SType")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("ExternalId");
                });

            modelBuilder.Entity("learnpoint_test_consoleApp.Models.Resource", b =>
                {
                    b.HasOne("learnpoint_test_consoleApp.Models.Resource+ExternalId", "SourceId")
                        .WithMany()
                        .HasForeignKey("SourceIdId");

                    b.HasOne("learnpoint_test_consoleApp.Models.Resource+ExternalId", "TargetId")
                        .WithMany()
                        .HasForeignKey("TargetIdId");

                    b.Navigation("SourceId");

                    b.Navigation("TargetId");
                });
#pragma warning restore 612, 618
        }
    }
}
