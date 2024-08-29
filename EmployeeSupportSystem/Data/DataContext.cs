using EmployeeSupportSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSupportSystem.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
