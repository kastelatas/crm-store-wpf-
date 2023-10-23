using CRM_Store.Core.Infastructure.Commands;
using CRM_Store.MVVM.ViewModels.Base;
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

            ChangeScreenCommand = new LambdaCommand(OnChangeScreenCommandExecuted, CanChangeScreenCommand);
        }
    }
}
