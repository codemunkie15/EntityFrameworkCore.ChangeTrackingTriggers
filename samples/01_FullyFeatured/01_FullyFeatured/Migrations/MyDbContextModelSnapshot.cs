﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using _01_FullyFeatured;

#nullable disable

namespace _01FullyFeatured.Migrations
{
    [DbContext(typeof(MyDbContext))]
    partial class MyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("_01_FullyFeatured.DbModels.Permissions.Permission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Permissions", null, t =>
                        {
                            t.HasTrigger("ChangeTrackingTrigger");
                        });

                    b
                        .HasAnnotation("ChangeTrackingTriggers:ChangeEntityTypeName", "_01_FullyFeatured.DbModels.Permissions.PermissionChange")
                        .HasAnnotation("ChangeTrackingTriggers:TriggerNameFormat", "CustomTriggerName_{0}")
                        .HasAnnotation("ChangeTrackingTriggers:Use", true);
                });

            modelBuilder.Entity("_01_FullyFeatured.DbModels.Permissions.PermissionChange", b =>
                {
                    b.Property<int>("ChangeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ChangeId"));

                    b.Property<DateTimeOffset>("ChangedAt")
                        .HasColumnType("datetimeoffset")
                        .HasAnnotation("ChangeTrackingTriggers:IsChangeContextColumn", true)
                        .HasAnnotation("ChangeTrackingTriggers:IsChangedAtColumn", true);

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OperationType")
                        .HasColumnType("int")
                        .HasColumnName("OperationTypeId")
                        .HasAnnotation("ChangeTrackingTriggers:IsChangeContextColumn", true)
                        .HasAnnotation("ChangeTrackingTriggers:IsOperationTypeColumn", true);

                    b.HasKey("ChangeId");

                    b.HasIndex("Id");

                    b.ToTable("PermissionChanges", (string)null);
                });

            modelBuilder.Entity("_01_FullyFeatured.DbModels.Users.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DateOfBirth")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users", null, t =>
                        {
                            t.HasTrigger("ChangeTrackingTrigger")
                                .HasDatabaseName("ChangeTrackingTrigger1");
                        });

                    b
                        .HasAnnotation("ChangeTrackingTriggers:ChangeEntityTypeName", "_01_FullyFeatured.DbModels.Users.UserChange")
                        .HasAnnotation("ChangeTrackingTriggers:Use", true);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DateOfBirth = "01/01/2000",
                            Name = "Robert"
                        });
                });

            modelBuilder.Entity("_01_FullyFeatured.DbModels.Users.UserChange", b =>
                {
                    b.Property<int>("ChangeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ChangeId"));

                    b.Property<int>("ChangeSource")
                        .HasColumnType("int")
                        .HasAnnotation("ChangeTrackingTriggers:IsChangeContextColumn", true)
                        .HasAnnotation("ChangeTrackingTriggers:IsChangeSourceColumn", true);

                    b.Property<DateTimeOffset>("ChangedAt")
                        .HasColumnType("datetimeoffset")
                        .HasAnnotation("ChangeTrackingTriggers:IsChangeContextColumn", true)
                        .HasAnnotation("ChangeTrackingTriggers:IsChangedAtColumn", true);

                    b.Property<int>("ChangedById")
                        .HasColumnType("int");

                    b.Property<string>("DateOfBirth")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OperationType")
                        .HasColumnType("int")
                        .HasColumnName("OperationTypeId")
                        .HasAnnotation("ChangeTrackingTriggers:IsChangeContextColumn", true)
                        .HasAnnotation("ChangeTrackingTriggers:IsOperationTypeColumn", true);

                    b.HasKey("ChangeId");

                    b.HasIndex("ChangedById");

                    b.HasIndex("Id");

                    b.ToTable("UserChanges", (string)null);
                });

            modelBuilder.Entity("_01_FullyFeatured.DbModels.Permissions.PermissionChange", b =>
                {
                    b.HasOne("_01_FullyFeatured.DbModels.Permissions.Permission", "TrackedEntity")
                        .WithMany("Changes")
                        .HasForeignKey("Id")
                        .HasAnnotation("ChangeTrackingTriggers:HasNoCheckConstraint", true);

                    b.Navigation("TrackedEntity");
                });

            modelBuilder.Entity("_01_FullyFeatured.DbModels.Users.UserChange", b =>
                {
                    b.HasOne("_01_FullyFeatured.DbModels.Users.User", "ChangedBy")
                        .WithMany()
                        .HasForeignKey("ChangedById")
                        .HasAnnotation("ChangeTrackingTriggers:HasNoCheckConstraint", true)
                        .HasAnnotation("ChangeTrackingTriggers:IsChangeContextColumn", true)
                        .HasAnnotation("ChangeTrackingTriggers:IsChangedByColumn", true);

                    b.HasOne("_01_FullyFeatured.DbModels.Users.User", "TrackedEntity")
                        .WithMany("Changes")
                        .HasForeignKey("Id")
                        .HasAnnotation("ChangeTrackingTriggers:HasNoCheckConstraint", true);

                    b.Navigation("ChangedBy");

                    b.Navigation("TrackedEntity");
                });

            modelBuilder.Entity("_01_FullyFeatured.DbModels.Permissions.Permission", b =>
                {
                    b.Navigation("Changes");
                });

            modelBuilder.Entity("_01_FullyFeatured.DbModels.Users.User", b =>
                {
                    b.Navigation("Changes");
                });
#pragma warning restore 612, 618
        }
    }
}
