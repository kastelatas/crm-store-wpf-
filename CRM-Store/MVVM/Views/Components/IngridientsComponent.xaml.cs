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
    /// Interaction logic for IngridientsComponent.xaml
    /// </summary>
    public partial class IngridientsComponent : UserControl
    {
        private IngridientsViewModel _ingridientsViewModel;
        public IngridientsComponent()
        {
            _ingridientsViewModel = new IngridientsViewModel();
            DataContext = _ingridientsViewModel;

            InitializeComponent();
        }
    }
}
