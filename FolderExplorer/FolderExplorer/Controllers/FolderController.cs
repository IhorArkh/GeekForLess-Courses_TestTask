using System.Diagnostics;
using System.Text;
using FolderExplorer.Data;
using FolderExplorer.Entities;
using FolderExplorer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FolderExplorer.Models;
using Microsoft.EntityFrameworkCore;

namespace FolderExplorer.Controllers;

public class FolderController : Controller
{
    private readonly DataContext _context;
    private readonly IFolderDataService _folderDataService;

    public FolderController(DataContext context, IFolderDataService folderDataService)
    {
        _context = context;
        _folderDataService = folderDataService;
    }

    public IActionResult ShowFolder(int? id)
    {
        Folder currentFolder;

        if (!_context.Folders.Any())
        {
            currentFolder = new Folder { Name = "Root" };

            _context.Folders.Add(currentFolder);
            _context.SaveChanges();
        }

        if (id.HasValue)
        {
            currentFolder = _context.Folders
                .Include(f => f.ChildFolders)
                .FirstOrDefault(f => f.Id == id);
        }
        else
        {
            currentFolder = _context.Folders
                .Include(f => f.ChildFolders)
                .FirstOrDefault(f => f.ParentFolderId == null);
        }

        if (currentFolder == null)
        {
            return NotFound();
        }

        return View(currentFolder);
    }

    public IActionResult Export()
    {
        var allFolders = _context.Folders
            .Include(f => f.ChildFolders)
            .ToList();

        var json = _folderDataService.ExportToJson(allFolders);

        return File(Encoding.UTF8.GetBytes(json), "application/json", "export.json");
    }

    public IActionResult ImportFromFile()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ImportFromFile(IFormFile file)
    {
        _context.Folders.RemoveRange(_context.Folders);
        _context.SaveChanges();

        if (file != null && file.Length > 0)
        {
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var json = reader.ReadToEnd();

                _folderDataService.ImportFromJson(json);
            }
        }

        return RedirectToAction("ShowFolder");
    }

    public IActionResult ImportFromSystem()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ImportFromSystem(string directoryPath)
    {
        try
        {
            _context.Folders.RemoveRange(_context.Folders);
            _context.SaveChanges();

            if (!Directory.Exists(directoryPath))
            {
                return BadRequest("Wrong path!");
            }

            var rootFolder = _folderDataService.ImportFolderFromSystem(directoryPath, null);

            _context.SaveChanges();

            return RedirectToAction("ShowFolder", new { id = rootFolder.Id });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error during import: {ex.Message}");
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}