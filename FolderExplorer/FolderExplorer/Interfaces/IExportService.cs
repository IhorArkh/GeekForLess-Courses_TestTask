using FolderExplorer.Entities;

namespace FolderExplorer.Interfaces;

public interface IExportService
{
    string ExportToJson(List<Folder> allFolders);

    void ImportFromJson(string json);
}