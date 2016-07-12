using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageShare.Models.PostViewModels
{
    public class CommentViewModel
    {
        /// <summary>
        /// Username of who submitted this comment.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Time of when this comment is submitted.
        /// </summary>
        public DateTimeOffset DateTime { get; set; }

        /// <summary>
        /// Text content.
        /// </summary>
        public string Text { get; set; }
    }
}
