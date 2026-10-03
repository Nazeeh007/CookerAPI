using System;
using System.Collections.Generic;

namespace CookerAPI.Models;

public partial class TotalOrder
{
    public int Id { get; set; }

    public int? ToBillNo { get; set; }

    public int? ToTableNo { get; set; }

    public double? ToTotal { get; set; }

    public bool? PayFlag { get; set; }
}
