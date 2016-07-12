using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ImageShare.Data;
using ImageShare.Models;
using ImageShare.Models.PostViewModels;
using Sakura.AspNetCore;

namespace ImageShare.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("~/User/{name?}")]
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            var currentUser = await GetCurrentUser();
            var currentUserId = currentUser.Id;
            name = name ?? currentUser.UserName;
            var normalizedName = name.ToUpper();
            var user = await (from u in _context.Users where u.NormalizedUserName == normalizedName select u).FirstOrDefaultAsync();
            var userId = user.Id;
            if (currentUser.Id == user.Id)
            {
                ViewData["IsSelf"] = true;
            }
            else
            {
                ViewData["IsSelf"] = false;
                var followedUsers = await (from u in _context.Users.Include(_ => _.FollowedUsers)
                                           where u.Id == currentUserId
                                           select u.FollowedUsers).FirstOrDefaultAsync();
                ViewData["Followed"] = (from u in followedUsers
                                        where u.NormalizedUserName == normalizedName
                                        select true).FirstOrDefault();
            }
            ViewData["UserName"] = user.UserName;
            ViewData["UserEmail"] = user.Email;
            ViewData["AvatarUrl"] = user.AvatarUrl;

            var pageSize = 10;
            var posts = (from post in _context.Posts.Include(p => p.Images)
                         where post.User.Id == userId
                         orderby post.Id descending
                         select new ShowViewModel
                         {
                             PostId = post.Id,
                             UserName = post.User.UserName,
                             ImageUrls = (from image in post.Images
                                          orderby image.Id ascending
                                          select Url.Content(string.Format("~/shareimages/{0}", image.FileName))).ToList(),
                             Text = post.Text
                         });

            var pageData = posts.ToPagedList(pageSize, page);
            return View(pageData);
        }

        [Route("~/User/Follow/{name}")]
        public async Task<IActionResult> Follow(string name)
        {
            if (name == null)
            {
                return RedirectToAction(nameof(UserController.Index), "User");
            }
            var normalizedName = name.ToUpper();
            var currentUser = await GetCurrentUser();
            var currentUserId = currentUser.Id;
            var userToFollow = await (from u in _context.Users
                                      where u.NormalizedUserName == normalizedName
                                      select u).FirstOrDefaultAsync();
            currentUser = await (from u in _context.Users.Include(_ => _.FollowedUsers)
                                 where u.Id == currentUserId
                                 select u).FirstOrDefaultAsync();
            if (!(from u in currentUser.FollowedUsers where u.NormalizedUserName == normalizedName select u).Any())
            {
                currentUser.FollowedUsers.Add(userToFollow);
            }
            ViewData["Followed"] = true;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(UserController.Index), "User");
        }

        [Route("~/User/Unfollow/{name}")]
        public async Task<IActionResult> Unfollow(string name)
        {
            if (name == null)
            {
                return RedirectToAction(nameof(UserController.Index), "User");
            }
            var normalizedName = name.ToUpper();
            var currentUser = await GetCurrentUser();
            var currentUserId = currentUser.Id;
            var userToUnfollow = await (from u in _context.Users
                                        where u.NormalizedUserName == normalizedName
                                        select u).FirstOrDefaultAsync();
            currentUser = await (from u in _context.Users.Include(_ => _.FollowedUsers)
                                 where u.Id == currentUserId
                                 select u).FirstOrDefaultAsync();
            if ((from u in currentUser.FollowedUsers where u.NormalizedUserName == normalizedName select u).Any())
            {
                currentUser.FollowedUsers.Remove(userToUnfollow);
            }

            ViewData["Followed"] = false;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(UserController.Index), "User");
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }
    }
}
