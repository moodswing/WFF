﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;
using WFF.Models;

namespace WFF.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("WFF.Models.Attachment", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<string>("Field");

                    b.Property<string>("FileName");

                    b.Property<int>("FormRequestID");

                    b.Property<string>("Location");

                    b.Property<string>("User");

                    b.HasKey("ID");

                    b.HasIndex("FormRequestID");

                    b.ToTable("Attachments");
                });

            modelBuilder.Entity("WFF.Models.FormRequest", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy");

                    b.Property<int>("FormId");

                    b.Property<string>("JSonFormData");

                    b.Property<string>("Reference");

                    b.Property<string>("StatusId");

                    b.Property<string>("UserAssigned");

                    b.HasKey("ID");

                    b.ToTable("FormRequests");
                });

            modelBuilder.Entity("WFF.Models.History", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Comment");

                    b.Property<DateTime>("Date");

                    b.Property<int>("FormRequestID");

                    b.Property<string>("Log");

                    b.Property<string>("User");

                    b.HasKey("ID");

                    b.HasIndex("FormRequestID");

                    b.ToTable("History");
                });

            modelBuilder.Entity("WFF.Models.UserProfile", b =>
                {
                    b.Property<int>("UserProfileID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Active");

                    b.Property<string>("Email");

                    b.Property<string>("Lastname");

                    b.Property<string>("Name");

                    b.Property<string>("Password");

                    b.HasKey("UserProfileID");

                    b.ToTable("PerfilesUsuarios");
                });

            modelBuilder.Entity("WFF.Models.Attachment", b =>
                {
                    b.HasOne("WFF.Models.FormRequest", "FormRequest")
                        .WithMany("Attachments")
                        .HasForeignKey("FormRequestID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WFF.Models.History", b =>
                {
                    b.HasOne("WFF.Models.FormRequest", "FormRequest")
                        .WithMany("History")
                        .HasForeignKey("FormRequestID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
