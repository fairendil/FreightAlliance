using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FreightAlliance.Base.Migrations;
using FreightAlliance.Base.Models;
using FreightAlliance.Common.Attributes;
using FreightAlliance.Common.Interfaces;

namespace FreightAlliance.Base.Providers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using FreightAlliance.Common.Enums;

    [Export(nameof(DataProvider), typeof(IBaseProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class DataProvider : DbContext , IBaseProvider
    {
        private Task saveTask; 

        [ImportingConstructor]     
        public DataProvider()
            :base("Alliance")
        {
            this.Initialize();
        }

        public DataProvider(string connectionString) : base(connectionString)
        {
            this.Initialize();
        }

        private void Initialize()
        {
            Database.SetInitializer<DataProvider>(new DropCreateDatabaseIfModelChanges<DataProvider>());
            var mode = new DbModelBuilder();
            var conn = this.Database.Connection;
            this.Database.Log = s => Debug.WriteLine(s);
            this.Configuration.LazyLoadingEnabled = false;
            this.OnModelCreating(mode);


            if (!this.Users.Any())
            {
                this.Users.Add(
                    new User() {Role = RoleEnum.Manager, Vessel = "Manager", Name = "Vlad"});
                this.Users.Add(
                    new User() {Role = RoleEnum.Capitan, Vessel = "Argo", Name = "Alex"});
                this.Users.Add(
                    new User() {Role = RoleEnum.Capitan, Vessel = "Toronto", Name = "Redon"});
                this.Users.Add(
                    new User() {Role = RoleEnum.Capitan, Vessel = "Horizon", Name = "Tan"});

                this.SaveChanges();
            }

            if (!this.Codes.Any())
            {
                this.Codes.Add(
                    new Code(7110, "Victualling", 71));
                this.Codes.Add(
                    new Code(7120, "MastersRepresentation", 71));
                this.Codes.Add(
                    new Code(7130, "Slopchest", 71));
                this.Codes.Add(
                    new Code(7140, "FreshWaterForCrewNeeds", 71));
                this.Codes.Add(
                    new Code(7150, "MedicalAttendanceMedicines", 71));
                this.Codes.Add(
                    new Code(7190, "Others", 71));
                this.Codes.Add(
                    new Code(7210, "Lubricants", 72));
                this.Codes.Add(
                    new Code(7215, "Chemicals", 72));
                this.Codes.Add(
                    new Code(7220, "WeldingGases", 72));
                this.Codes.Add(
                    new Code(7225, "PipesMetall", 72));
                this.Codes.Add(
                    new Code(7230, "Refrigerants", 72));
                this.Codes.Add(
                    new Code(7235, "DeckStores", 72));
                this.Codes.Add(
                    new Code(7240, "SeastockPaints", 72));
                this.Codes.Add(
                    new Code(7245, "ChartsAndPublications", 72));
                this.Codes.Add(
                    new Code(7250, "StewardStores", 72));
                this.Codes.Add(
                    new Code(7255, "TechnicalSupply", 72));
                this.Codes.Add(
                    new Code(7260, "OtherStores", 72));
                this.Codes.Add(
                    new Code(7265, "ForwardingExpensesStores", 72));
                this.Codes.Add(
                    new Code(7290, "Others", 72));
                this.Codes.Add(
                    new Code(7310, "M", 73));
                this.Codes.Add(
                    new Code(7320, "AE+EDG", 73));
                this.Codes.Add(
                    new Code(7330, "Machinery", 73));
                this.Codes.Add(
                    new Code(7340, "DeckEquipment", 73));
                this.Codes.Add(
                    new Code(7350, "ElEquipmentAndAutomation", 73));
                this.Codes.Add(
                    new Code(7360, "RadioAndNavigationEquipment", 73));
                this.Codes.Add(
                    new Code(7370, "SpecialEquipment", 73));
                this.Codes.Add(
                    new Code(7390, "OtherSpares", 73));
                this.Codes.Add(
                    new Code(7380, "ForwardingExpensesSpares", 73));
                this.Codes.Add(
                    new Code(7410, "ME", 74));
                this.Codes.Add(
                    new Code(7420, "AE_rep", 74));
                this.Codes.Add(
                    new Code(7430, "Machinery_rep", 74));
                this.Codes.Add(
                    new Code(7440, "DeckEquipment_rep", 74));
                this.Codes.Add(
                    new Code(7450, "ElEquipmentAndAutomation_rep", 74));
                this.Codes.Add(
                    new Code(7460, "RadioAndNavigationEquipment_rep", 74));
                this.Codes.Add(
                    new Code(7470, "SpecialEquipment_rep", 74));
                this.Codes.Add(
                    new Code(7480, "SteelAndPipes", 74));
                this.Codes.Add(
                    new Code(7810, "Machinery", 78));
                this.Codes.Add(
                    new Code(7820, "DeckEquipment", 78));
                this.Codes.Add(
                    new Code(7830, "El.equipment & Automation", 78));
                this.Codes.Add(
                    new Code(7840, "RadioAndNavigationEquipment", 78)); 
                this.Codes.Add(
                    new Code(7850, "Computers e.t.c.", 78));
                this.Codes.Add(
                    new Code(7890, "Other upgrade", 78));
                this.CodeTypes.Add(
                    new CodeType() {Name = "Employer's contribution", Type = 71});
                this.CodeTypes.Add(
                    new CodeType() { Name = "Consumables stores", Type = 72 });
                this.CodeTypes.Add(
                    new CodeType() { Name = "Spare Parts", Type = 74 });
                this.CodeTypes.Add(
                    new CodeType() { Name = "Management Expences", Type = 74 });
                this.CodeTypes.Add(
                    new CodeType() { Name = "Modification Expences", Type = 78 });
                this.SaveChanges();
            }

            if (this.saveTask != null)
            {
                this.saveTask = Task.Factory.StartNew(
                    () =>
                    {
                        while (true)
                        {
                            this.SaveChanges();
                            Thread.Sleep(new TimeSpan(0, 0, 5));
                        }
                    });
            }
        }

      
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
            //modelBuilder.Entity<OrderPosition>().HasRequired(p => p.OrderTrue);
        }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }

        public DbSet<SparePartsOrder> SparePartsOrder { get; set; }

        public DbSet<SupplyOrder> SupplyOrder { get; set; }

        public DbSet<Number> Numbers { get; set; }

        public DbSet<Person> Persons { get; set; }

        public DbSet<Code> Codes { get; set; }

        public DbSet<SupplyType> SupplyTypes { get; set; }

        public DbSet<SupplyOrderPosition> SupplyOrderPosition { get; set; }

        public DbSet<SparePartsOrderPosition> SparePartsOrderPosition { get; set; }

        public DbSet<ItemCode> ItemCodes { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Status> Statuses { get; set; }

        public DbSet<StatusColor> StatusColors { get; set; }

        public DbSet<WritenOff> WritenOffs { get; set; }

        public DbSet<Currency> Currencies { get; set; }

        public DbSet<StoragePlace> StoragePlaces { get; set; }

        public DbSet<OrderFilePosition> OrderFilePositions { get; set; }

        public DbSet<CodeType> CodeTypes { get; set; }

    }


   
}
