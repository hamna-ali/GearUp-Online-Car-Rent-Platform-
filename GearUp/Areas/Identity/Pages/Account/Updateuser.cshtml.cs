using GearUp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GearUp.Areas.Identity.Pages.Account
{
    [Authorize]
    public class UpdateModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UpdateModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public class InputModel
        {
            [Display(Name = "Full Name")]
            public string? FullName { get; set; }

            public string? Address { get; set; }

            public string? Phone { get; set; }

            // Password change fields
            [DataType(DataType.Password)]
            [Display(Name = "Current Password")]
            public string? CurrentPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "New Password")]
            public string? NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm New Password")]
            [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
            public string? ConfirmPassword { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Forbid(); // User not found or not authenticated
            }

            Input = new InputModel
            {
                FullName = user.FullName,
                Address = user.Address,
                Phone = user.Phone
            };

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateFullNameAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Forbid();

            user.FullName = Input.FullName;
            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAddressAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Forbid();

            user.Address = Input.Address;
            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdatePhoneAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Forbid();

            user.Phone = Input.Phone;
            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdatePasswordAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Forbid();

            var result = await _userManager.ChangePasswordAsync(user, Input.CurrentPassword, Input.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Forbid();

            await _signInManager.SignOutAsync();
            await _userManager.DeleteAsync(user);
            return RedirectToPage("/Account/Login");
        }


    }
}
