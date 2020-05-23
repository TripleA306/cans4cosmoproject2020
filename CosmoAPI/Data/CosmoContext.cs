using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CosmoAPI.Models;


namespace CosmoAPI.Models
{
    public class CosmoContext : DbContext
    {
        public CosmoContext (DbContextOptions<CosmoContext> options)
            : base(options)
        {
        }

        //Configures options when Models are create d in the context
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                //Restricts Deletion when Foreign Key property is present
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<CosmoAPI.Models.Location> Location { get; set; }
        public DbSet<CosmoAPI.Models.Subscriber> Subscriber { get; set; }
        //Sets a Region DbSet property for the Context
        
        public DbSet<CosmoAPI.Models.Region> Region { get; set; }
        //Sets a Route DbSet property for the Context
        public DbSet<CosmoAPI.Models.Route> Route { get; set; }

        public DbSet<CosmoAPI.Models.Admin> Admin { get; set; }


        public DbSet<CosmoAPI.Models.LocationRoute> LocationRoute { get; set; }
    }
}
    
