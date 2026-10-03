using System;
using System.Collections.Generic;

namespace CookerAPI.Models;

public partial class UserRolesName
{
    public int? RoleId { get; set; }

    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? RoleName { get; set; }
}
