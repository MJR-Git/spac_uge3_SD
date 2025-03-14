﻿// <auto-generated />
using CerealApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CerealApi.Migrations
{
    [DbContext(typeof(CerealDb))]
    [Migration("20250314103656_UpdatedApi")]
    partial class UpdatedApi
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.3");

            modelBuilder.Entity("CerealApi.Models.Cereal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Calories")
                        .HasColumnType("INTEGER");

                    b.Property<float?>("Carbo")
                        .HasColumnType("REAL");

                    b.Property<float?>("Cups")
                        .HasColumnType("REAL");

                    b.Property<int?>("Fat")
                        .HasColumnType("INTEGER");

                    b.Property<float?>("Fiber")
                        .HasColumnType("REAL");

                    b.Property<string>("Image")
                        .HasColumnType("TEXT");

                    b.Property<string>("Mfr")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Potass")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Protein")
                        .HasColumnType("INTEGER");

                    b.Property<float?>("Rating")
                        .HasColumnType("REAL");

                    b.Property<int?>("Shelf")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Sodium")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Sugar")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Vitamins")
                        .HasColumnType("INTEGER");

                    b.Property<float?>("Weight")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("Cereals");
                });
#pragma warning restore 612, 618
        }
    }
}
