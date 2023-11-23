using FolderExplorer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FolderExplorer.Data.Configurations;

public class FolderConfiguration : IEntityTypeConfiguration<Folder>
{
    public void Configure(EntityTypeBuilder<Folder> builder)
    {
        builder.HasData(
            new Folder { Id = 1, Name = "Creating Digital Images" },
            new Folder { Id = 2, Name = "Resources", ParentFolderId = 1 },
            new Folder { Id = 3, Name = "Evidence", ParentFolderId = 1 },
            new Folder { Id = 4, Name = "Graphic Products", ParentFolderId = 1 },
            new Folder { Id = 5, Name = "Primary Sources", ParentFolderId = 2 },
            new Folder { Id = 6, Name = "Secondary Sources", ParentFolderId = 2 },
            new Folder { Id = 7, Name = "Process", ParentFolderId = 4 },
            new Folder { Id = 8, Name = "Final Product", ParentFolderId = 4 }
        );
    }
}