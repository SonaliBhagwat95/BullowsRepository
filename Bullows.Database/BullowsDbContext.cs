using System;
using System.Collections.Generic;
using System.Text;
using bullows.database;
using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;

namespace Bullows.Database
{
   public class BullowsDbContext:DbContext
    {
        public BullowsDbContext(DbContextOptions<BullowsDbContext> options) : base(options)
        {
        }

        public DbSet<Users> User { get; set; }
        public DbSet<UserRoles> UserRole { get; set; }
        public DbSet<Projects> Project { get; set; }

        public DbSet<PanelInputDetails> PanelInputDetail { get; set; }

        public DbSet<PanelCutout> PanelCutouts { get; set; }
        public DbSet<CustomerMaster> CustomerMasters { get; set; }
        public DbSet<ComponentTable> ComponentTables { get; set; }
        public DbSet<EnquiryMaster> EnquiryMasters { get; set; }
        public DbSet<tblAddContactPerson> tblAddContactPersons { get; set; }
        public DbSet<tblState> tblState { get; set; }
       // public DbSet<tblDistrict> tblDistricts { get; set; }
        public DbSet<tblCity> tblCity { get; set; }
        public DbSet<PaintBooth> PaintBooths { get; set; }
        public DbSet<PaintBoothDetails>PaintBoothDetails { get; set; }
        public DbSet<tblMOC> tblMOC { get; set;}
        public DbSet<PanelDetails> PanelDetails { get; set; }
        public DbSet<BendSectionTable> BendSectionTable { get; set; }
        public DbSet<SettingDetails> SettingDetails { get; set; }
        public DbSet<tblMotorFlange> tblMotorFlange { get; set; }
        public DbSet<MotorDetails> MotorDetails { get; set; }
        public DbSet<TubeLightDetails> TubeLightDetails { get; set; }
        public DbSet<FilterFrameDetails> FilterFrameDetails { get; set; }
        public DbSet<PressureDrop> PressureDrop { get; set; }
        public DbSet<ExhaustDuctings> ExhaustDuctings { get; set; }
        public DbSet<MetalBaffleDetails> MetalBaffleDetails { get; set; }

        public DbSet<PriceDetailsTable> PriceDetailsTable { get; set; }
        public DbSet<ExceptionHandler> ExceptionHandler { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BendSectionTable>()
                .HasKey(b => b.BendId);
            modelBuilder.Entity<tblMotorFlange>().HasKey(b => b.MotorCatalogID);
            modelBuilder.Entity<MotorDetails>().HasKey(b => b.MotorID);
            modelBuilder.Entity<FilterFrameDetails>().HasKey(b => b.FID);
            modelBuilder.Entity<TubeLightDetails>().HasKey(b => b.TubeLightID);
            modelBuilder.Entity<PressureDrop>().HasKey(b => b.ItemNumber);
            modelBuilder.Entity<tblCity>().HasKey(b => b.CityID);
            modelBuilder.Entity<ExhaustDuctings>().HasKey(b => b.DuctId);


        }
    }

}
