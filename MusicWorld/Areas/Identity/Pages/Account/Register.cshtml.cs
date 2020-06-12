using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using MusicWorld.Data;
using MusicWorld.Data.Entity;

namespace MusicWorld.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<MyUser> _signInManager;
        private readonly UserManager<MyUser> _userManager;
        private readonly MusicWorldDbContext _dbContext;

        public RegisterModel(
            UserManager<MyUser> userManager,
            SignInManager<MyUser> signInManager,
            MusicWorldDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            public string Username { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [RegularExpression(@"^[a-zA-Zа-яА-Я]+$", ErrorMessage = "Use chacracters only from the Cyrillic and the Latin ahlpabet, please!")]
            public string FirstName { get; set; }

            [Required]
            [RegularExpression(@"^[a-zA-Zа-яА-Я]+$", ErrorMessage = "Use chacracters only from the Cyrillic and the Latin ahlpabet, please!")]
            public string LastName { get; set; }

        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {

            if (!_dbContext.Roles.Any(x => x.Name == "Admin"))
            {
                _dbContext.Roles.Add(new IdentityRole()
                {
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                });
            }

            if (!_dbContext.Roles.Any(x => x.Name == "User"))
            {
                _dbContext.Roles.Add(new IdentityRole()
                {
                    Name = "User",
                    NormalizedName = "User".ToUpper()
                });
            }

            if (!_dbContext.Roles.Any(x => x.Name == "Admin") || !_dbContext.Roles.Any(x => x.Name == "User"))
            {
                _dbContext.SaveChanges();
            }

            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {

                var user = new MyUser { 
                    Id = Guid.NewGuid().ToString(), 
                    UserName = Input.Username, 
                    FirstName = Input.FirstName, 
                    LastName = Input.LastName 
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                
                if (result.Succeeded)
                {

                    // The user gets their role (by default: only the very first one is admin)
                    if (this._dbContext.Users.Count() == 1)
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                    }

                    // The user automatically logs in after successful registration
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return Redirect("/");

                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
