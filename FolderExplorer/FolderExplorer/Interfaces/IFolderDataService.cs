using FolderExplorer.Entities;

namespace FolderExplorer.Interfaces;

public interface IFolderDataService
{
    string ExportToJson(List<Folder> allFolders);

    void ImportFromJson(string json);

    Folder ImportFolderFromSystem(string folderPath, Folder parentFolder);
}