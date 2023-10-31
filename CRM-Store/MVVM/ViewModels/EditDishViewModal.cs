using CRM_Store.MVVM.Models;
using CRM_Store.MVVM.ViewModels.Base;
using CRM_Store.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Text.Json;
using Microsoft.VisualBasic;
using CRM_Store.Core.Infastructure.Commands;
using CRM_Store.MVVM.Views.Popups;

namespace CRM_Store.MVVM.ViewModels
{
    class EditDishViewModal : ViewModel
    {   
        private Window _window;
        public ObservableCollection<Dish> Menu { get; set; }
        public ObservableCollection<IngridientName> IngridientNames { get; set; }
        public ObservableCollection<QuantityUnit> QuantityUnits { get; set; }

        private Dish _dishForOneServing;

        private Dish _dish;
        public Dish Dish
        {
            get { return _dish; }
            set
            {
                if (_dish != value)
                {
                    _dish = value;
                    OnPropertyChanged(nameof(Dish)); 
                }
            }
        }

        public double ServingsCount
        {
            get { return Dish.ServingsCount; }
            set
            {
                Dish.ServingsCount = value; 
                OnPropertyChanged(nameof(ServingsCount));
                RecountForOutherServings();
            }
        }

        public double DishPrice
        {
            get { return Dish.DishPrice; }
            set
            {
                Dish.DishPrice = value;
                OnPropertyChanged(nameof(DishPrice));
                UpdateDishMarkup();
            }
        }

        public double DishMarkup
        {
            get { return Dish.Markup; }
            set
            {
                Dish.Markup = value;
                OnPropertyChanged(nameof(DishMarkup));
            }
        }

        public ICommand SaveUpdateDishCommand { get; }
        public ICommand CloseDishCommand { get; }
        public ICommand RemoveRowCommand { get; }
        public ICommand AddNewRowCommand { get; }

        private void OnSaveUpdateDishCommandExecuted(object p)
        {
            int indexToReplace = GlobalStore.Instance.Menu.IndexOf(GlobalStore.Instance.Menu.FirstOrDefault(item => item.ID == Dish.ID));

            if (indexToReplace >= 0)
            {
                GlobalStore.Instance.Menu.RemoveAt(indexToReplace);
                GlobalStore.Instance.Menu.Insert(indexToReplace, Dish);
                RecountForOutherServings();
                //_window.Close();
            }

        }
        private bool CanSaveUpdateDishCommand(object p) => true;

        private void OnCloseDishCommandExecuted(object p)
        {
            _window.Close();
        }
        private bool CanCloseDishCommand(object p) => true;

        private void OnRemoveRowCommandExecuted(object p)
        {
            if (p is CalculatorTableItem item)
            {
                Dish.Ingridients.Remove(item);
            }
        }
        private bool CanRemoveRowCommand(object p) => true;

        private void OnAddNewRowCommandExecuted(object p)
        {
            Dish.Ingridients.Add(new CalculatorTableItem());
        }
        private bool CanAddNewRowCommand(object p) => true;

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
                    var ingridientForOneServing = Dish.Ingridients[i];
                    Dish.Ingridients[i].Quantity = (Math.Round((double.Parse(ingridientForOneServing.Quantity) * ServingsCount), 3)).ToString();
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

                Dish.DishCost = Math.Round(_dishCost, 2);
                Dish.DishWeight = Math.Round(_dishWeight, 3);
                Dish.OneServing = Math.Round((_dishCost / ServingsCount), 2);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                };

                Dish = JsonSerializer.Deserialize<Dish>(JsonSerializer.Serialize(Dish, options), options);

                OnPropertyChanged(nameof(Dish));
                UpdateDishMarkup();

            }

        }

        private Dish CloneDish(Dish source)
        {
            var clonedDish = new Dish
            {
                ID = source.ID,
                DishCost = source.DishCost,
                DishPrice = source.DishPrice,
                DishWeight = source.DishWeight,
                Name = source.Name,
                Markup = source.Markup,
                OneServing = source.OneServing,
                ServingsCount = source.ServingsCount,
                Ingridients = new ObservableCollection<CalculatorTableItem>()
            };

            foreach (var ingridient in source.Ingridients)
            {
                clonedDish.Ingridients.Add(new CalculatorTableItem
                {
                    Name = ingridient.Name,
                    Price = ingridient.Price,
                    Quantity = ingridient.Quantity,
                    QuantityUnit = ingridient.QuantityUnit
                });
            }

            return clonedDish;
        }

        public EditDishViewModal(Dish dish, Window window)
        {
            _dishForOneServing = dish;
            _window = window;

            Dish = CloneDish(_dishForOneServing);

            Menu = GlobalStore.Instance.Menu;
            IngridientNames = GlobalStore.Instance.IngridientNames;
            QuantityUnits = GlobalStore.Instance.QuantityUnits;

            SaveUpdateDishCommand = new LambdaCommand(OnSaveUpdateDishCommandExecuted, CanSaveUpdateDishCommand);
            CloseDishCommand = new LambdaCommand(OnCloseDishCommandExecuted, CanCloseDishCommand);
            RemoveRowCommand = new LambdaCommand(OnRemoveRowCommandExecuted, CanRemoveRowCommand);
            AddNewRowCommand = new LambdaCommand(OnAddNewRowCommandExecuted, CanAddNewRowCommand);
        }
    }
}
