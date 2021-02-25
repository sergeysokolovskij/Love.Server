using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Apis.Utils;
using System.Linq;
using Api.Utils;
using Api.Dal;
using Api.Dal.Dev;
using Api.Dal.Auth;
using Api.Dal.Messanger;
using Api.Dal.Accounting;

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
			else
				logger.LogInformation("Миграции не были применены");
		}

		#region Entities

		public DbSet<LongSession> LongSessions { get; set; } //Хранилище refresh-tokenов
		public DbSet<Cypher> Cyphers { get; set; } //Хранилище 
		public DbSet<StrongKey> StrongKeys { get; set; } // уникальные 256-битные ключи для пользователя. используется для реализации протокола 
		public DbSet<Message> Messages { get; set; }
		public DbSet<PushTask> PushTasks { get; set; }
		public DbSet<Characters> Characters { get; set; }
		public DbSet<BaseCharecters> BaseCharacters { get; set; }
		public DbSet<City> Cities { get; set; }
		public DbSet<Country> Countries { get; set; }
		public DbSet<Like> Likes { get; set; }
		public DbSet<Picture> Pictures { get; set; }
		public DbSet<Profile> Profiles { get; set; }
		public DbSet<Visit> Visits { get; set; }
		public DbSet<UserPair> UserPairs { get; set; }
		public DbSet<Cordinate> Cordinates { get; set; }
		public DbSet<RsaKey> RsaKeys { get; set; }
		public DbSet<UserAccount> UserAccounts { get; set; } 
		public DbSet<UserToken> UserProcessingTokens { get; set; }
		public DbSet<Session> Sessions { get; set; }
		public DbSet<Connection> Connections { get; set; }
		public DbSet<Dialog> Dialogs { get; set; }

		#region Accounting

		public DbSet<AccountingComment> AccountingComments { get; set; }
		public DbSet<AccountingPlan> AccountingPlans { get; set; }
		public DbSet<Flow> Flows { get; set; }
		public DbSet<AccountingRecord> AccountingRecords { get; set; }

		public DbSet<UserOnlineAccounting> UserOnlineAccountings { get; set; }

		#endregion
		#endregion

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<LongSession>().HasKey(x => x.Id);
			modelBuilder.Entity<Cypher>().HasKey(x => x.Id);
			modelBuilder.Entity<StrongKey>().HasKey(x => x.Id);
			modelBuilder.Entity<Picture>().HasKey(x => x.Id);
			modelBuilder.Entity<Message>().HasKey(x => x.Id);
			modelBuilder.Entity<PushTask>().HasKey(x => x.Id);
			modelBuilder.Entity<City>().HasKey(x => x.Id);
			modelBuilder.Entity<Country>().HasKey(x => x.Id);
			modelBuilder.Entity<Like>().HasKey(x => x.Id);
			modelBuilder.Entity<Profile>().HasKey(x => x.Id);
			modelBuilder.Entity<Visit>().HasKey(x => x.Id);
			modelBuilder.Entity<UserPair>().HasKey(x => x.Id);
			modelBuilder.Entity<Cordinate>().HasKey(x => x.Id);
			modelBuilder.Entity<RsaKey>().HasKey(x => x.Id);
			modelBuilder.Entity<UserAccount>().HasKey(x => x.Id);
			modelBuilder.Entity<UserToken>().HasKey(x => x.Id);
			modelBuilder.Entity<Session>().HasKey(x => x.Id);
			modelBuilder.Entity<Connection>().HasKey(x => x.Id);
			modelBuilder.Entity<Dialog>().HasKey(x => x.Id);
			modelBuilder.Entity<AccountingComment>().HasKey(x => x.Id);
			modelBuilder.Entity<Flow>().HasKey(x => x.Id);
			modelBuilder.Entity<AccountingPlan>().HasKey(x => x.Id);
			modelBuilder.Entity<AccountingRecord>().HasKey(x => x.Id);

			modelBuilder.Entity<LongSession>()
				.HasOne(x => x.User)
				.WithMany(x => x.LongSessions);

			modelBuilder.Entity<Message>()
				.HasOne(x => x.Cypher);

			modelBuilder.Entity<PushTask>()
				.HasOne(x => x.User)
				.WithMany(x => x.PushTasks);

			modelBuilder.Entity<City>()
				.HasOne(x => x.Country)
				.WithMany(x => x.Cities);

			modelBuilder.Entity<Like>()
				.HasOne(x => x.User)
				.WithMany(x => x.Likes);

			modelBuilder.Entity<Visit>()
				.HasOne(x => x.User)
				.WithMany(x => x.Visits);

			modelBuilder.Entity<Picture>()
				.HasOne(x => x.User)
				.WithMany(x => x.Pictures);

			modelBuilder.Entity<Message>()
				.HasOne(x => x.Cypher);

			modelBuilder.Entity<Profile>()
				.HasMany(x => x.Characters)
				.WithOne(x => x.Profile);

			modelBuilder.Entity<Profile>()
				.HasOne(x => x.User)
				.WithOne(x => x.Profile)
				.HasForeignKey<Profile>(x => x.UserId);

			modelBuilder.Entity<Profile>()
				.HasOne(x => x.Country)
				.WithMany(x => x.Profiles);

			modelBuilder.Entity<City>()
				.HasMany(x => x.Profiles)
				.WithOne(x => x.City);

			modelBuilder.Entity<Cordinate>()
				.HasOne(x => x.User)
				.WithMany(x => x.Cordinates);

			modelBuilder.Entity<RsaKey>()
				.HasOne(x => x.User)
				.WithMany(x => x.RsaKeys);

			modelBuilder.Entity<User>()
				.HasMany(x => x.UserTokens)
				.WithOne(x => x.User);
			modelBuilder.Entity<User>()
				.HasMany(x => x.Sessions)
				.WithOne(x => x.User);

			modelBuilder.Entity<User>()
				.HasMany(x => x.Sessions)
				.WithOne(x => x.User);

			modelBuilder.Entity<StrongKey>()
				.HasOne(x => x.Cypher);

			modelBuilder.Entity<StrongKey>()
				.HasOne(x => x.User)
				.WithOne(x => x.StrongKey)
				.HasForeignKey<StrongKey>(x => x.UserId);

			modelBuilder.Entity<AccountingRecord>()
				.HasOne(x => x.User)
				.WithMany(x => x.AccountingRecords);
			

			modelBuilder.Entity<LongSession>().HasIndex(x => x.UserId);
			modelBuilder.Entity<Message>().HasIndex(x => new { x.ReceiverId, x.SenderId });
			modelBuilder.Entity<PushTask>().HasIndex(x => x.UserId);
			modelBuilder.Entity<Like>().HasIndex(x => x.UserId);
			modelBuilder.Entity<Visit>().HasIndex(x => x.UserId);
			modelBuilder.Entity<Profile>().HasIndex(x => x.UserId);
			modelBuilder.Entity<UserPair>().HasIndex(x => new { x.UserId1, x.UserId2 });
			modelBuilder.Entity<Message>().HasIndex(x => x.MessageId);

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
				},
				new IdentityRole()
				{
					Name = "ProtocoledUsers",
					NormalizedName = "PROTOCOLEDUSERS"
				}
			});

			modelBuilder.Entity<Cypher>().HasData(new Cypher[]
			{
				new Cypher()
				{
					Id = 1,
					Secret = CryptoRandomizer.GenerateSecurityKey(32)
				}
			});

			modelBuilder.Entity<UserAccount>().HasData(new UserAccount[]
			{
				new UserAccount()
				{
					Id = "1",
					Login = "test1@mail.ru",
					Password = "asddasSdas#112"
				},
				new UserAccount()
				{
					Id = "2",
					Login = "test2@mail.ru",
					Password = "asddasSdasdas#212"
				}
			});

			modelBuilder.Entity<AccountingPlan>().HasData(AccountingInitializers.GetAccountingsPlan());
		}
	}
}
