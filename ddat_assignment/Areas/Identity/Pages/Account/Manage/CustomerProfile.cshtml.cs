using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ddat_assignment.Areas.Identity.Data;
using ddat_assignment.Data;
using ddat_assignment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ddat_assignment.Areas.Identity.Pages.Account.Manage
{
    public class CustomerProfileModel : PageModel
    {
        private readonly UserManager<ddat_assignmentUser> _userManager;
        private readonly SignInManager<ddat_assignmentUser> _signInManager;
        private readonly ddat_assignmentContext _context;
        private readonly ILogger<CustomerProfileModel> _logger;

        public CustomerProfileModel(
            UserManager<ddat_assignmentUser> userManager,
            SignInManager<ddat_assignmentUser> signInManager,
            ddat_assignmentContext context,
            ILogger<CustomerProfileModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public CustomerPersonalInfoInputModel CustomerPersonalInfo { get; set; }

        public class CustomerPersonalInfoInputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(50)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [StringLength(50)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [StringLength(50)]
            [Display(Name = "Identity Card Number")]
            public string IdentityCardNumber { get; set; }

            [Required]
            [StringLength(15)]
            [Phone(ErrorMessage = "Invalid phone number format.")]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [StringLength(255, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string? Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string? ConfirmPassword { get; set; }

            [DataType(DataType.Date)]
            [Display(Name = "Date of Birth")]
            public DateTime DateOfBirth { get; set; }

            [StringLength(10)]
            public string? Gender { get; set; }
        }

        private async Task LoadAsync(ddat_assignmentUser user)
        {
            CustomerPersonalInfo = new CustomerPersonalInfoInputModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IdentityCardNumber = user.IdentityCardNumber,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = (DateTime)user.DateOfBirth,
                Gender = user.Gender,
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            user.FirstName = CustomerPersonalInfo.FirstName;
            user.LastName = CustomerPersonalInfo.LastName;
            user.IdentityCardNumber = CustomerPersonalInfo.IdentityCardNumber;
            user.DateOfBirth = CustomerPersonalInfo.DateOfBirth;
            user.Gender = CustomerPersonalInfo.Gender;
            user.PhoneNumber = CustomerPersonalInfo.PhoneNumber;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    _logger.LogError("Error updating user: {Error}", error.Description);
                }
                StatusMessage = "Unexpected error when trying to update profile.";
                return RedirectToPage();
            }

            if (!string.IsNullOrEmpty(CustomerPersonalInfo.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, CustomerPersonalInfo.Password);

                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                    {
                        _logger.LogError("Error updating password: {Error}", error.Description);
                    }
                    StatusMessage = "Error updating password.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }


    }
}
