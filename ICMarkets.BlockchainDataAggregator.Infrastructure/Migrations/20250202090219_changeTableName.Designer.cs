﻿// <auto-generated />
using System;
using ICMarkets.BlockchainDataAggregator.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ICMarkets.BlockchainDataAggregator.Infrastructure.Migrations
{
    [DbContext(typeof(BlockchainDbContext))]
    [Migration("20250202090219_changeTableName")]
    partial class changeTableName
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.12");

            modelBuilder.Entity("ICMarkets.BlockchainDataAggregator.Domain.BlockchainData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Height")
                        .HasColumnType("INTEGER");

                    b.Property<int>("HighFeePerKb")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LastForkHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("LastForkHeight")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LatestUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("LowFeePerKb")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MediumFeePerKb")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("PeerCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PreviousHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PreviousUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Time")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("UnconfirmedCount")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Hash", "Height")
                        .IsUnique();

                    b.ToTable("BlockchainData", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
