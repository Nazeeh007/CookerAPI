using System;
using System.Collections.Generic;

namespace CookerAPI.Models;

public partial class Group
{
    public int Id { get; set; }

    public string? Gname { get; set; }

    public string? Gimage { get; set; }

    public bool? Gstatus { get; set; }
}
