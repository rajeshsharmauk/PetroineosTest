using System;
using System.Collections.Generic;
using System.Text;

namespace PowerPosition.Server
{
    /// <summary>
    /// Represents total volume for a time period.
    /// </summary>
    public class TotalPerTimePeriod
    {
        public required string TimePeriod { get; set; }
        public double Total { get; set; }
    }
}