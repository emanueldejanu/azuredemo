using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Demo2.Web.Models
{
    public class EditPhotoModel
    {
        public Guid Id { get; set; }

        [MaxLength(2056)]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        
        public IFormFile Image { get; set; }
    }
}