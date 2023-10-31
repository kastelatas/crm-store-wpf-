using CRM_Store.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CRM_Store.MVVM.Views.Components
{
    /// <summary>
    /// Interaction logic for CalculatorComponent.xaml
    /// </summary>
    public partial class CalculatorComponent : UserControl
    {
        private CalculatorViewModel _calculatorViewModel;
        public CalculatorComponent()
        {
            InitializeComponent();

            _calculatorViewModel = new CalculatorViewModel(this);

            DataContext = _calculatorViewModel;
        }

        private void DishNameCombobox_TextChanged(object sender, RoutedEventArgs e)
        {
            string enteredText = DishNameCombobox.Text;
            if (!string.IsNullOrWhiteSpace(enteredText))
            {
                _calculatorViewModel.DishName = enteredText;
                _calculatorViewModel.CreateNewDishNameCommand.Execute(this);
            }    
                
           /* if (!string.IsNullOrWhiteSpace(enteredText))
            {
                bool itemExists = false;
                foreach (Menu item in DishNameCombobox.Items)
                {
                    if (item.Name == enteredText)
                    {
                        itemExists = true;
                        break;
                    }
                }

                if (!itemExists)
                {
                    
                    // Выберите новый элемент
                    //DishNameCombobox.SelectedItem = newItem;
                }

            }*/
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _calculatorViewModel.CalcDataGrid = customTable;
            _calculatorViewModel.ExportToPdfCommand.Execute(this);
        }
    }
}
