using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mantis.Domain.Carriers.Models;

namespace Mantis.Shared.DataAccess
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options) { }

        public virtual DbSet<Carrier> Carriers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Data Source=SQL5111.site4now.net;Initial Catalog=db_aaa56a_folient;User Id=db_aaa56a_folient_admin;Password=***REMOVED***");
            }
        }
    }
}
