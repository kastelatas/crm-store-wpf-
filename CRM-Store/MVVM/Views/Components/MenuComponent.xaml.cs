﻿using CRM_Store.MVVM.ViewModels;
using CRM_Store.MVVM.Views.Components;
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
    /// Interaction logic for MenuComponent.xaml
    /// </summary>
    public partial class MenuComponent : UserControl
    {

        private MenuViewModel _menuViewModel;

        public MenuComponent()
        {
            _menuViewModel = new MenuViewModel();
            DataContext = _menuViewModel;
            InitializeComponent();
        }
    }
}
