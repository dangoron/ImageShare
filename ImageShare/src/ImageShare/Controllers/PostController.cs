using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using ImageShare.Data;
using ImageShare.Models;
using ImageShare.Models.PostViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ImageShare.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hosting;

        public PostController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment hosting)
        {
            _context = context;
            _userManager = userManager;
            _hosting = hosting;
        }

        public IActionResult Index()
        {
            var model = new IndexViewModel();
            return View(model);
        }

        [HttpGet]
        [Route("~/Post/{id}")]
        public async Task<IActionResult> Index(int id)
        {
            var model = await (from post in _context.Posts.Include(_ => _.Images).Include(__ => __.Comments)
                               where post.Id == id
                               select new IndexViewModel()
                               {
                                   PostShowModel = new ShowViewModel()
                                   {
                                       PostId = id,
                                       UserName = post.User.UserName,
                                       ImageUrls = (from image in post.Images
                                                    orderby image.Id ascending
                                                    select Url.Content(string.Format("~/shareimages/{0}", image.FileName))).ToList(),
                                       Text = post.Text,
                                       Comments = (from comment in post.Comments
                                                   orderby comment.DateTime ascending
                                                   select new CommentViewModel
                                                   {
                                                       DateTime = comment.DateTime,
                                                       Text = comment.Text,
                                                       UserName = comment.User.UserName
                                                   }).ToList()
                                   },
                               }).FirstOrDefaultAsync() ?? new IndexViewModel();

            return View("Index", model);
        }

        [HttpGet]
        [Route("~/Post/Upload")]
        public IActionResult Upload()
        {
            return View("Upload");
        }

        [HttpPost]
        [Route("~/Post/Upload")]
        public async Task<IActionResult> Upload(UploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Upload", model);
            }
            if (model.ImageFiles.Length > 9)
            {
                ModelState.AddModelError("ImageFiles", "You can upload at most 9 images once.");
                return View("Upload", model);
            }
            var currentUser = await GetCurrentUser();
            var images = new List<Image>();
            foreach (var file in model.ImageFiles)
            {
                var directory = Path.Combine(_hosting.WebRootPath, "shareimages");
                string fileName;
                string path;

                do
                {
                    fileName = Path.ChangeExtension(Path.GetRandomFileName(), Path.GetExtension(file.FileName));
                    path = Path.Combine(directory, fileName);
                } while (System.IO.File.Exists(path));

                using (var stream = System.IO.File.Create(path))
                {
                    await file.CopyToAsync(stream);
                    var image = new Image { FileName = fileName };
                    images.Add(image);
                    _context.Images.Add(image);
                }
            }
            var newPost = new Post { Images = images, Text = model.Text, User = currentUser, UploadDate = DateTimeOffset.Now };
            _context.Posts.Add(newPost);
            await _context.SaveChangesAsync();
            return View("Upload");
        }

        [Route("~/Post/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUser = await GetCurrentUser();
            var post = await (from p in _context.Posts.Include(_ => _.Images).Include(__=>__.Comments)
                              where p.Id == id select p).FirstOrDefaultAsync();
            try
            {
                if (post.User == currentUser)
                {
                    var directory = Path.Combine(_hosting.WebRootPath, "shareimages");
                    foreach (var fileName in from i in post.Images select i.FileName)
                    {
                        var path = Path.Combine(directory, fileName);
                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                    }
                    _context.Comments.RemoveRange(post.Comments);
                    _context.Images.RemoveRange(post.Images);
                    _context.Posts.Remove(post);
                }
                await _context.SaveChangesAsync();
            }
            catch
            {
                // ignored
            }
            //TempData["Message"] = "Post has been deleted.";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Route("~/Post/Commet/{id}")]
        public async Task<IActionResult> Comment(IndexViewModel model, int id)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }
            var currentUser = await GetCurrentUser();
            var comment = new Comment
            {
                DateTime = DateTimeOffset.Now,
                Text = model.Comment,
                User = currentUser
            };

            var post = await (from p in _context.Posts where p.Id == id select p).FirstOrDefaultAsync();
            if (post != null)
            {
                _context.Comments.Add(comment);
                if (post.Comments == null)
                {
                    post.Comments = new List<Comment>();
                }
                post.Comments.Add(comment);
                await _context.SaveChangesAsync();
            }

            return await Index(id);
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }
    }
}
