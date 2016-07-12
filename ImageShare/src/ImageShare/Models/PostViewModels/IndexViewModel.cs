using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageShare.Models.PostViewModels
{
    public class IndexViewModel
    {
        /// <summary>
        /// Post show model for HttpGet
        /// </summary>
        public ShowViewModel PostShowModel { get; set; }

        /// <summary>
        /// Comment property for HttpPost
        /// </summary>
        public string Comment { get; set; }
    }
}
