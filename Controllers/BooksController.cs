using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using UsedBooksApp.Data;
using UsedBooksApp.Models;
using UsedBooksApp.ViewModels;

namespace UsedBooksApp.Controllers
{
    public class BooksController : Controller
    {
        private readonly UsedBooksAppContext _context;
        private readonly IHostEnvironment _hostEnvironment;

        public BooksController(UsedBooksAppContext context, IHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            return View(await _context.Book.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( BooksView book)
        {
            if (ModelState.IsValid)
            {
                string fileName = "";
                if (book.Image != null)
                {
                    fileName = Guid.NewGuid().ToString() + "_" + 
                        book.Image.FileName.Substring(book.Image.FileName.LastIndexOf('\\') + 1);

                    string uploadFolder = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot", "images");
                    string filePath = Path.Combine(uploadFolder, fileName);
                    FileStream uploadFile = new FileStream(filePath, FileMode.Create);
                    book.Image.CopyTo(uploadFile);
                }
                else
                    fileName = "no_image.png";
                Book newBook = new Book()
                {
                    Title = book.Title,
                    Author = book.Author,
                    ISBN = book.ISBN,
                    PublishedDate = book.PublishedDate,
                    Image = fileName
                };
                _context.Add(newBook);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id=newBook.Id});

            }
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["Image"] = book.Image;
            return View(new BooksView() { Id= book.Id, Title=book.Title, Author=book.Author, ISBN=book.ISBN, PublishedDate=book.PublishedDate});
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,ISBN,PublishedDate,Image")] BooksView book)
        {
            Book oldBook = await _context.Book.FindAsync(id);
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string fileName = "";
                if (book.Image != null)
                {
                    fileName = Guid.NewGuid().ToString() + "_" + book.Image.FileName.Substring(book.Image.FileName.LastIndexOf('\\') + 1);

                    string uploadFolder = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot", "images");
                    string filePath = Path.Combine(uploadFolder, fileName);
                    FileStream uploadFile = new FileStream(filePath, FileMode.Create);
                    book.Image.CopyTo(uploadFile);
                    //delete old file
                    System.IO.File.Delete(Path.Combine(uploadFolder, oldBook.Image));
                    oldBook.Image = fileName;
                }
                oldBook.Title = book.Title;
                oldBook.ISBN = book.ISBN;
                oldBook.PublishedDate = book.PublishedDate;
                
                try
                {
                    _context.Update(oldBook);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = book.Id});
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Book.FindAsync(id);
            _context.Book.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.Id == id);
        }
    }
}
