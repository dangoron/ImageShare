using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ImageShare.Models
{
    public class Image
    {
        /// <summary>
        /// Primary key for database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Image file name.
        /// </summary>
        [Required]
        public string FileName { get; set; }
    }
}
