// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using VBkg.TgClientBot.Data;

#nullable disable

namespace VBkg.TgClientBot.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20221030163235_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("VBkg.TgClientBot.Data.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("RemovedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("TelegramUserId")
                        .HasColumnType("bigint");

                    b.Property<long>("VbkgUserId")
                        .HasColumnType("bigint");

                    b.Property<string>("VbkgUserToken")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("TelegramUserId")
                        .IsUnique()
                        .HasFilter("\"RemovedAt\" is not null");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("VBkg.TgClientBot.Data.Entities.UserState", b =>
                {
                    b.Property<long>("TelegramUserId")
                        .HasColumnType("bigint");

                    b.Property<long>("TelegramChatId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("TelegramUserId", "TelegramChatId");

                    b.ToTable("UserStates");
                });
#pragma warning restore 612, 618
        }
    }
}
