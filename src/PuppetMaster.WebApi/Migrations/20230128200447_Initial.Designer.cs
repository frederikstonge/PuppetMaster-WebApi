﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PuppetMaster.WebApi.Repositories;

#nullable disable

namespace PuppetMaster.WebApi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230128200447_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreApplication<System.Guid>", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ClientId")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ClientSecret")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("ConsentType")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DisplayNames")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Permissions")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostLogoutRedirectUris")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Properties")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RedirectUris")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Requirements")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("ClientId")
                        .IsUnique()
                        .HasFilter("[ClientId] IS NOT NULL");

                    b.ToTable("OpenIddictApplications", (string)null);
                });

            modelBuilder.Entity("OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreAuthorization<System.Guid>", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ApplicationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Properties")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Scopes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Subject")
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<string>("Type")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationId", "Status", "Subject", "Type");

                    b.ToTable("OpenIddictAuthorizations", (string)null);
                });

            modelBuilder.Entity("OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreScope<System.Guid>", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Descriptions")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DisplayNames")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Properties")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Resources")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.ToTable("OpenIddictScopes", (string)null);
                });

            modelBuilder.Entity("OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreToken<System.Guid>", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ApplicationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AuthorizationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ExpirationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Payload")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Properties")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RedemptionDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ReferenceId")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Status")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Subject")
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<string>("Type")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorizationId");

                    b.HasIndex("ReferenceId")
                        .IsUnique()
                        .HasFilter("[ReferenceId] IS NOT NULL");

                    b.HasIndex("ApplicationId", "Status", "Subject", "Type");

                    b.ToTable("OpenIddictTokens", (string)null);
                });

            modelBuilder.Entity("PuppetMaster.WebApi.Models.Database.ApplicationRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("PuppetMaster.WebApi.Models.Database.ApplicationUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("AvatarUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid?>("MatchId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("MatchId");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("PuppetMaster.WebApi.Models.Database.Game", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("IconUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PlayerCount")
                        .HasColumnType("int");

                    b.Property<int>("TeamCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("PuppetMaster.WebApi.Models.Database.GameUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ApplicationUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("GameId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Region")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UniqueGameId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("GameId");

                    b.ToTable("GameUsers");
                });

            modelBuilder.Entity("PuppetMaster.WebApi.Models.Database.Map", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("GameId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("Maps");
                });

            modelBuilder.Entity("PuppetMaster.WebApi.Models.Database.Match", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("GameId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LobbyId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("LobbyLeaderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Region")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("RoomId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("RoomId")
                        .IsUnique()
                        .HasFilter("[RoomId] IS NOT NULL");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("PuppetMaster.WebApi.Models.Database.MatchTeam", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("MatchId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("TeamIndex")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MatchId");

                    b.ToTable("MatchTeams");
                });

            modelBuilder.Entity("PuppetMaster.WebApi.Models.Database.MatchTeamUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ApplicationUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("HasJoined")
                        .HasColumnType("bit");

                    b.Property<bool>("IsCaptain")
                        .HasColumnType("bit");

                    b.Property<string>("MapVote")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("MatchTeamId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("MatchTeamId");

                    b.ToTable("MatchTeamUsers");
                });

            modelBuilder.Entity("PuppetMaster.WebApi.Models.Database.Room", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("GameId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Region")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("PuppetMaster.WebApi.Models.Database.RoomUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ApplicationUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsReady")
                        .HasColumnType("bit");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("RoomId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId")
                        .IsUnique();

                    b.HasIndex("RoomId");

                    b.ToTable("RoomUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("PuppetMaster.WebApi.Models.Database.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("PuppetMaster.WebApi.Models.Database.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("PuppetMaster.WebApi.Models.Database.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("PuppetMaster.WebApi.Models.Database.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PuppetMaster.WebApi.Models.Database.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("PuppetMaster.WebApi.Models.Database.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreAuthorization<System.Guid>", b =>
                {
                    b.HasOne("OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreApplication<System.Guid>", "Application")
                        .WithMany("Authorizations")
                        .HasForeignKey("ApplicationId");

                    b.Navigation("Application");
                });

            modelBuilder.Entity("OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreToken<System.Guid>", b =>
                {
                    b.HasOne("OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreApplication<System.Guid>", "Application")
                        .WithMany("Tokens")
                        .HasForeignKey("ApplicationId");

                    b.HasOne("OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreAuthorization<System.Guid>", "Authorization")
                        .WithMany("Tokens")
                        .HasForeignKey("AuthorizationId");

                    b.Navigation("Application");

                    b.Navigation("Authorization");
                });

            modelBuilder.Entity("PuppetMaster.WebApi.Models.Database.ApplicationUser", b =>
                {
                    b.HasOne("PuppetMaster.WebApi.Models.Database.Match", "Match")
                        .WithMany("Users")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Match");
                });

            modelBuilder.Entity("PuppetMaster.WebApi.Models.Database.GameUser", b =>
                {
                    b.HasOne("PuppetMaster.WebApi.Models.Database.ApplicationUser", "ApplicationUser")
                        .WithMany("GameUsers")
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("PuppetMaster.WebApi.Models.Database.Game", "Game")
                        .WithMany("GameUsers")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("ApplicationUser");

                    b.Navigation("Game");
                });

            modelBuilder.Entity("PuppetMaster.WebApi.Models.Database.Map", b =>
                {
                    b.HasOne("PuppetMaster.WebApi.Models.Database.Game", "Game")
                        .WithMany("Maps")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("PuppetMaster.WebApi.Models.Database.Match", b =>
                {
                    b.HasOne("PuppetMaster.WebApi.Models.Database.Game", "Game")
                        .WithMany("Matches")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("PuppetMaster.WebApi.Models.Database.Room", "Room")
                        .WithOne("Match")
                        .HasForeignKey("PuppetMaster.WebApi.Models.Database.Match", "RoomId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Game");

                    b.Navigation("Room");
                });

            modelBuilder.Entity("PuppetMaster.WebApi.Models.Database.MatchTeam", b =>
                {
                    b.HasOne("PuppetMaster.WebApi.Models.Database.Match", "Match")
                        .WithMany("MatchTeams")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Match");
                });

            modelBuilder.Entity("PuppetMaster.WebApi.Models.Database.MatchTeamUser", b =>
                {
                    b.HasOne("PuppetMaster.WebApi.Models.Database.ApplicationUser", "ApplicationUser")
                        .WithMany("MatchTeamUsers")
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("PuppetMaster.WebApi.Models.Database.MatchTeam", "MatchTeam")
                        .WithMany("MatchTeamUsers")
                        .HasForeignKey("MatchTeamId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("ApplicationUser");

                    b.Navigation("MatchTeam");
                });

            modelBuilder.Entity("PuppetMaster.WebApi.Models.Database.Room", b =>
                {
                    b.HasOne("PuppetMaster.WebApi.Models.Database.Game", "Game")
                        .WithMany("Rooms")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("PuppetMaster.WebApi.Models.Database.RoomUser", b =>
                {
                    b.HasOne("PuppetMaster.WebApi.Models.Database.ApplicationUser", "ApplicationUser")
                        .WithOne("RoomUser")
                        .HasForeignKey("PuppetMaster.WebApi.Models.Database.RoomUser", "ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PuppetMaster.WebApi.Models.Database.Room", "Room")
                        .WithMany("RoomUsers")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApplicationUser");

                    b.Navigation("Room");
                });

            modelBuilder.Entity("OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreApplication<System.Guid>", b =>
                {
                    b.Navigation("Authorizations");

                    b.Navigation("Tokens");
                });

            modelBuilder.Entity("OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreAuthorization<System.Guid>", b =>
                {
                    b.Navigation("Tokens");
                });

            modelBuilder.Entity("PuppetMaster.WebApi.Models.Database.ApplicationUser", b =>
                {
                    b.Navigation("GameUsers");

                    b.Navigation("MatchTeamUsers");

                    b.Navigation("RoomUser");
                });

            modelBuilder.Entity("PuppetMaster.WebApi.Models.Database.Game", b =>
                {
                    b.Navigation("GameUsers");

                    b.Navigation("Maps");

                    b.Navigation("Matches");

                    b.Navigation("Rooms");
                });

            modelBuilder.Entity("PuppetMaster.WebApi.Models.Database.Match", b =>
                {
                    b.Navigation("MatchTeams");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("PuppetMaster.WebApi.Models.Database.MatchTeam", b =>
                {
                    b.Navigation("MatchTeamUsers");
                });

            modelBuilder.Entity("PuppetMaster.WebApi.Models.Database.Room", b =>
                {
                    b.Navigation("Match");

                    b.Navigation("RoomUsers");
                });
#pragma warning restore 612, 618
        }
    }
}
