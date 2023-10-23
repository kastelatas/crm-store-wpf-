using CRM_Store.MVVM.Models;
using CRM_Store.MVVM.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM_Store.MVVM.ViewModels
{
    class CalculatorViewModel : ViewModel
    {
        public ObservableCollection<CalculatorTableItem> CalculatorTableData { get; set; }

        public CalculatorViewModel()
        {
            CalculatorTableData = new ObservableCollection<CalculatorTableItem>();
        }
    }
}
