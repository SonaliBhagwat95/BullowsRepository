using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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

        public DbSet<PanelInputDetail> PanelInputDetails { get; set; }
       
        public DbSet<PanelCutout> PanelCutouts { get; set; }
        public DbSet<CustomerMaster> CustomerMasters { get; set; }
        public DbSet<ComponentTable> ComponentTables { get; set; }
        public DbSet<EnquiryMaster> EnquiryMasters { get; set; }
        public DbSet<tblAddContactPerson> tblAddContactPersons { get; set; }
        public DbSet<tblState> tblStates { get; set; }
        public DbSet<tblDistrict> tblDistricts { get; set; }
        public DbSet<tblCity> TblCities { get; set; }
        public DbSet<PaintBooth> PaintBooths { get; set;}

    }

}
