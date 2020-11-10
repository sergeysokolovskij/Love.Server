using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Apis.Utils;
using System.Linq;
using Api.Utils;

namespace Api.DAL.Base
{
	public class ApplicationContext : IdentityDbContext<User>
	{
		private readonly ILogger<ApplicationContext> logger;
		public ApplicationContext(DbContextOptions options,
			ILoggerFactory loggerFactory) : base(options)
		{
			this.logger = loggerFactory.CreateLogger<ApplicationContext>();
		}

		public void Migrate()
		{
			var pending = Database.GetPendingMigrations();
			if (pending.Any())
			{
				logger.LogInformation("Миграции были применены");
				Database.Migrate();
			}
			logger.LogInformation("Миграции не были применены");
		}

		#region Entities

		public DbSet<LongSession> LongSessions { get; set; } //Хранилище refresh-tokenов
		public DbSet<Cypher> Cyphers { get; set; } //Хранилище 

		#endregion

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<LongSession>().HasKey(x => x.Id);
			modelBuilder.Entity<Cypher>().HasKey(x => x.Name);

			modelBuilder.Entity<LongSession>()
				.HasOne(x => x.User)
				.WithMany(x => x.LongSessions);

			modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole[]
			{
				new IdentityRole()
				{
					Name = "User",
					NormalizedName = "USER"
				},
				new IdentityRole()
				{
					Name = "Admin",
					NormalizedName = "ADMIN"
				}
			});

			modelBuilder.Entity<Cypher>().HasData(new Cypher[]
			{
				new Cypher()
				{
					Name = "EmailCypher",
					Secret = CryptoRandomizer.GenerateSecurityKey(32)
				}
			});
		}
	}
}
