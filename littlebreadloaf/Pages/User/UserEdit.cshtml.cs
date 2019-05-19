using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using littlebreadloaf.Data;
using System.Drawing;

namespace littlebreadloaf.Pages.User
{
    [Authorize]
    public class UserEditModel : PageModel
    {
        private readonly ProductContext _context;
        public UserEditModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public IFormFile FileUpload { get; set; }

        [BindProperty]
        public UserProfile UserProfile { get; set; }

        public async Task<IActionResult> OnGetAsync(string userID)
        {
            if (String.IsNullOrEmpty(userID) || !Guid.TryParse(userID, out Guid parsedID))
            {
                return new RedirectToPageResult("/User/UserView");
            }

            UserProfile = await _context.UserProfile.FirstOrDefaultAsync(f => f.UserID == parsedID);

            if (UserProfile == null)
            {
                UserProfile = new UserProfile()
                {
                    UserID = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value),
                    FirstName = "",
                    LastName = ""
                };
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //ModelState.ErrorCount
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if(FileUpload != null)
            {
                var sourceID = UserProfile.UserID.ToString();
                var imgHelper = new ImageHelper(sourceID);

                var sizes = new List<Size>
                {
                    new Size(50, 50),
                    new Size(100, 100),
                    new Size(200, 200)
                };

                try
                {
                    imgHelper.AddImages(ImageHelper.ImageResizeMode.Square,
                                        sourceID,
                                        sizes.ToArray(),
                                        FileUpload);
                }
                catch  (Exception ex)
                {
                    var systemError = new SystemError
                    {
                        ErrorID = Guid.NewGuid(),
                        RequestID = sourceID,
                        Path = "UserEditModel",
                        Error = ex.ToString(),
                        Occurred = DateTime.Now
                    };
                    _context.SystemError.Add(systemError);
                    await _context.SaveChangesAsync();

                    ModelState.AddModelError("BadFile", "The file is invalid. It must be an image");
                    return Page();
                }

                UserProfile.ProfileImageLocation = imgHelper.GetDisplayFileName(sourceID);
            }

            if (UserProfile.InstagramDisplay == null)
                UserProfile.InstagramDisplay = "";
            if (UserProfile.InstagramURL == null)
                UserProfile.InstagramURL = "";
            if (! await _context.UserProfile.AnyAsync(a => a.UserID == UserProfile.UserID))
            {
                //Add

                if (UserProfile.InstagramDisplay == null)
                    UserProfile.InstagramDisplay = "";
                if (UserProfile.InstagramURL == null)
                    UserProfile.InstagramURL = "";
                _context.UserProfile.Add(UserProfile);
            }
            else
            {
                //Update
                _context.UserProfile.Update(UserProfile);
            }

            await _context.SaveChangesAsync();



            return new RedirectToPageResult("/User/UserView", new { UserProfile.UserID });
        }


    }
}