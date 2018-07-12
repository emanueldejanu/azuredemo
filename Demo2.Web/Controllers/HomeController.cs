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
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Demo2.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDatabaseContext context;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ILogger<HomeController> logger;
        private readonly IMapper mapper;

        public HomeController(
                AppDatabaseContext context,
                IHostingEnvironment hostingEnvironment,
                ILogger<HomeController> logger,
                IMapper mapper
            )
        {
            this.context = context;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            List<Photo> photos = await context.Photos.ToListAsync();
            logger.LogWarning("Loadded {Count} photos", photos.Count);
            return View(mapper.Map<List<PhotoModel>>(photos));
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

        [Authorize]
        public IActionResult Create()
        {
            return View(new EditPhotoModel { Date = DateTime.UtcNow });
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EditPhotoModel editPhoto)
        {
            if (ModelState.IsValid)
            {
                var photo = new Photo
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                    Description = editPhoto.Description,
                    Date = editPhoto.Date
                };

                context.Photos.Add(photo);

                await context.SaveChangesAsync();

                await SaveFile(editPhoto.Image, photo.Id);

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

        [Authorize]
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
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditPhotoModel editPhoto)
        {
            Photo photo = await context.Photos.SingleOrDefaultAsync(m => m.Id == id);
            if (photo == null)
            {
                return NotFound();
            }

            photo.Description = editPhoto.Description;
            photo.Date = editPhoto.Date;

            if (ModelState.IsValid)
            {
                photo.ModifiedAt = DateTime.UtcNow;
                context.Update(photo);
                await context.SaveChangesAsync();

                await SaveFile(editPhoto.Image, photo.Id);

                return RedirectToAction("Index");
            }

            return View(mapper.Map<EditPhotoModel>(photo));
        }

        [Authorize]
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
        [Authorize]
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
