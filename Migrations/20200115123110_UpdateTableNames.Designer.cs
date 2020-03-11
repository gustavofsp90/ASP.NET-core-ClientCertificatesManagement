﻿// <auto-generated />
using System;
using CertificatesManager.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CertificatesManager.Migrations
{
    [DbContext(typeof(CertificatesManagerDBContext))]
    [Migration("20200115123110_UpdateTableNames")]
    partial class UpdateTableNames
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CertificatesManager.Models.Application", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Application");
                });

            modelBuilder.Entity("CertificatesManager.Models.Certificate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Environment");

                    b.Property<string>("Extension");

                    b.Property<DateTime>("From");

                    b.Property<string>("Guid");

                    b.Property<string>("InstallationLink");

                    b.Property<string>("Issuer");

                    b.Property<string>("Location");

                    b.Property<string>("Name");

                    b.Property<string>("OtherInfo");

                    b.Property<string>("Password");

                    b.Property<string>("Purpose");

                    b.Property<string>("SerialNumber");

                    b.Property<string>("SubjectName");

                    b.Property<DateTime>("To");

                    b.HasKey("Id");

                    b.ToTable("Certificate");
                });

            modelBuilder.Entity("CertificatesManager.Models.CertificateApplication", b =>
                {
                    b.Property<int>("CertificateId");

                    b.Property<int>("ApplicationId");

                    b.HasKey("CertificateId", "ApplicationId");

                    b.HasIndex("ApplicationId");

                    b.ToTable("CertificateApplication");
                });

            modelBuilder.Entity("CertificatesManager.Models.CertificateServer", b =>
                {
                    b.Property<int>("CertificateId");

                    b.Property<int>("ServerId");

                    b.HasKey("CertificateId", "ServerId");

                    b.HasIndex("ServerId");

                    b.ToTable("CertificateServer");
                });

            modelBuilder.Entity("CertificatesManager.Models.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Group");
                });

            modelBuilder.Entity("CertificatesManager.Models.GroupUser", b =>
                {
                    b.Property<int>("GroupId");

                    b.Property<int>("UserId");

                    b.HasKey("GroupId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("GroupUser");
                });

            modelBuilder.Entity("CertificatesManager.Models.Server", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Server");
                });

            modelBuilder.Entity("CertificatesManager.Models.Settings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DaysBeforeEmail");

                    b.Property<string>("EmailSubject");

                    b.Property<string>("EmailText");

                    b.Property<int?>("GroupId");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("CertificatesManager.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email");

                    b.Property<bool>("IsAdmin");

                    b.Property<string>("Name");

                    b.Property<string>("PhoneExtension");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("CertificatesManager.Models.CertificateApplication", b =>
                {
                    b.HasOne("CertificatesManager.Models.Application", "Application")
                        .WithMany()
                        .HasForeignKey("ApplicationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CertificatesManager.Models.Certificate", "Certificate")
                        .WithMany("Applications")
                        .HasForeignKey("CertificateId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CertificatesManager.Models.CertificateServer", b =>
                {
                    b.HasOne("CertificatesManager.Models.Certificate", "Certificate")
                        .WithMany("Servers")
                        .HasForeignKey("CertificateId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CertificatesManager.Models.Server", "Server")
                        .WithMany()
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CertificatesManager.Models.GroupUser", b =>
                {
                    b.HasOne("CertificatesManager.Models.Group", "Group")
                        .WithMany("Users")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CertificatesManager.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CertificatesManager.Models.Settings", b =>
                {
                    b.HasOne("CertificatesManager.Models.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId");
                });
#pragma warning restore 612, 618
        }
    }
}
