using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace dotnet.Models
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Symbol { get; set; }
        [Required]
        public DateOnly Date { get; set; }
        [Required]
        public float CloseorLast { get; set; }
        [Required] 
        public int Volume { get; set; }
        [Required]
        public float Open { get; set; }
        [Required]
        public float High { get; set; }
        [Required]
        public float Low { get; set; }
    }
}