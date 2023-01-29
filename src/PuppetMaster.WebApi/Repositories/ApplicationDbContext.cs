using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PuppetMaster.WebApi.Models.Database;

namespace PuppetMaster.WebApi.Repositories
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<GameUser>? GameUsers { get; set; }

        public DbSet<Game>? Games { get; set; }

        public DbSet<Room>? Rooms { get; set; }

        public DbSet<RoomUser>? RoomUsers { get; set; }

        public DbSet<Match>? Matches { get; set; }

        public DbSet<MatchTeam>? MatchTeams { get; set; }

        public DbSet<MatchTeamUser>? MatchTeamUsers { get; set; }

        public DbSet<Map>? Maps { get; set; }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            throw new InvalidOperationException("Use the async method");
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Application User
            builder.Entity<ApplicationUser>()
               .HasMany(au => au.GameUsers)
               .WithOne(gu => gu.ApplicationUser)
               .HasForeignKey(gu => gu.ApplicationUserId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
               .HasOne(au => au.RoomUser)
               .WithOne(ru => ru.ApplicationUser)
               .HasForeignKey<RoomUser>(ru => ru.ApplicationUserId)
               .OnDelete(DeleteBehavior.Cascade);

            // Game
            builder.Entity<Game>()
               .HasMany(g => g.GameUsers)
               .WithOne(gu => gu.Game)
               .HasForeignKey(gu => gu.GameId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Game>()
               .HasMany(g => g.Rooms)
               .WithOne(r => r.Game)
               .HasForeignKey(r => r.GameId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Game>()
               .HasMany(g => g.Matches)
               .WithOne(m => m.Game)
               .HasForeignKey(m => m.GameId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Game>()
               .HasMany(g => g.GameUsers)
               .WithOne(gu => gu.Game)
               .HasForeignKey(gu => gu.GameId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Game>()
                .HasMany(g => g.Maps)
                .WithOne(m => m.Game)
                .HasForeignKey(m => m.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            // Game User
            builder.Entity<GameUser>()
               .HasOne(gu => gu.ApplicationUser)
               .WithMany(au => au.GameUsers)
               .HasForeignKey(gu => gu.ApplicationUserId)
               .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<GameUser>()
               .HasOne(gu => gu.Game)
               .WithMany(au => au.GameUsers)
               .HasForeignKey(gu => gu.GameId)
               .OnDelete(DeleteBehavior.NoAction);

            // Room Game User
            builder.Entity<RoomUser>()
               .HasOne(ru => ru.Room)
               .WithMany(r => r.RoomUsers)
               .HasForeignKey(ru => ru.RoomId)
               .OnDelete(DeleteBehavior.NoAction);

            // Room
            builder.Entity<Room>()
               .HasOne(r => r.Game)
               .WithMany(g => g.Rooms)
               .HasForeignKey(r => r.GameId)
               .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Room>()
               .HasMany(r => r.RoomUsers)
               .WithOne(ru => ru.Room)
               .HasForeignKey(ru => ru.RoomId)
               .OnDelete(DeleteBehavior.Cascade);

            // Match
            builder.Entity<Match>()
               .HasOne(m => m.Game)
               .WithMany(g => g.Matches)
               .HasForeignKey(m => m.GameId)
               .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Match>()
               .HasMany(m => m.MatchTeams)
               .WithOne(mt => mt.Match)
               .HasForeignKey(mt => mt.MatchId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Match>()
               .HasMany(m => m.Users)
               .WithOne(au => au.Match)
               .HasForeignKey(au => au.MatchId)
               .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Match>()
               .HasOne(m => m.Room)
               .WithOne(r => r.Match)
               .HasForeignKey<Match>(r => r.RoomId)
               .OnDelete(DeleteBehavior.NoAction);

            // Match Team
            builder.Entity<MatchTeam>()
               .HasMany(mt => mt.MatchTeamUsers)
               .WithOne(mtu => mtu.MatchTeam)
               .HasForeignKey(mtu => mtu.MatchTeamId)
               .OnDelete(DeleteBehavior.Cascade);
             
            builder.Entity<MatchTeam>()
               .HasOne(mt => mt.Match)
               .WithMany(m => m.MatchTeams)
               .HasForeignKey(mt => mt.MatchId)
               .OnDelete(DeleteBehavior.NoAction);

            // Match Team User
            builder.Entity<MatchTeamUser>()
               .HasOne(mtu => mtu.MatchTeam)
               .WithMany(mt => mt.MatchTeamUsers)
               .HasForeignKey(mtu => mtu.MatchTeamId)
               .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<MatchTeamUser>()
               .HasOne(mtu => mtu.ApplicationUser)
               .WithMany(mt => mt.MatchTeamUsers)
               .HasForeignKey(mtu => mtu.ApplicationUserId)
               .OnDelete(DeleteBehavior.NoAction);

            // Map
            builder.Entity<Map>()
                .HasOne(m => m.Game)
                .WithMany(m => m.Maps)
                .HasForeignKey(m => m.GameId)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(builder);
        }

        private void AddTimestamps()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var userId = httpContext?.User.GetUserIdOrDefault();
            if (userId.HasValue)
            {
                var entities = ChangeTracker.Entries()
                    .Where(x => x.Entity is EntityBase && 
                                (x.State == EntityState.Added || x.State == EntityState.Modified));

                foreach (var entity in entities)
                {
                    if (entity.State == EntityState.Added)
                    {
                        ((EntityBase)entity.Entity).CreatedDate = DateTime.UtcNow;
                        ((EntityBase)entity.Entity).CreatedBy = userId.Value;
                    }
                    else
                    {
                        ((EntityBase)entity.Entity).ModifiedDate = DateTime.UtcNow;
                        ((EntityBase)entity.Entity).ModifiedBy = userId;
                    }
                }
            }
        }
    }
}
