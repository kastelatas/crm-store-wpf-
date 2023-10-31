using CRM_Store.Core.Infastructure.Commands;
using CRM_Store.MVVM.Models;
using CRM_Store.MVVM.ViewModels.Base;
using CRM_Store.MVVM.Views.Popups;
using CRM_Store.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CRM_Store.MVVM.ViewModels
{
    class CreateDishViewModel : ViewModel
    {
        public ObservableCollection<CalculatorTableItem> Ingridients { get; set; }
        public ObservableCollection<IngridientName> IngridientNames { get; set; }
        public ObservableCollection<QuantityUnit> QuantityUnits { get; set; }
        public string DishName
        {
            get { return Dish.Name; }
            set
            {
                Dish.Name = value;
                OnPropertyChanged(nameof(DishName));
            }
        }
        public double DishCost
        {
            get { return Dish.DishCost; }
            set
            {
                Dish.DishCost = value;
                OnPropertyChanged(nameof(ServingsCount));
            }
        }
        public double DishPrice
        {
            get { return Dish.DishPrice; }
            set
            {
                Dish.DishPrice = value;
                UpdateDishMarkup();
                OnPropertyChanged(nameof(ServingsCount));
            }
        }
        public double DishMarkup
        {
            get { return Dish.Markup; }
            set
            {
                Dish.Markup = value;
                OnPropertyChanged(nameof(ServingsCount));
            }
        }
        public double DishWeight
        {
            get { return Dish.DishWeight; }
            set
            {
                Dish.DishWeight = value;
                OnPropertyChanged(nameof(ServingsCount));
            }
        }
        public double ServingsCount
        {
            get { return Dish.ServingsCount; }
            set
            {
                Dish.ServingsCount = value;
                OnPropertyChanged(nameof(ServingsCount));
            }
        }
        public double OneServing
        {
            get { return Dish.OneServing; }
            set
            {
                Dish.OneServing = value;
                OnPropertyChanged(nameof(ServingsCount));
            }
        }
        public Dish Dish { get; set; }
        public ObservableCollection<Dish> Menu { get; set; }
        private void UpdateDishMarkup()
        {
            DishMarkup = Math.Round(((DishPrice - Dish.DishCost) / Dish.DishCost) * 100, 2);
            OnPropertyChanged(nameof(DishMarkup));
        }

        private void RecountForOutherServings()
        {
            if (ServingsCount > 0 && Dish.Ingridients.Count() > 0)
            {
                for (int i = 0; i < Dish.Ingridients.Count(); i++)
                {
                    var ingridient = Dish.Ingridients[i];
                    Dish.Ingridients[i].Quantity = (Math.Round((double.Parse(ingridient.Quantity) * ServingsCount), 3)).ToString();
                }

                Dish.ServingsCount = ServingsCount;

                double _dishCost = 0.0;
                double _dishWeight = 0.0;

                for (int i = 0; i < Dish.Ingridients.Count(); i++)
                {
                    var ingridient = Dish.Ingridients[i];

                    double localQuantity = double.Parse(ingridient.Quantity);

                    switch (ingridient.QuantityUnit)
                    {
                        case "кг":
                            if (localQuantity >= 1)
                            {
                                _dishWeight += (localQuantity * 1000);
                            }
                            else
                            {
                                _dishWeight += localQuantity;
                            }
                            break;

                        case "шт":
                            break;

                        case "л":
                            if (localQuantity >= 1)
                            {
                                _dishWeight += (localQuantity * 1000);
                            }
                            else
                            {
                                _dishWeight += localQuantity;
                            }
                            break;

                        default:
                            break;
                    }

                    _dishCost += localQuantity * double.Parse(ingridient.Price);
                }

                Dish.DishCost = Math.Round(_dishCost, 3);
                Dish.DishWeight = Math.Round(_dishWeight, 3);
                Dish.OneServing = Math.Round((_dishCost / ServingsCount), 3);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                };

                Dish = JsonSerializer.Deserialize<Dish>(JsonSerializer.Serialize(Dish, options), options);

                OnPropertyChanged(nameof(Dish));
                UpdateDishMarkup();

            }

        }

        public ICommand CreateDishCommand { get; }
        public ICommand SaveDishCommand { get; }
        public ICommand AddNewRowCommand { get; }
        public ICommand RemoveRowCommand { get; }

        private void OnCalculateDishCommandExecuted(object p)
        {
            DishPrice = 0.0;
            DishWeight = 0.0;
            OneServing = 0;

            foreach (var item in Dish.Ingridients)
            {
                if (item.Quantity != null)
                {
                 double localQuantity = double.Parse(item.Quantity);

                    switch (item.QuantityUnit)
                    {
                        case "кг":
                            if (localQuantity >= 1)
                            {
                                DishWeight += (localQuantity * 1000);
                            }
                            else
                            {
                                DishWeight += localQuantity;
                            }
                            break;

                        case "шт":
                            break;

                        case "л":
                            if (localQuantity >= 1)
                            {
                                DishWeight += (localQuantity * 1000);
                            }
                            else
                            {
                                DishWeight += localQuantity;
                            }
                            break;

                        default:
                            break;
                    }

                    DishPrice += Math.Round((localQuantity * double.Parse(item.Price)), 2);

                }
            }
            DishCost = DishPrice;

            if (ServingsCount > 0)
            {
                OneServing = Math.Round((DishPrice / ServingsCount), 2);
            }

            OnPropertyChanged(nameof(OneServing));
            OnPropertyChanged(nameof(DishCost));
            OnPropertyChanged(nameof(DishPrice));
            OnPropertyChanged(nameof(DishWeight));
        }
        private bool CanCreateDishCommand(object p) => true;

        private void OnSaveDishCommandExecuted(object p)
        {

            if (string.IsNullOrEmpty(DishName) || ServingsCount == 0 || Dish.Ingridients.Count() == 0)
            {
                MessageBox.Show("Данные не верные!");
            }
            else
            {
                ObservableCollection<CalculatorTableItem> IngridientsForOneServing = new ObservableCollection<CalculatorTableItem>();

                foreach (var item in Dish.Ingridients.ToList())
                {
                    if (item.Quantity != null)
                    {
                        double quantity = Math.Round((double.Parse(item.Quantity) / ServingsCount), 2);

                        IngridientsForOneServing.Add(new CalculatorTableItem
                        {
                            Name = item.Name,
                            Quantity = quantity.ToString(),
                            QuantityUnit = item.QuantityUnit,
                            Price = item.Price,
                        });
                    }
                    else
                    {
                        MessageBox.Show("Данные не верные!");
                    }

                }

                double dishWeightForOneServing = 0.0;
                double dishPriceForOneServing = 0.0;

                foreach (var item in IngridientsForOneServing)
                {
                    double localQuantity = double.Parse(item.Quantity);

                    switch (item.QuantityUnit)
                    {
                        case "кг":
                            if (localQuantity >= 1)
                            {
                                dishWeightForOneServing += (localQuantity * 1000);
                            }
                            else
                            {
                                dishWeightForOneServing += localQuantity;
                            }
                            break;

                        case "шт":
                            break;

                        case "л":
                            if (localQuantity >= 1)
                            {
                                dishWeightForOneServing += (localQuantity * 1000);
                            }
                            else
                            {
                                dishWeightForOneServing += localQuantity;
                            }
                            break;

                        default:
                            break;
                    }

                    dishPriceForOneServing += localQuantity * double.Parse(item.Price);
                }

                var currentDish = Menu.FirstOrDefault(i => i.Name == DishName);

                if (currentDish != null)
                {
                    currentDish.Ingridients = IngridientsForOneServing;

                    int indexToReplace = GlobalStore.Instance.Menu.IndexOf(GlobalStore.Instance.Menu.FirstOrDefault(item => item.ID == currentDish.ID));

                    if (indexToReplace >= 0)
                    {
                        GlobalStore.Instance.Menu.RemoveAt(indexToReplace);
                        GlobalStore.Instance.Menu.Insert(indexToReplace, currentDish);
                    }
                }
                else
                {
                    Menu.Add(new Dish
                    {
                        ID = Menu.Count + 1,
                        Name = DishName,
                        DishPrice = Math.Round(dishPriceForOneServing, 2),
                        DishCost = Math.Round(dishPriceForOneServing, 2),
                        DishWeight = Math.Round(dishWeightForOneServing, 2),
                        OneServing = Math.Round((dishPriceForOneServing / 1), 2),
                        ServingsCount = 1,
                        Markup = 0,
                        Ingridients = IngridientsForOneServing,
                    }
                    );
                }
                CreateDishCommand.Execute(this);
                GlobalStore.Instance.SaveToJsonFile(GlobalStore.Instance.Menu, "menu.json");
                MessageBox.Show("Блюдо добавленно в меню!");
            }
        }
        private bool CannSaveDishCommand(object p) => true;

        private void OnAddNewRowCommandExecuted(object p)
        {
            Dish.Ingridients.Add(new CalculatorTableItem());
        }
        private bool CanAddNewRowCommand(object p) => true;

        private void OnRemoveRowCommandExecuted(object p)
        {
            if (p is CalculatorTableItem item)
            {
                Dish.Ingridients.Remove(item);
            }
        }
        private bool CanRemoveRowCommand(object p) => true;
        public CreateDishViewModel()
        {
            Dish = new Dish();
            Dish.Ingridients = new ObservableCollection<CalculatorTableItem>();
            Menu = GlobalStore.Instance.Menu;
            IngridientNames = GlobalStore.Instance.IngridientNames;
            QuantityUnits = GlobalStore.Instance.QuantityUnits;

            CreateDishCommand = new LambdaCommand(OnCalculateDishCommandExecuted, CanCreateDishCommand);
            SaveDishCommand = new LambdaCommand(OnSaveDishCommandExecuted, CannSaveDishCommand);
            AddNewRowCommand = new LambdaCommand(OnAddNewRowCommandExecuted, CanAddNewRowCommand);
            RemoveRowCommand = new LambdaCommand(OnRemoveRowCommandExecuted, CanRemoveRowCommand);
        }

    }
}
