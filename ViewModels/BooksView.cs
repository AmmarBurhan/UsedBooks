using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UsedBooksApp.ViewModels
{
    public class BooksView
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        public string ISBN { get; set; }
        [DataType(DataType.Date)]
        public DateTime PublishedDate { get; set; }
        public IFormFile Image { get; set; }
    }
}
