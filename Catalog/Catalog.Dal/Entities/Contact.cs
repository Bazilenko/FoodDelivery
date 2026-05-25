using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalog.Dal.Enums;

namespace Catalog.Dal.Entities
{
    public class Contact : BaseEntity
    {
        public int RestaurantId { get; set; }
        public ContactType Type { get; set; }
        public string Value { get; set; }
        public Restaurant Restaurant { get; set; } = null!;

    }
}
