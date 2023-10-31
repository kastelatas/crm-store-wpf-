using CRM_Store.Core.Infastructure.Commands;
using CRM_Store.MVVM.Models;
using CRM_Store.MVVM.ViewModels.Base;
using CRM_Store.MVVM.Views.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CRM_Store.MVVM.ViewModels
{
    class MainViewModel : ViewModel
    {
        #region Variables
        public CalculatorViewModel CalculatorVM { get; set; }
        public MenuViewModel MenuVM { get; set; }
        public IngridientsViewModel IngridientsVM { get; set; }
        public ClientsViewModel ClientsVM { get; set; }
        public OrdersViewModel OrdersVM { get; set; }
        public StorageViewModel StorageVM { get; set; }

        private string selectedMenuItem;
        public string SelectedMenuItem
        {
            get { return selectedMenuItem; }
            set 
            {
                if (selectedMenuItem != value)
                {
                    selectedMenuItem = value;
                    ChangeScreenCommand.Execute(this);
                    OnPropertyChanged(nameof(SelectedMenuItem));
                }
            }
        }

        private object _currnetView;
        public object CurrentView
        {
            get { return _currnetView; }
            set
            {
                _currnetView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }
        #endregion

        #region Commands
        public ICommand ChangeScreenCommand { get; }

        private void OnChangeScreenCommandExecuted(object p)
        {
            
            Dictionary<string, object> menu = new()
            {
                { "menu", MenuVM },
                { "calc", CalculatorVM },
                { "ingridients", IngridientsVM },
                { "clients", ClientsVM },
                { "storage", StorageVM },
                { "orders", OrdersVM },
            };

            if (menu.ContainsKey(selectedMenuItem))
            {
                CurrentView = menu[selectedMenuItem];
            }
        }
        private bool CanChangeScreenCommand(object p) => true;

        #endregion

        public MainViewModel() 
        {
            MenuVM = new MenuViewModel();
            CalculatorVM = new CalculatorViewModel();
            IngridientsVM = new IngridientsViewModel();
            StorageVM = new StorageViewModel();
            OrdersVM = new OrdersViewModel();
            ClientsVM = new ClientsViewModel();

            CurrentView = MenuVM;

            ChangeScreenCommand = new LambdaCommand(OnChangeScreenCommandExecuted, CanChangeScreenCommand);
        }
    }
}
