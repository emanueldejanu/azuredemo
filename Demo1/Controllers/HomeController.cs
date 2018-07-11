using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using Demo1.Data;
using Demo1.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using Demo1.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace Demo1.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDatabaseContext context;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IMapper mapper;

        public HomeController(AppDatabaseContext context, IHostingEnvironment hostingEnvironment, IMapper mapper)
        {
            this.context = context;
            this.hostingEnvironment = hostingEnvironment;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            List<TodoEntity> todoList = await context.Todo.ToListAsync();
            Blob blob = new Blob();
            var blobConnection = blob.GetAllBlobs().Result;

            List<Todo> toDo = new List<Todo>();
            foreach (var image in blobConnection)
            {
                toDo.Add(new Todo
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    ModifiedAt = DateTime.Now,
                    Date = DateTime.Now,
                    Description = "Got It from the storage account",
                    ImageUrl = image.Uri.ToString()
                });
            }

            return View(toDo);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
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

        public async Task<IActionResult> Details(Guid id)
        {
            TodoEntity todoEntity = await context.Todo.SingleOrDefaultAsync(m => m.Id == id);

            if (todoEntity == null)
            {
                return NotFound();
            }

            return View(mapper.Map<Todo>(todoEntity));
        }

        // GET: Todos/Create
        public IActionResult Create()
        {
            return View(new EditTodo { Date = DateTime.UtcNow });
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EditTodo editTodo)
        {
            Blob blob = new Blob();
            string cotainerName = "alex";
            await blob.UploadBlob(cotainerName, editTodo.Image);

            if (ModelState.IsValid)
            {
                //var todoEntity = new TodoEntity
                //{
                //    Id = Guid.NewGuid(),
                //    CreatedAt = DateTime.UtcNow,
                //    ModifiedAt = DateTime.UtcNow,
                //    Description = editTodo.Description,
                //    Date = editTodo.Date
                //};

                //context.Todo.Add(todoEntity);

                //await context.SaveChangesAsync();

                //await SaveFile(editTodo.Image, todoEntity.Id);

                return RedirectToAction("Index");
            }

            return View(editTodo);
        }

        private async Task SaveFile(IFormFile file, Guid id)
        {
            if (file == null || file.Length == 0)
                return;

            var picturesPath = Path.Combine(hostingEnvironment.WebRootPath, "pictures");

            DeleteOldFiles(id, picturesPath);

            var filePath = Path.Combine(picturesPath, id + Path.GetExtension(file.FileName));

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }

        private static void DeleteOldFiles(Guid id, string picturesPath)
        {
            string[] files = Directory.GetFiles(picturesPath, id + ".*");
            foreach (string filePath in files)
            {
                System.IO.File.Delete(filePath);
            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            TodoEntity todoEntity = await context.Todo.SingleOrDefaultAsync(m => m.Id == id);
            if (todoEntity == null)
            {
                return NotFound();
            }
            return View(mapper.Map<EditTodo>(todoEntity));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditTodo editTodo)
        {
            TodoEntity todoEntity = await context.Todo.SingleOrDefaultAsync(m => m.Id == id);
            if (todoEntity == null)
            {
                return NotFound();
            }

            todoEntity.Description = editTodo.Description;
            todoEntity.Date = editTodo.Date;

            if (ModelState.IsValid)
            {
                todoEntity.ModifiedAt = DateTime.UtcNow;
                context.Update(todoEntity);
                await context.SaveChangesAsync();

                await SaveFile(editTodo.Image, todoEntity.Id);

                return RedirectToAction("Index");
            }

            return View(mapper.Map<EditTodo>(todoEntity));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            TodoEntity todoEntity = await context.Todo.SingleOrDefaultAsync(m => m.Id == id);
            if (todoEntity == null)
            {
                return NotFound();
            }

            return View(mapper.Map<Todo>(todoEntity));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            TodoEntity todo = await context.Todo.SingleOrDefaultAsync(m => m.Id == id);
            context.Todo.Remove(todo);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
