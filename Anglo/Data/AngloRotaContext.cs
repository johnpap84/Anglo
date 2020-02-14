using AngloRota.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AngloRota.Data
{
    public class AngloRotaContext : IdentityDbContext<User>
    {
        public AngloRotaContext(DbContextOptions<AngloRotaContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<JobTitle> JobTitles { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<RotaData> RotaData { get; set; }

    }
}
