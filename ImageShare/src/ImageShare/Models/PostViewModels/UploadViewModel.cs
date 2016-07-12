using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ImageShare.Models.PostViewModels
{
    public class UploadViewModel
    {
        [Required]
        public IFormFile[] ImageFiles { get; set; }

        [StringLength(25, ErrorMessage = "The {0} must be at max {1} characters long.")]
        public string Text { get; set; }
    }
}
