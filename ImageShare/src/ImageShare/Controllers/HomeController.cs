using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using ImageShare.Data;
using ImageShare.Models;
using pvm = ImageShare.Models.PostViewModels;
using hvm = ImageShare.Models.HomeViewModels;
using Microsoft.AspNetCore.Http;
using Sakura.AspNetCore;

namespace ImageShare.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hosting;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment hosting)
        {
            _context = context;
            _userManager = userManager;
            _hosting = hosting;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            var currentUser = await GetCurrentUser();
            if (currentUser == null)
                return RedirectToAction("Login", "Account");

            var model = new hvm.IndexViewModel()
            {
                PagedList = await GetPostPagedList(currentUser.Id, page),
                PostToUpload = new pvm.UploadViewModel()
            };
            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(pvm.UploadViewModel model)
        {
            var currentUser = await GetCurrentUser();
            if (!ModelState.IsValid)
            {
                return View("Index", new hvm.IndexViewModel
                {
                    PostToUpload = model,
                    PagedList = await GetPostPagedList(currentUser.Id)
                });
            }
            if (model.ImageFiles.Length > 9)
            {
                ModelState.AddModelError("ImageFiles", "You can upload at most 9 images once.");
                return View("Index", new hvm.IndexViewModel
                {
                    PostToUpload = model,
                    PagedList = await GetPostPagedList(currentUser.Id)
                });
            }
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
            var newPost = new Post
            {
                Images = images,
                Text = model.Text,
                User = currentUser,
                UploadDate = DateTimeOffset.Now
            };

            currentUser.Posts.Add(newPost);
            await _context.SaveChangesAsync();
            return await Index();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "ImageShare Ver 1.0.0";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        private async Task<IPagedList> GetPostPagedList(string currentUserId, int page = 1)
        {

            var currentUser = await (from u in _context.Users.Include(_ => _.FollowedUsers)
                                     where u.Id == currentUserId
                                     select u).FirstOrDefaultAsync();
            var userIds = new List<string> { currentUser.Id };
            userIds.AddRange((from u in currentUser.FollowedUsers select u.Id));
            userIds = userIds.Distinct().ToList();  // just a check, maybe not necessary.

            var pageSize = 10;
            var posts = (from post in _context.Posts.Include(_ => _.Images).Include(__ => __.Comments)
                         where userIds.Contains(post.User.Id)
                         orderby post.Id descending
                         select new pvm.ShowViewModel()
                         {

                             PostId = post.Id,
                             UserName = post.User.UserName,
                             ImageUrls = (from image in post.Images
                                          orderby image.Id ascending
                                          select Url.Content(string.Format("~/shareimages/{0}", image.FileName))).ToList(),
                             Text = post.Text,
                             Comments = (from comment in post.Comments
                                         orderby comment.DateTime ascending
                                         select new pvm.CommentViewModel
                                         {
                                             DateTime = comment.DateTime,
                                             Text = comment.Text,
                                             UserName = comment.User.UserName
                                         }).ToList()
                         });
            return posts.ToPagedList(pageSize, page);
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }
    }
}
