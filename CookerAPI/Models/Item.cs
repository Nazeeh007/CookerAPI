using System;
using System.Collections.Generic;

namespace CookerAPI.Models;

public partial class Item
{
    public int Id { get; set; }

    public int? Gid { get; set; }

    public string? ItmName { get; set; }

    public double? ItmPrice { get; set; }

    public string? ItmDesc { get; set; }

    public string? ItmImage { get; set; }

    public bool? ItmStatus { get; set; }
}
