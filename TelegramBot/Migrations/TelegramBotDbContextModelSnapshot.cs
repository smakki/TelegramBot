﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TelegramBot;

#nullable disable

namespace TelegramBot.Migrations
{
    [DbContext(typeof(TelegramBotDbContext))]
    partial class TelegramBotDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TelegramBot.Models.UserTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("AddedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("Completed")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("CompletionDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Message")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<bool>("Notificated")
                        .HasColumnType("boolean");

                    b.Property<int>("NumberOfSnoozes")
                        .HasColumnType("integer");

                    b.Property<DateTime>("ReminderDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("Remindered")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("TaskDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("TelegramId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("UserTasks");
                });

            modelBuilder.Entity("TelegramBot.Models.Users", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("ReminderToTaskMinutes")
                        .HasColumnType("integer");

                    b.Property<string>("TimeZone")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
