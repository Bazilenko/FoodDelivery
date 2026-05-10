namespace Catalog.Dal.Entities{
    public class ModifierGroup : BaseEntity{
        public int DishId {get; set;}
        public string Name {get; set;}
        public int MinSelect {get;set;}
        public int MaxSelect {get;set;}

        public Dish Dish {get; set;} = null!;
        public ICollection<DishOption> dishOptions {get; set;} = [];
    }
    
}