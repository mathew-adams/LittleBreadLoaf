using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using littlebreadloaf.Data;

namespace littlebreadloaf.ViewComponents
{
    public class DisplayUserProfile
    {
        public UserProfile UserProfile { get; set; }
        public bool ShowInstagram { get; set; }
        public bool ShowLastName { get; set; }
        public bool HasProfile { get; set; }
    }

    public class UserViewComponent : ViewComponent
    {
        private readonly ProductContext _context;
        public UserViewComponent(ProductContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userID, string email, bool showInstagram, bool showLastName, bool showEmail)
        {
            Guid.TryParse(userID, out Guid parsedID);
            var userProfile = await _context.UserProfile.FirstOrDefaultAsync(f => f.UserID == parsedID);
            var displayUserProfile = new DisplayUserProfile()
            {
                UserProfile = userProfile,
                ShowInstagram = showInstagram,
                ShowLastName = showLastName,
                HasProfile = userProfile != null
            };
            return View(displayUserProfile);
        }

    }
}