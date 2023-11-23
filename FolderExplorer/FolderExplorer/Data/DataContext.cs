using FolderExplorer.Data.Configurations;
using FolderExplorer.Entities;
using Microsoft.EntityFrameworkCore;

namespace FolderExplorer.Data;

public class DataContext : DbContext
{
    public DbSet<Folder> Folders { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new FolderConfiguration());
    }
    
}