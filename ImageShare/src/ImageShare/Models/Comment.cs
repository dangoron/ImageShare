using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImageShare.Models
{
    public class Comment
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Text content.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The comment should be at max {1} characters.")]
        public string Text { get; set; }

        /// <summary>
        /// The date time when this comment is submitted.
        /// </summary>
        [Required]
        public DateTimeOffset DateTime { get; set; }


        /// <summary>
        /// The user who submitted this comment.
        /// </summary>
        [Required]
        public ApplicationUser User { get; set; }

        /// <summary>
        /// The post which this comment belongs to.
        /// </summary>
        [Required]
        public Post Post { get; set; }
    }
}
