using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace semestrovka1_oris.framework.core
{
    

    public class GameContext : DbContext
    {
        public DbSet<Role> Roles { get; set; }
        public DbSet<District> Districts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=game.db");
        }
    }
}
