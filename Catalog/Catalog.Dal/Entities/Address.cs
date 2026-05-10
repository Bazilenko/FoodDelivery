using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Dal.Entities
{
    public class Address : BaseEntity
    {
        public int RestaurantId { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string BuildingNumber { get; set; }
        public string? PostalCode { get; set; }
        public decimal Latitude {get; set;}
        public decimal Longitude {get; set;}
        public Restaurant Restaurant { get; set; } = null!;
    }
}
