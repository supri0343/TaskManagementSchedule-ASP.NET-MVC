using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestAplication.Models
{
    public class Schedule
    {
        [Key]
        public int Id { get; set; }
        public string Detail { get; set; }
        public int Priority { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsDone { get; set; }
    }
}
