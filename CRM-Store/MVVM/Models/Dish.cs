using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM_Store.MVVM.Models
{
    public class Dish
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double DishPrice { get; set; }
        public double DishCost { get; set; }
        public double DishWeight { get; set; }
        public double DefaultServingsCount { get; set; }
        public double ServingsCount { get; set; }
        public double OneServing { get; set; }
        public double Markup { get; set; }
        public ObservableCollection<CalculatorTableItem> Ingridients { get; set; }
    }
}
