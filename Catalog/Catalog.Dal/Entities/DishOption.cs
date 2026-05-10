using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Dal.Entities
{
    public class DishOption : BaseEntity
    {
        public int ModifierGroupId {get; set;}
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }

        public ModifierGroup ModifierGroup {get; set;} = null!;
    }
}
