using System.IO;
using AutoMapper;
using Demo1.Data.Entity;
using Microsoft.AspNetCore.Hosting;

namespace Demo1.Models
{
    public class TodoMapingAction : IMappingAction<TodoEntity, Todo>
    {
        private readonly string picturesPath;

        public TodoMapingAction(IHostingEnvironment hostingEnvironment)
        {
            picturesPath = Path.Combine(hostingEnvironment.WebRootPath, "pictures");
        }

        public void Process(TodoEntity source, Todo destination)
        {
            var files = Directory.GetFiles(picturesPath, source.Id + ".*");

            if (files.Length > 0)
                destination.ImageUrl = "pictures/" + Path.GetFileName(files[0]);
        }
    }
}