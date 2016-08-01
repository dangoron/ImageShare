using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ImageShare.Models
{
    public class Post
    {
        /// <summary>
        /// Primary key for database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Images contained in this post.
        /// </summary>
        [Required]
        public virtual ICollection<Image> Images { get; set; } = new List<Image>();

        /// <summary>
        /// Text content of this post.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Date time when the image was updated.
        /// </summary>
        [Required]
        public DateTimeOffset UploadDate { get; set; }

        /// <summary>
        /// User who updated this image.
        /// </summary>
        [Required(ErrorMessage = "You have to login first.")]
        public ApplicationUser User { get; set; }

        /// <summary>
        /// Comments on this post.
        /// </summary>
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
