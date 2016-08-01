using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageShare.Models.PostViewModels
{
    public class CommentHistoryViewModel
    {
        /// <summary>
        /// Comment's text content.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Belonging post's id.
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        /// Date time when this comment is submitted.
        /// </summary>
        public DateTimeOffset DateTime { get; set; }
    }
}
