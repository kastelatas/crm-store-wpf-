using CRM_Store.Core.Infastructure.Commands;
using CRM_Store.MVVM.Models;
using CRM_Store.MVVM.ViewModels.Base;
using CRM_Store.MVVM.Views.Popups;
using CRM_Store.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace CRM_Store.MVVM.ViewModels
{
    public class MenuViewModel : ViewModel
    {
        private Window _currentWindow;
        public ObservableCollection<Dish> Menu { get; set; }

        public ICommand EditDishCommand { get; }
        public ICommand RemoveRowCommand { get; }

        private void OnEditDishCommandExecuted(object p)
        {
            if (p is Dish dish)
            {
                var EditDishWindow = new EditDish(dish);
                EditDishWindow.ShowDialog();
            }
            
        }
        private bool CanEditDishCommand(object p) => true;

        private void OnRemoveRowCommandExecuted(object p)
        {
            if (p is Dish dish)
            {
                Menu.Remove(dish);
                GlobalStore.Instance.SaveToJsonFile(Menu, "menu.json");
            }
        }
        private bool CanRemoveRowCommand(object p) => true;
        public ICommand CreateDishCommand { get; }
        private void OnCreateDishCommandExecuted(object p)
        {
            var CreateDishWindow = new CreateDish();

            if (_currentWindow != null)
            {
                CreateDishWindow.Owner = _currentWindow;
                CreateDishWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            
            CreateDishWindow.ShowDialog();
        }
        private bool CanCreateDishCommand(object p) => true;

        public MenuViewModel(Window currentWindow = null)
        {
            _currentWindow = currentWindow;
            EditDishCommand = new LambdaCommand(OnEditDishCommandExecuted, CanEditDishCommand);
            RemoveRowCommand = new LambdaCommand(OnRemoveRowCommandExecuted, CanRemoveRowCommand);
            CreateDishCommand = new LambdaCommand(OnCreateDishCommandExecuted, CanCreateDishCommand);


            Menu = GlobalStore.Instance.Menu;
        }
    }
}
