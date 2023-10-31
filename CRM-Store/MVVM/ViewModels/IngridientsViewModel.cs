using CRM_Store.Core.Infastructure.Commands;
using CRM_Store.MVVM.Models;
using CRM_Store.MVVM.ViewModels.Base;
using CRM_Store.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CRM_Store.MVVM.ViewModels
{
    internal class IngridientsViewModel : ViewModel
    {
        public ObservableCollection<IngridientName> IngridientNames { get; set; }
        public string IngridientName { get; set; } = string.Empty;

        #region Command
        public ICommand AddIngridientCommand { get; }
        public ICommand RemoveIngridientCommand { get; }

        private void OnAddIngridientCommandExecuted(object p)
        {
            if (IngridientName != string.Empty)
            {
                IngridientNames.Add(
                    new IngridientName
                    {
                        ID = IngridientNames.Count,
                        Name = IngridientName
                    }
                );
                GlobalStore.Instance.SaveToJsonFile(IngridientNames, "ingridients.json");
                IngridientName = "";
                OnPropertyChanged(nameof(IngridientName));
            }
        }
        private bool CanAddIngridientCommand(object p) => true;

        private void OnRemoveIngridientCommandExecuted(object p)
        {
            if (p is IngridientName item)
            {
                IngridientNames.Remove(item);
                GlobalStore.Instance.SaveToJsonFile(IngridientNames, "ingridients.json");
            }
        }
        private bool CanRemoveIngridientCommand(object p) => true;
        #endregion
        public IngridientsViewModel() 
        {
            AddIngridientCommand = new LambdaCommand(OnAddIngridientCommandExecuted, CanAddIngridientCommand);
            RemoveIngridientCommand = new LambdaCommand(OnRemoveIngridientCommandExecuted, CanRemoveIngridientCommand);

            IngridientNames = GlobalStore.Instance.IngridientNames;
        }
    }
}
