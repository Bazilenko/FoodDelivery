using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Dal.Entities
{
    public class Dish : BaseEntity
    {
        public int RestaurantId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? Weight {get; set;}
        public decimal? Calories {get; set;}
        public string? Unit {get; set;}
        public string? ImageUrl { get; set; }
        public bool IsAvailable {get; set; }

        public Category Category { get; set; } = null!;
        public Restaurant Restaturant { get; set; } = null!;
        public ICollection<DishOption?> DishOptions { get; set; } = [];
        public ICollection<ModifierGroup> ModifierGroups {get; set;} = [];

    }
}
