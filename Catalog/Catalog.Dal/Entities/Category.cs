using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Dal.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public int RestaurantId {get; set; }
        public ICollection<Dish> Dishes { get; set; } = [];
        public Restaurant Restaurant {get; set;} = null!;
    }
}
