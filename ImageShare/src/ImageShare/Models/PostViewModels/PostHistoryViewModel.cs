using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageShare.Models.PostViewModels
{
    public class PostHistoryViewModel
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public int ImageCount { get; set; }

        public int CommentCount { get; set; }

        public DateTimeOffset UploadDate { get; set; }
    }
}
