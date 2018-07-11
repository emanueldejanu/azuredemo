using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Demo1.Models
{
    public class EditTodo
    {
        public Guid Id { get; set; }

        [MaxLength(2056)]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        
        public IFormFile Image { get; set; }
    }
}