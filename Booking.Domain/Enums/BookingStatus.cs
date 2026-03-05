using Booking.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Booking.Domain.Enums
{
    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Rejected,
        Completed,
        Cancelled
    }
}
