using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ghumfir.Models;
using Microsoft.AspNetCore.Authorization;

namespace Ghumfir.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PackagesController : Controller
    {
        private readonly ghumfirContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public PackagesController(ghumfirContext context,IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Packages
        public async Task<IActionResult> Index()
        {
            var ghumfirContext = _context.Packages.Include(p => p.Category);
            return View(await ghumfirContext.ToListAsync());
        }

        // GET: Packages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Packages == null)
            {
                return NotFound();
            }

            var package = await _context.Packages
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (package == null)
            {
                return NotFound();
            }

            return View(package);
        }

        // GET: Packages/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }
        public string UploadedFile(Package packages)
        {
            string? uniqueFileName = null;
            if (packages.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "Uploads");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + packages.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    packages.ImageFile.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        // POST: Packages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Image,ImageFile,Price,CategoryId")] Package packages)
        {
            /*if (ModelState.IsValid)
            {*/
                string uniqueFileName = UploadedFile(packages);
                packages.Image = uniqueFileName;
                _context.Attach(packages);
                _context.Entry(packages).State = EntityState.Added;
                await _context.SaveChangesAsync();
                TempData["Message"] = "New package created successfully!";
                return RedirectToAction(nameof(Index));
            /*}
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", packages.CategoryId);
            return View(packages);*/
        }


        // GET: Packages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Packages == null)
            {
                return NotFound();
            }

            var package = await _context.Packages.FindAsync(id);
            if (package == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", package.CategoryId);
            return View(package);
        }

        // POST: Packages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Image,Price,CategoryId")] Package package)
        {
            if (id != package.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(package);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PackageExists(package.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            TempData["Message"] = "Package updated successfully!";
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", package.CategoryId);
            return View(package);
        }

        // GET: Packages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Packages == null)
            {
                return NotFound();
            }

            var package = await _context.Packages
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (package == null)
            {
                return NotFound();
            }

            return View(package);
        }

        // POST: Packages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Packages == null)
            {
                return Problem("Entity set 'ghumfirContext.Packages'  is null.");
            }
            var package = await _context.Packages.FindAsync(id);
            if (package != null)
            {
                _context.Packages.Remove(package);
            }
            TempData["Message"] = "Package deleted successfully!";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PackageExists(int id)
        {
          return _context.Packages.Any(e => e.Id == id);
        }
    }
}
