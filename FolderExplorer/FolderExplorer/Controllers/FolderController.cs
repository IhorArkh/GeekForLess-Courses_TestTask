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
    private readonly IExportService _exportService;

    public FolderController(DataContext context, IExportService exportService)
    {
        _context = context;
        _exportService = exportService;
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

        var json = _exportService.ExportToJson(allFolders);

        return File(Encoding.UTF8.GetBytes(json), "application/json", "export.json");
    }

    public IActionResult Import()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Import(IFormFile file)
    {
        _context.Folders.RemoveRange(_context.Folders);
        _context.SaveChanges();

        if (file != null && file.Length > 0)
        {
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var json = reader.ReadToEnd();

                _exportService.ImportFromJson(json);
            }
        }

        return RedirectToAction("ShowFolder");
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