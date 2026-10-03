using System;
using System.Collections.Generic;

namespace CookerAPI.Models;

public partial class Order
{
    public int Id { get; set; }

    public int? OrderBillNo { get; set; }

    public int? OrderTableNo { get; set; }

    public DateTime? OrderDate { get; set; }

    public int? ItmId { get; set; }

    public double? OrderPrice { get; set; }

    public double? Quntity { get; set; }

    public double? OrderTotal { get; set; }

    public bool? ConfirmFlag { get; set; }
}
