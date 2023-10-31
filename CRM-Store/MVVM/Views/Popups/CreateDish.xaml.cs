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
    /// Interaction logic for CreateDish.xaml
    /// </summary>
    public partial class CreateDish : Window
    {

        private CreateDishViewModel _createDishViewModel;
        public CreateDish()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            _createDishViewModel = new CreateDishViewModel();
            DataContext = _createDishViewModel;

            InitializeComponent();
        }
    }
}
