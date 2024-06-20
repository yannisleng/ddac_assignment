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
using ddat_assignment.Data;

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
        private readonly ddat_assignmentContext _context;

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 5;

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

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                if (CurrentPage == 1)
                {
                    user.FirstName = "John";
                    user.LastName = "Doe";
                    user.Gender = "Male";
                    user.DateOfBirth = DateTime.Now;
                    user.Role = "Driver";

                    CurrentPage++;
                    return RedirectToPage("/Account/Register", new { page = CurrentPage });
                }
                else if (CurrentPage == 2)
                {
                    user.IdentityCardNumber = "1234567890";

                    CurrentPage++;
                    return RedirectToPage("/Account/Register", new { page = CurrentPage });
                }
                else if (CurrentPage == 3)
                {
                    await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (result.Succeeded)
                    {
                        await _userManager.UpdateAsync(user);

                        var userDetails = new UserDetailsModel
                        {
                            Address = "123 Main St, Anytown, USA",
                            UserId = user.Id,
                            User = user
                        };

                        var driver = new DriverModel
                        {
                            LicenseId = "DL123456", // Sample license ID
                            DrivingLicenseType = "Class C", // Sample driving license type
                            DrivingLicenseExpiryDate = DateTime.Today.AddYears(5), // Sample expiry date
                            VehicleType = "SUV", // Sample vehicle type
                            VehiclePlateNumber = "ABC1234", // Sample vehicle plate number
                            StartDate = DateTime.Today.AddDays(7), // Sample start date
                            PreferredWorkingDay = "Monday", // Sample preferred working day
                            PreferredWorkingLocation = "City Center", // Sample preferred working location
                            EmergencyContactName = "Jane Doe", // Sample emergency contact name
                            EmergencyContactPhone = "123-456-7890", // Sample emergency contact phone number
                            EmergencyContactRelationship = "Spouse", // Sample emergency contact relationship
                            User = user
                        };

                        // Save UserDetailsModel and DriverModel to database
                        _context.UserDetailsModel.Add(userDetails);
                        _context.DriverModel.Add(driver);
                        await _context.SaveChangesAsync();

                        _logger.LogInformation("User created a new account with password.");

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
             }
            // If we got this far, something failed, redisplay form
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
