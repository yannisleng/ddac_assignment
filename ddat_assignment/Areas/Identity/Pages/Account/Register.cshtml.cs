// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using ddat_assignment.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using ddat_assignment.Models;
using Microsoft.EntityFrameworkCore;
using ddat_assignment.Data; // Add this line to include your context

namespace ddat_assignment.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ddat_assignmentUser> _signInManager;
        private readonly UserManager<ddat_assignmentUser> _userManager;
        private readonly IUserStore<ddat_assignmentUser> _userStore;
        private readonly IUserEmailStore<ddat_assignmentUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ddat_assignmentContext _context; // Add this line to declare the context

        public RegisterModel(
            UserManager<ddat_assignmentUser> userManager,
            IUserStore<ddat_assignmentUser> userStore,
            SignInManager<ddat_assignmentUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ddat_assignmentContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string SuccessMessage { get; set; }

        public class InputModel
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

            [Required]
            [StringLength(255, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [DataType(DataType.Date)]
            [Display(Name = "Date of Birth")]
            public DateTime DateOfBirth { get; set; }

            //[StringLength(10)]
            //public string? Gender { get; set; }

            public string Role { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                Response.Redirect(Url.Content("~/"));
                return;
            }

            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogError($"Validation error: {error.ErrorMessage}");
                    }
                }
                return Page();
            }

            var user = CreateUser();

            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.IdentityCardNumber = Input.IdentityCardNumber;
            user.Email = Input.Email;
            user.PhoneNumber = Input.PhoneNumber;
            user.DateOfBirth = Input.DateOfBirth;
            user.Role = "Customer";

            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                var roleResult = await _userManager.AddToRoleAsync(user, "Customer");
                if (!roleResult.Succeeded)
                {
                    _logger.LogError("Failed to assign the 'Customer' role to the user.");
                    foreach (var error in roleResult.Errors)
                    {
                        _logger.LogError($"Error: {error.Description}");
                    }
                    ModelState.AddModelError(string.Empty, "Failed to assign the 'Customer' role.");
                    return Page();
                }

                await _userManager.UpdateAsync(user);

                // Create and save the UserDetailsModel
                var userDetails = new UserDetailsModel
                {
                    UserId = user.Id,
                    Address = null // Set address as null
                };

                _context.UserDetailsModel.Add(userDetails);
                await _context.SaveChangesAsync();

                // Redirect to login or some other page
                return RedirectToPage("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }



        private ddat_assignmentUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ddat_assignmentUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ddat_assignmentUser)}'. " +
                    $"Ensure that '{nameof(ddat_assignmentUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ddat_assignmentUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ddat_assignmentUser>)_userStore;
        }
    }
}
