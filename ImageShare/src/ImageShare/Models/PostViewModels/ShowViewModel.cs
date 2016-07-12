using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageShare.Models.PostViewModels
{
    public class ShowViewModel
    {
        /// <summary>
        /// User name of who upload this post.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Id of this post.
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        /// Post text content.
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// Images' urls in this post.
        /// </summary>
        public List<string> ImageUrls { get; set; }

        /// <summary>
        /// Comments on this post.
        /// </summary>
        public List<CommentViewModel> Comments { get; set; }
    }
}
