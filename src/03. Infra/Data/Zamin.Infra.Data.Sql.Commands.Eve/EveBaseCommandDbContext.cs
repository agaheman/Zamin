using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using Zamin.Core.Domain.Eve.Entities;
using Zamin.Utilities.Services.Users;

namespace Zamin.Infra.Data.Sql.Commands.Eve
{
    public class EveBaseCommandDbContext : DbContext
    {
        private const string GuidString = "Guid";
        private const string CreatedByString = "CreatedBy";
        private const string ModifiedByString = "ModifiedBy";
        private const string LastModifiedDateString = "LastModifiedDate";
        private const string CreationDateString = "CreationDate";

        private readonly string _connectionString;
        private readonly IUserInfoService _userInfoService;

        public readonly string CommandConnectionString;
        public readonly string QueryConnectionString;
        public bool IsUniformPersianCharactersEnabled = true;
        public bool IsAuditLogEnabled = true;

        public EveBaseCommandDbContext(DbContextOptions options) : base(options)
        {
            QueryConnectionString = CommandConnectionString = base.Database.GetDbConnection().ConnectionString;
        }

        public EveBaseCommandDbContext(IUserInfoService userInfoSvc)
        {
            _userInfoService = userInfoSvc;
        }

        public EveBaseCommandDbContext(IUserInfoService userInfoSvc, string connectionString)
        {
            _userInfoService = userInfoSvc;
            _connectionString = connectionString;
        }

        public EveBaseCommandDbContext(IUserInfoService userInfoSvc, string commandConnectionString, string queryConnectionString)
        {
            _userInfoService = userInfoSvc;

            QueryConnectionString = queryConnectionString;
            CommandConnectionString = _connectionString = commandConnectionString;
        }

        public EveBaseCommandDbContext(IUserInfoService userInfoSvc, DbContextOptions options) : base(options)
        {
            _userInfoService = userInfoSvc;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
                options.UseSqlServer(_connectionString, op =>
                {
                    op.CommandTimeout(600);
                });
        }

        public override int SaveChanges()
        {
            if (_userInfoService is null)
                return base.SaveChanges();

            ProcessAdded();
            ProcessModefied();

            return base.SaveChanges();
        }

        private void ProcessAdded()
        {
            DateTime currentDatetime = DateTime.Now;

            var addedEntries = ChangeTracker.Entries<IEveIdentifiable>().Where(p => p.State == EntityState.Added);

            foreach (EntityEntry<IEveIdentifiable> added in addedEntries)
            {
                if (IsUniformPersianCharactersEnabled)
                    UniformPersianCharacters(added.Entity);

                SetBaseProperties(added);
            }

            void SetBaseProperties(EntityEntry<IEveIdentifiable> entity)
            {
                if (Guid.Parse(entity.Property(GuidString).CurrentValue.ToString()) == Guid.Empty)
                    entity.Property(GuidString).CurrentValue = Guid.NewGuid();

                if (entity.Property(CreatedByString).CurrentValue == null)
                    entity.Property(CreatedByString).CurrentValue = (long)_userInfoService.UserId();

                if (entity.Property(CreationDateString).CurrentValue == null)
                    entity.Property(CreationDateString).CurrentValue = currentDatetime;

                if (entity.Property(LastModifiedDateString).CurrentValue == null)
                    entity.Property(LastModifiedDateString).CurrentValue = currentDatetime;
            }
        }

        private void ProcessModefied()
        {
            DateTime currentDateTime = DateTime.Now;

            var modifiedEntries = ChangeTracker.Entries<IEveIdentifiable>().Where(p => p.State == EntityState.Modified);

            foreach (EntityEntry<IEveIdentifiable> modified in modifiedEntries)
            {
                if (IsUniformPersianCharactersEnabled)
                    UniformPersianCharacters(modified.Entity);

                SetBaseProperties(modified);
            }

            void SetBaseProperties(EntityEntry<IEveIdentifiable> entity)
            {
                entity.Property(ModifiedByString).CurrentValue = (long)_userInfoService.UserId();
                entity.Property(LastModifiedDateString).CurrentValue = currentDateTime;

                if (entity.Property(CreatedByString).CurrentValue == null)
                    entity.Property(CreatedByString).CurrentValue = (long)_userInfoService.UserId();
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var mutableProperties = modelBuilder.Model.GetEntityTypes()
                                                      .SelectMany(t => t.GetProperties())
                                                      .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?));

            foreach (var property in mutableProperties)
            {
                property.SetColumnType("datetime");
            }
        }

        #region Transaction

        private bool TransactionOnTheFly { get; set; } = false;

        internal IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            TransactionOnTheFly = true;
            return base.Database.BeginTransaction(isolationLevel);
        }

        internal void CommitTransaction()
        {
            if (base.Database.CurrentTransaction != null)
                base.Database.CommitTransaction();
            TransactionOnTheFly = false;
        }

        internal void RollbackTransaction()
        {
            if (base.Database.CurrentTransaction != null)
                base.Database.RollbackTransaction();
            TransactionOnTheFly = false;
        }

        #endregion Transaction


        public void UniformPersianCharacters<T>(T inputObject) where T : class
        {
            var objectProps = inputObject.GetType().GetProperties();
            foreach (var prop in objectProps)
            {
                var t = prop.PropertyType;
                if (prop.PropertyType == typeof(string) && prop.CanWrite)
                {
                    var propValue = prop.GetValue(inputObject);
                    if (propValue != null)
                    {
                        var uniformedValue = UniformPersianCharacters(propValue.ToString());
                        prop.SetValue(inputObject, uniformedValue);
                    }
                }
            }

            static string UniformPersianCharacters(string inputString)
            {
                return inputString.Replace((char)1610, (char)1740).Replace((char)1603, (char)1705);
            }
        }
    }
}