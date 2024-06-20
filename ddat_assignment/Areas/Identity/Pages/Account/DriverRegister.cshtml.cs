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
    public class DriverRegisterModel : PageModel
    {
        private readonly SignInManager<ddat_assignmentUser> _signInManager;
        private readonly UserManager<ddat_assignmentUser> _userManager;
        private readonly IUserStore<ddat_assignmentUser> _userStore;
        private readonly IUserEmailStore<ddat_assignmentUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ddat_assignmentContext _context;

        public DriverRegisterModel(
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
        /// 
        public string SuccessMessage { get; set; }

        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            // ASP.NET Core Identity properties
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            // Additional properties
            [Required]
            [StringLength(100)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [StringLength(100)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [StringLength(20)]
            [Phone(ErrorMessage = "Invalid phone number format.")]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [Required]
            [StringLength(255)]
            [Display(Name = "Address")]
            public string Address { get; set; }

            [Required]
            [StringLength(50)]
            [Display(Name = "Identity Card Number")]
            public string IdentityCardNumber { get; set; }

            [Required]
            [StringLength(10)]
            [Display(Name = "Gender")]
            public string Gender { get; set; }

            [Required]
            [DataType(DataType.Date)]
            [Display(Name = "Date of Birth")]
            public DateTime DateOfBirth { get; set; }

            [Required]
            [StringLength(50)]
            [Display(Name = "License ID")]
            public string LicenseId { get; set; }

            //[Required]
            //[Display(Name = "Driving License Image")]
            //public byte[] LicenseImage { get; set; }

            [Required]
            [StringLength(50)]
            [Display(Name = "Driving License Type")]
            public string DrivingLicenseType { get; set; }

            [Required]
            [Display(Name = "Driving License Expiry Date")]
            public DateTime? DrivingLicenseExpiryDate { get; set; }

            [Required]
            [StringLength(50)]
            [Display(Name = "Vehicle Type")]
            public string VehicleType { get; set; }

            [Required]
            [StringLength(50)]
            [Display(Name = "Vehicle Plate Number")]
            public string VehiclePlateNumber { get; set; }

            [Required]
            [Display(Name = "Start Date")]
            [DataType(DataType.Date)]
            public DateTime? StartDate { get; set; }

            [Required]
            [StringLength(50)]
            [Display(Name = "Preferred Working Day")]
            public string PreferredWorkingDay { get; set; }

            [Required]
            [StringLength(100)]
            [Display(Name = "Preferred Working Location")]
            public string PreferredWorkingLocation { get; set; }

            [Required]
            [StringLength(100)]
            [Display(Name = "Emergency Contact Name")]
            public string EmergencyContactName { get; set; }

            [Required]
            [StringLength(20)]
            [Phone(ErrorMessage = "Invalid phone number format.")]
            [Display(Name = "Emergency Contact Phone")]
            public string EmergencyContactPhone { get; set; }
            
            [Required]
            [StringLength(50)]
            [Display(Name = "Emergency Contact Relationship")]
            public string EmergencyContactRelationship { get; set; }
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

                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.PhoneNumber = Input.PhoneNumber;
                user.Gender = Input.Gender;
                user.DateOfBirth = Input.DateOfBirth;
                user.Role = "Driver";
                user.IdentityCardNumber = Input.IdentityCardNumber;

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, "admin@123A");

                if (result.Succeeded)
                {
                    await _userManager.UpdateAsync(user);

                    var userDetails = new UserDetailsModel
                    {
                        Address = Input.Address,
                        UserId = user.Id,
                        User = user
                    };

                    var driver = new DriverModel
                    {
                        LicenseId = Input.LicenseId,
                        //LicenseImage = Input.LicenseImage,
                        DrivingLicenseType = Input.DrivingLicenseType,
                        DrivingLicenseExpiryDate = Input.DrivingLicenseExpiryDate,
                        VehicleType = Input.VehicleType,
                        VehiclePlateNumber = Input.VehiclePlateNumber,
                        StartDate = Input.StartDate,
                        PreferredWorkingDay = Input.PreferredWorkingDay,
                        PreferredWorkingLocation = Input.PreferredWorkingLocation,
                        EmergencyContactName = Input.EmergencyContactName,
                        EmergencyContactPhone = Input.EmergencyContactPhone,
                        EmergencyContactRelationship = Input.EmergencyContactRelationship,
                        User = user
                    };

                    _context.UserDetailsModel.Add(userDetails);
                    _context.DriverModel.Add(driver);
                    await _context.SaveChangesAsync();

                    SuccessMessage = "Registered as OK Boss's driver successfully!";

                    //_logger.LogInformation("User created a new account with password.");

                    //var userId = await _userManager.GetUserIdAsync(user);
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);

                foreach (var error in errors)
                {
                    if (!string.IsNullOrEmpty(error.ErrorMessage))
                    {
                        Console.WriteLine($"Error: {error.ErrorMessage}");
                    }

                    if (error.Exception != null)
                    {
                        Console.WriteLine($"Exception: {error.Exception}");
                    }
                }
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
