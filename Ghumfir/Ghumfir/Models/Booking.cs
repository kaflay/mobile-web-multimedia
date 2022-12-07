using System;
using System.Collections.Generic;

namespace Ghumfir.Models
{
    public partial class Booking
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public int PackageId { get; set; }
        public string? Status { get; set; }

        public virtual Package Package { get; set; } = null!;
        public virtual AspNetUser User { get; set; } = null!;
    }
}
