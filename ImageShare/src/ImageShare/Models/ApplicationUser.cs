using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ImageShare.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Url of avatar image.
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// Date time when this account was signed up.
        /// </summary>
        public DateTimeOffset SignUpDate { get; set; }

        /// <summary>
        /// Other users whom you are following.
        /// </summary>
        public virtual ICollection<ApplicationUser> FollowedUsers { get; set; }

        /// <summary>
        /// Posts upload by this user.
        /// </summary>
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

        /// <summary>
        /// Comments submitted by this user.
        /// </summary>
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
