using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pvm = ImageShare.Models.PostViewModels;
using Sakura.AspNetCore.Mvc;
using Sakura.AspNetCore;

namespace ImageShare.Models.HomeViewModels
{
    public class IndexViewModel
    {
        public pvm.UploadViewModel PostToUpload { get; set; }

        public IPagedList PagedList { get; set; }
    }
}
