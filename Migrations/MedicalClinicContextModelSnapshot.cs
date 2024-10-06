﻿// <auto-generated />
using System;
using MedicalClinic.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MedicalClinic.Migrations
{
    [DbContext(typeof(MedicalClinicContext))]
    partial class MedicalClinicContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MedicalClinic.Models.DoctorType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("DoctorTypes");
                });

            modelBuilder.Entity("MedicalClinic.Models.Human", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("Office")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Patronymic")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Humans");
                });

            modelBuilder.Entity("MedicalClinic.Models.HumanDoctorType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DoctorTypeId")
                        .HasColumnType("int");

                    b.Property<int>("HumanId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DoctorTypeId");

                    b.HasIndex("HumanId");

                    b.ToTable("HumanDoctorTypes");
                });

            modelBuilder.Entity("MedicalClinic.Models.Service", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Services");
                });

            modelBuilder.Entity("MedicalClinic.Models.ServiceDoctorType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DoctorTypeId")
                        .HasColumnType("int");

                    b.Property<int>("ServiceId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DoctorTypeId");

                    b.HasIndex("ServiceId");

                    b.ToTable("ServiceDoctorTypes");
                });

            modelBuilder.Entity("MedicalClinic.Models.ServiceList", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("DoctorId")
                        .HasColumnType("int");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<int>("ServiceId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("DoctorId");

                    b.HasIndex("ServiceId");

                    b.ToTable("ServiceLists");
                });

            modelBuilder.Entity("MedicalClinic.Models.HumanDoctorType", b =>
                {
                    b.HasOne("MedicalClinic.Models.DoctorType", "DoctorType")
                        .WithMany("HumanDoctorTypes")
                        .HasForeignKey("DoctorTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MedicalClinic.Models.Human", "Human")
                        .WithMany("HumanDoctorTypes")
                        .HasForeignKey("HumanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DoctorType");

                    b.Navigation("Human");
                });

            modelBuilder.Entity("MedicalClinic.Models.ServiceDoctorType", b =>
                {
                    b.HasOne("MedicalClinic.Models.DoctorType", "DoctorType")
                        .WithMany()
                        .HasForeignKey("DoctorTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MedicalClinic.Models.Service", "Service")
                        .WithMany("ServiceDoctorTypes")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DoctorType");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("MedicalClinic.Models.ServiceList", b =>
                {
                    b.HasOne("MedicalClinic.Models.Human", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("MedicalClinic.Models.Human", "Doctor")
                        .WithMany()
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("MedicalClinic.Models.Service", "Service")
                        .WithMany()
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");

                    b.Navigation("Doctor");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("MedicalClinic.Models.DoctorType", b =>
                {
                    b.Navigation("HumanDoctorTypes");
                });

            modelBuilder.Entity("MedicalClinic.Models.Human", b =>
                {
                    b.Navigation("HumanDoctorTypes");
                });

            modelBuilder.Entity("MedicalClinic.Models.Service", b =>
                {
                    b.Navigation("ServiceDoctorTypes");
                });
#pragma warning restore 612, 618
        }
    }
}
