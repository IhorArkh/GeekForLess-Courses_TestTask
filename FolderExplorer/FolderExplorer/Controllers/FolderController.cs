using System.Diagnostics;
using FolderExplorer.Data;
using FolderExplorer.Entities;
using Microsoft.AspNetCore.Mvc;
using FolderExplorer.Models;
using Microsoft.EntityFrameworkCore;

namespace FolderExplorer.Controllers;

public class FolderController : Controller
{
    private readonly DataContext _context;

    public FolderController(DataContext context)
    {
        _context = context;
    }

    public IActionResult Index(int? id)
    {
        Folder currentFolder;

        if (id.HasValue)
        {
            currentFolder = _context.Folders.Include(f => f.ChildFolders)
                .FirstOrDefault(f => f.Id == id);
        }
        else
        {
            currentFolder = _context.Folders.Include(f => f.ChildFolders)
                .FirstOrDefault(f => f.ParentFolderId == null);
        }
        
        if (currentFolder == null)
        {
            return NotFound();
        }

        return View(currentFolder);
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