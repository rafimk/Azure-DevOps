namespace Azure_DevOps.Api.Data;

using Microsoft.EntityFrameworkCore;
using Azure_DevOps.Api.Models;

public class DataContext : DbContext
{
    protected readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // connect to sqlite database
        options.UseSqlite(Configuration.GetConnectionString("WebApiDatabase"));
    }

    public DbSet<WorkItem> WorkItems { get; set; }
}