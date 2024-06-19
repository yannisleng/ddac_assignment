using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ddat_assignment.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ddat_assignmentUser class
public class ddat_assignmentUser : IdentityUser
{
    [PersonalData]
    [StringLength(50, MinimumLength = 2)]
    public string ? FirstName { get; set; }

    [PersonalData]
    [StringLength(50, MinimumLength = 2)]
    public string ? LastName { get; set; }

    [Display(Name = "Full Name")]
    public string FullName { get { return LastName + " " + FirstName; } }

    [PersonalData]
    [StringLength(50)]
    [Remote("IsIdentityCardNumberExists", "ddat_assignmentUser", ErrorMessage = "IdentityCard Number already exists")]
    public string ? IdentityCardNumber { get; set; }

    [PersonalData]
    [StringLength(20)]
    public string ? Role { get; set; }
}

