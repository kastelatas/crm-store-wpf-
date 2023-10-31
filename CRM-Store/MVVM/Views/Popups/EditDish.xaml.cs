using CRM_Store.MVVM.Models;
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
using System.Windows.Shapes;

namespace CRM_Store.MVVM.Views.Popups
{
    /// <summary>
    /// Interaction logic for EditDish.xaml
    /// </summary>
    public partial class EditDish : Window
    {
        private EditDishViewModal _editDishViewModel;
        public EditDish(Dish dish)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            _editDishViewModel = new EditDishViewModal(dish, this);
            DataContext = _editDishViewModel;

            InitializeComponent();
        }
    }
}
