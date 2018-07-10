using System.IO;
using AutoMapper;
using Demo2.Data.Entities;
using Microsoft.AspNetCore.Hosting;

namespace Demo2.Web.Models
{
    public class PhotoMapingAction : IMappingAction<Photo, PhotoModel>
    {
        private readonly string picturesPath;

        public PhotoMapingAction(IHostingEnvironment hostingEnvironment)
        {
            picturesPath = Path.Combine(hostingEnvironment.WebRootPath, "pictures");
        }

        public void Process(Photo source, PhotoModel destination)
        {
            var files = Directory.GetFiles(picturesPath, source.Id + ".*");

            if (files.Length > 0)
                destination.ImageUrl = "pictures/" + Path.GetFileName(files[0]);
        }
    }
}