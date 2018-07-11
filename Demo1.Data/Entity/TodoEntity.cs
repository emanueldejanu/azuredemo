using System;
using System.ComponentModel.DataAnnotations;

namespace Demo1.Data.Entity
{
    public class TodoEntity
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }
    }
}
