using System;
using System.Collections.Generic;

namespace CookerAPI.Models;

public partial class UserPass
{
    public int Id { get; set; }

    public string? EmployeeName { get; set; }

    public string? UserName { get; set; }

    public string? Pass { get; set; }

    public bool? UserStatus { get; set; }
}
