﻿// <auto-generated />
using LeagueStatusBot.RPGEngine.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LeagueStatusBot.RPGEngine.Data.Migrations
{
    [DbContext(typeof(GameDbContext))]
    [Migration("20231002175711_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.10");

            modelBuilder.Entity("LeagueStatusBot.RPGEngine.Data.Entities.BeingEntity", b =>
                {
                    b.Property<ulong>("DiscordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("EloRating")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Losses")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<ulong>("ServerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Wins")
                        .HasColumnType("INTEGER");

                    b.HasKey("DiscordId");

                    b.HasIndex("ServerId");

                    b.ToTable("Beings");
                });

            modelBuilder.Entity("LeagueStatusBot.RPGEngine.Data.Entities.ServerEntity", b =>
                {
                    b.Property<ulong>("ServerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.HasKey("ServerId");

                    b.ToTable("Servers");
                });

            modelBuilder.Entity("LeagueStatusBot.RPGEngine.Data.Entities.BeingEntity", b =>
                {
                    b.HasOne("LeagueStatusBot.RPGEngine.Data.Entities.ServerEntity", "Server")
                        .WithMany("Beings")
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("LeagueStatusBot.RPGEngine.Data.Entities.ServerEntity", b =>
                {
                    b.Navigation("Beings");
                });
#pragma warning restore 612, 618
        }
    }
}