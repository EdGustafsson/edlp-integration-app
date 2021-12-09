﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using learnpoint_test_consoleApp.Data;

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

            modelBuilder.Entity("learnpoint_test_consoleApp.Entities.Resource", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Resource");
                });

            modelBuilder.Entity("learnpoint_test_consoleApp.Entities.Resource", b =>
                {
                    b.OwnsOne("learnpoint_test_consoleApp.Entities.Resource+ExternalId", "SourceId", b1 =>
                        {
                            b1.Property<Guid>("ResourceId")
                                .HasColumnType("TEXT");

                            b1.Property<Guid>("GuidId")
                                .HasColumnType("TEXT");

                            b1.Property<int>("IType")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("IntId")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("SType")
                                .HasColumnType("INTEGER");

                            b1.HasKey("ResourceId");

                            b1.ToTable("Resource");

                            b1.WithOwner()
                                .HasForeignKey("ResourceId");
                        });

                    b.OwnsOne("learnpoint_test_consoleApp.Entities.Resource+ExternalId", "TargetId", b1 =>
                        {
                            b1.Property<Guid>("ResourceId")
                                .HasColumnType("TEXT");

                            b1.Property<Guid>("GuidId")
                                .HasColumnType("TEXT");

                            b1.Property<int>("IType")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("IntId")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("SType")
                                .HasColumnType("INTEGER");

                            b1.HasKey("ResourceId");

                            b1.ToTable("Resource");

                            b1.WithOwner()
                                .HasForeignKey("ResourceId");
                        });

                    b.Navigation("SourceId");

                    b.Navigation("TargetId");
                });
#pragma warning restore 612, 618
        }
    }
}
