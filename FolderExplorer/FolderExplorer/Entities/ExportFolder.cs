namespace FolderExplorer.Models;

public class ExportFolder
{
    public string Name { get; set; }
    public List<ExportFolder> ChildFolders { get; set; } = new List<ExportFolder>();
}