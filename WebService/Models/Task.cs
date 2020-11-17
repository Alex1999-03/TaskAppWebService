using System;
using System.Collections.Generic;

namespace WebService.Models
{
    public partial class Task
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public bool Completed { get; set; }
    }
}
