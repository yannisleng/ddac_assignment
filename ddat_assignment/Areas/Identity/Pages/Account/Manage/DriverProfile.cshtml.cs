using System.ComponentModel.DataAnnotations;
using ddat_assignment.Areas.Identity.Data;
using ddat_assignment.Data;
using ddat_assignment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ddat_assignment.Areas.Identity.Pages.Account.Manage
{
    [Authorize(Roles = "Driver")]
    public class DriverProfileModel : PageModel
    {
        private readonly UserManager<ddat_assignmentUser> _userManager;
        private readonly SignInManager<ddat_assignmentUser> _signInManager;
        private readonly ddat_assignmentContext _context;

        public DriverProfileModel(
            UserManager<ddat_assignmentUser> userManager,
            SignInManager<ddat_assignmentUser> signInManager,
            ddat_assignmentContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public string Username { get; set; }
        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public PersonalInfoInputModel PersonalInfo { get; set; }

        [BindProperty]
        public LicenseInputModel LicenseInfo { get; set; }

        [BindProperty]
        public WorkPreferencesInputModel WorkPreferences { get; set; }

        [BindProperty]
        public EmergencyContactInputModel EmergencyContact { get; set; }

        public class PersonalInfoInputModel
        {
            [Required]
            [StringLength(100, MinimumLength = 2)]
            [RegularExpression(@"^[a-zA-Z\s']+$", ErrorMessage = "First name must contain only letters.")]
            public string FirstName { get; set; }

            [Required]
            [StringLength(100, MinimumLength = 2)]
            [RegularExpression(@"^[a-zA-Z\s']+$", ErrorMessage = "Last name must contain only letters.")]
            public string LastName { get; set; }

            [Required]
            [RegularExpression(@"^0\d{8,10}$", ErrorMessage = "Phone number must start with 0 and be 9-11 digits long.")]
            public string PhoneNumber { get; set; }

            [EmailAddress]
            public string? Email { get; set; }

            [DataType(DataType.Password)]
            public string? NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string? ConfirmPassword { get; set; }

            [Required]
            [StringLength(200, MinimumLength = 2)]
            public string Address { get; set; }

            [Required]
            [RegularExpression(@"^\d{6}-\d{2}-\d{4}$", ErrorMessage = "Identity Card Number must be in the format xxxxxx-xx-xxxx and contain only digits.")]
            public string IdentityCardNumber { get; set; }

            [Required]
            [Display(Name = "Gender")]
            public string Gender { get; set; }

            [Required]
            [DataType(DataType.Date)]
            public DateTime DateOfBirth { get; set; }
        }

        public class LicenseInputModel
        {
            [Required]
            [RegularExpression(@"^\d{6}-\d{2}-\d{4}$", ErrorMessage = "License ID must be in the format xxxxxx-xx-xxxx and contain only digits.")]
            public string LicenseId { get; set; }

            [Required]
            public string DrivingLicenseType { get; set; }

            [Required]
            public DateTime DrivingLicenseExpiryDate { get; set; }

            [Required]
            public string VehicleType { get; set; }

            [Required]
            [StringLength(7, MinimumLength = 3)]
            public string VehiclePlateNumber { get; set; }
        }

        public class WorkPreferencesInputModel
        {
            public DateTime? StartDate { get; set; }

            [Required]
            public string PreferredWorkingDay { get; set; }

            [Required]
            public string PreferredWorkingLocation { get; set; }
        }

        public class EmergencyContactInputModel
        {
            [Required]
            [StringLength(100, MinimumLength = 2)]
            [RegularExpression(@"^[a-zA-Z\s']+$", ErrorMessage = "Emergency contact name must contain only letters, spaces, and apostrophes.")]
            public string EmergencyContactName { get; set; }

            [Required]
            [RegularExpression(@"^0\d{8,10}$", ErrorMessage = "Emergency contact number must start with 0 and be 9-11 digits long.")]
            public string EmergencyContactPhone { get; set; }

            [Required]
            [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Emergency contact relationship must contain only letters.")]
            public string EmergencyContactRelationship { get; set; }
        }

        private async Task LoadPersonalInfoAsync(ddat_assignmentUser user, UserDetailsModel userDetails)
        {
            PersonalInfo = new PersonalInfoInputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                IdentityCardNumber = user.IdentityCardNumber,
                DateOfBirth = (DateTime)user.DateOfBirth,
                Email = user.Email,
                NewPassword = string.Empty,
                ConfirmPassword = string.Empty,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                Address = userDetails.Address,
            };
        }
        private async Task LoadLicenseAsync(DriverModel driver)
        {
            LicenseInfo = new LicenseInputModel
            {
                LicenseId = driver.LicenseId,
                DrivingLicenseType = driver.DrivingLicenseType,
                DrivingLicenseExpiryDate = (DateTime)driver.DrivingLicenseExpiryDate,
                VehicleType = driver.VehicleType,
                VehiclePlateNumber = driver.VehiclePlateNumber
            };
        }

        private async Task LoadWorkingPreferencesAsync(DriverModel driver)
        {
            WorkPreferences = new WorkPreferencesInputModel
            {
                StartDate = (DateTime)driver.StartDate,
                PreferredWorkingDay = driver.PreferredWorkingDay,
                PreferredWorkingLocation = driver.PreferredWorkingLocation
            };
        }

        private async Task LoadEmergencyContactAsync(DriverModel driver)
        {
            EmergencyContact = new EmergencyContactInputModel
            {
                EmergencyContactName = driver.EmergencyContactName,
                EmergencyContactPhone = driver.EmergencyContactPhone,
                EmergencyContactRelationship = driver.EmergencyContactRelationship
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userDetails = await _context.UserDetailsModel.FirstOrDefaultAsync(u => u.UserId == user.Id);
            var driver = await _context.DriverModel.FirstOrDefaultAsync(d => d.User.Id == user.Id);

            await LoadPersonalInfoAsync(user, userDetails);
            await LoadLicenseAsync(driver);
            await LoadWorkingPreferencesAsync(driver);
            await LoadEmergencyContactAsync(driver);
            return Page();
        }

        public async Task<IActionResult> OnPostPersonalInfoAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userDetails = await _context.UserDetailsModel.FirstOrDefaultAsync(u => u.UserId == user.Id);
            var driver = await _context.DriverModel.FirstOrDefaultAsync(d => d.User.Id == user.Id);

            ModelState.Clear();
            TryValidateModel(PersonalInfo, nameof(PersonalInfo));

            if (!ModelState.IsValid)
            {
                await LoadPersonalInfoAsync(user, userDetails);
                await LoadEmergencyContactAsync(driver);
                return Page();
            }

            user.FirstName = PersonalInfo.FirstName;
            user.LastName = PersonalInfo.LastName;
            user.IdentityCardNumber = PersonalInfo.IdentityCardNumber;
            user.DateOfBirth = PersonalInfo.DateOfBirth;
            user.PhoneNumber = PersonalInfo.PhoneNumber;
            user.Gender = PersonalInfo.Gender;
            userDetails.Address = PersonalInfo.Address;

            var updateResult = await _userManager.UpdateAsync(user);
            _context.UserDetailsModel.Update(userDetails);
            await _context.SaveChangesAsync();

            if (!updateResult.Succeeded)
            {
                StatusMessage = "Unexpected error when trying to update profile.";
                return RedirectToPage();
            }

            if (!string.IsNullOrEmpty(PersonalInfo.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, PersonalInfo.NewPassword);

                if (!passwordResult.Succeeded)
                {
                    StatusMessage = "Error updating password.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostLicenseAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var driver = await _context.DriverModel.FirstOrDefaultAsync(d => d.User.Id == user.Id);

            ModelState.Clear();
            TryValidateModel(LicenseInfo, nameof(LicenseInfo));

            if (!ModelState.IsValid)
            {
                await LoadLicenseAsync(driver);
                return Page();
            }

            driver.LicenseId = LicenseInfo.LicenseId;
            driver.DrivingLicenseType = LicenseInfo.DrivingLicenseType;
            driver.DrivingLicenseExpiryDate = LicenseInfo.DrivingLicenseExpiryDate;
            driver.VehicleType = LicenseInfo.VehicleType;
            driver.VehiclePlateNumber = LicenseInfo.VehiclePlateNumber;

            _context.DriverModel.Update(driver);
            await _context.SaveChangesAsync();

            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostWorkingPreferencesAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var driver = await _context.DriverModel.FirstOrDefaultAsync(d => d.User.Id == user.Id);

            ModelState.Clear();
            TryValidateModel(WorkPreferences, nameof(WorkPreferences));

            if (!ModelState.IsValid)
            {
                await LoadLicenseAsync(driver);
                return Page();
            }

            driver.PreferredWorkingDay = WorkPreferences.PreferredWorkingDay;
            driver.PreferredWorkingLocation = WorkPreferences.PreferredWorkingLocation;

            _context.DriverModel.Update(driver);
            await _context.SaveChangesAsync();

            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEmergencyContactAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var driver = await _context.DriverModel.FirstOrDefaultAsync(d => d.User.Id == user.Id);

            ModelState.Clear();
            TryValidateModel(EmergencyContact, nameof(EmergencyContact));

            if (!ModelState.IsValid)
            {
                await LoadEmergencyContactAsync(driver);
                return Page();
            }

            driver.EmergencyContactName = EmergencyContact.EmergencyContactName;
            driver.EmergencyContactPhone = EmergencyContact.EmergencyContactPhone;
            driver.EmergencyContactRelationship = EmergencyContact.EmergencyContactRelationship;

            _context.DriverModel.Update(driver);
            await _context.SaveChangesAsync();

            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}

