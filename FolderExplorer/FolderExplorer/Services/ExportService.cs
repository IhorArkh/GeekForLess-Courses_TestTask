using System.Text.Json;
using FolderExplorer.Data;
using FolderExplorer.Entities;
using FolderExplorer.Interfaces;
using FolderExplorer.Models;

namespace FolderExplorer.Services;

public class ExportService : IExportService
{
    private readonly DataContext _context;

    public ExportService(DataContext context)
    {
        _context = context;
    }

    public string ExportToJson(List<Folder> allFolders)
    {
        var rootFolders = allFolders
            .Where(f => f.ParentFolderId == null)
            .ToList();

        var exportFolders = rootFolders
            .Select(folder => ConvertToExportFolder(folder, allFolders))
            .ToList();

        var json = JsonSerializer.Serialize(exportFolders, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        System.IO.File.WriteAllText("export.json", json);

        return json;
    }

    public void ImportFromJson(string json)
    {
        var importFolders = JsonSerializer.Deserialize<List<ExportFolder>>(json);

        foreach (var importFolder in importFolders)
        {
            ImportFolder(importFolder, null);
        }

        _context.SaveChanges();
    }

    private void ImportFolder(ExportFolder importFolder, Folder parentFolder)
    {
        var newFolder = new Folder
        {
            Name = importFolder.Name,
            ParentFolder = parentFolder
        };

        _context.Folders.Add(newFolder);

        foreach (var childImportFolder in importFolder.ChildFolders)
        {
            ImportFolder(childImportFolder, newFolder);
        }
    }

    private ExportFolder ConvertToExportFolder(Folder folder, List<Folder> allFolders)
    {
        return new ExportFolder
        {
            Name = folder.Name,
            ChildFolders = folder.ChildFolders
                .Select(childFolder => ConvertToExportFolder(childFolder, allFolders))
                .ToList()
        };
    }
}