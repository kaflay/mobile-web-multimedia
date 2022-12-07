using System;
using System.Collections.Generic;

namespace Ghumfir.Models
{
    public partial class Category
    {
        public Category()
        {
            Packages = new HashSet<Package>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public virtual ICollection<Package> Packages { get; set; }
    }
}
