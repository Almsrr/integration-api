using IntegrationAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace IntegrationAPI.Data {
  public class IntegrationContext : DbContext {
    public IntegrationContext(DbContextOptions<IntegrationContext> options) : base(options) {
    }

    public DbSet<Transaction> Transactions { get; set; }
  }
}