using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using Demo2.Data;
using Demo2.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Demo2.Web.Models;

namespace Demo2.Web.Controllers
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
            //List<Photo> photos = await context.Photos.ToListAsync();
            //return View(mapper.Map<List<PhotoModel>>(photos));

            List<Photo> todoList = await context.Photos.ToListAsync();
            Blob blob = new Blob();
            var blobConnection = blob.GetAllBlobs().Result;

            List<PhotoModel> photoModel = new List<PhotoModel>();
            foreach (var image in blobConnection)
            {
                photoModel.Add(new PhotoModel
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    ModifiedAt = DateTime.Now,
                    Date = DateTime.Now,
                    Description = "Got It from the storage account",
                    ImageUrl = image.Uri.ToString()
                });
            }

            return View(photoModel);
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
            Photo photo = await context.Photos.SingleOrDefaultAsync(m => m.Id == id);

            if (photo == null)
            {
                return NotFound();
            }

            return View(mapper.Map<PhotoModel>(photo));
        }

        // GET: Todos/Create
        public IActionResult Create()
        {
            return View(new EditPhotoModel { Date = DateTime.UtcNow });
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EditPhotoModel editPhoto)
        {
            Blob blob = new Blob();
            string cotainerName = "alex";
            await blob.UploadBlob(cotainerName, editPhoto.Image);

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

            return View(editPhoto);
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
            Photo photo = await context.Photos.SingleOrDefaultAsync(m => m.Id == id);
            if (photo == null)
            {
                return NotFound();
            }
            return View(mapper.Map<EditPhotoModel>(photo));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditPhotoModel editTodo)
        {
            Photo todoEntity = await context.Photos.SingleOrDefaultAsync(m => m.Id == id);
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

            return View(mapper.Map<EditPhotoModel>(todoEntity));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            Photo photo = await context.Photos.SingleOrDefaultAsync(m => m.Id == id);
            if (photo == null)
            {
                return NotFound();
            }

            return View(mapper.Map<PhotoModel>(photo));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            Photo photo = await context.Photos.SingleOrDefaultAsync(m => m.Id == id);
            context.Photos.Remove(photo);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
