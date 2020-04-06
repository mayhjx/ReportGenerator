﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ReportGenerator.Data;

namespace ReportGenerator.Migrations.Report
{
    [DbContext(typeof(ReportContext))]
    [Migration("20200406060642_useSqlServer")]
    partial class useSqlServer
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ReportGenerator.Models.Report", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("ALE")
                        .HasColumnType("float");

                    b.Property<DateTime>("ApprovalDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Approver")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Bias")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndTestDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EvaluationDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("InvestigationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Investigator")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Item")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MatchInstrumentName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MatchReagentLot")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MatchResult")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("P")
                        .HasColumnType("float");

                    b.Property<string>("PicturePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SampleName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartTestDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TargetInstrumentName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TargetReagentLot")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TargetResult")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Technician")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Xc1")
                        .HasColumnType("float");

                    b.Property<double>("Xc2")
                        .HasColumnType("float");

                    b.Property<double>("a")
                        .HasColumnType("float");

                    b.Property<double>("aLCI")
                        .HasColumnType("float");

                    b.Property<double>("aUCI")
                        .HasColumnType("float");

                    b.Property<double>("b")
                        .HasColumnType("float");

                    b.Property<double>("bLCI")
                        .HasColumnType("float");

                    b.Property<double>("bUCI")
                        .HasColumnType("float");

                    b.HasKey("ID");

                    b.ToTable("Report");
                });
#pragma warning restore 612, 618
        }
    }
}
