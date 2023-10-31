using CRM_Store.Core.Infastructure.Commands;
using CRM_Store.MVVM.Models;
using CRM_Store.MVVM.ViewModels.Base;
using CRM_Store.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Windows.Xps.Packaging;
using PdfSharp.Xps;
using PdfSharp.Pdf;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using SpreadsheetLight;
using Syncfusion.Calculate;
using DocumentFormat.OpenXml.Wordprocessing;
using HorizontalAlignmentValues = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues;
using Microsoft.Win32;

namespace CRM_Store.MVVM.ViewModels
{
    class CalculatorViewModel : ViewModel
    {
        private UserControl _window;
        private ObservableCollection<CalculatorTableItem> _calculatorTableData;
        public ObservableCollection<CalculatorTableItem> CalculatorTableData
        {
            get { return _calculatorTableData; }
            set
            {
                _calculatorTableData = value;
                OnPropertyChanged(nameof(CalculatorTableData));
            }
        }
        public ObservableCollection<CalculatorTableItem> _ingridientsForOneServing { get; set; }
        public ObservableCollection<IngridientName> IngridientNames { get; set; }
        public ObservableCollection<QuantityUnit> QuantityUnits { get; set; }
        public ObservableCollection<Dish> Menu { get; set; }

        public DataGrid CalcDataGrid { get; set; }

        private double _dishPrice;
        public double DishPrice
        {
            get { return _dishPrice; }
            set
            {
                _dishPrice = value;
                OnPropertyChanged(nameof(_dishPrice));
            }
        }

        private string _dishName;
        public string DishName
        {
            get { return _dishName; }
            set
            {
                _dishName = value;
                Application.Current.Properties["DishName"] = _dishName;
                OnPropertyChanged(nameof(_dishName));
            }
        }

        private double _servingsCount;
        public double ServingsCount
        {
            get { return _servingsCount; }
            set
            {
                _servingsCount = value;
                Application.Current.Properties["ServingsCount"] = _servingsCount;
                RecountForOutherServings();
                OnPropertyChanged(nameof(_servingsCount));
            }
        }

        private double _oneServing;
        public double OneServing
        {
            get { return _oneServing; }
            set
            {
                _oneServing = value;
                OnPropertyChanged(nameof(_oneServing));
            }
        }

        private double _dishWeight;
        public double DishWeight
        {
            get { return _dishWeight; }
            set
            {
                _dishWeight = value;
                OnPropertyChanged(nameof(_dishWeight));
            }
        }
        private void RecountForOutherServings()
        {
            if (ServingsCount > 0 && _calculatorTableData.Count() > 0)
            {
                for (int i = 0; i < _ingridientsForOneServing.Count(); i++)
                {
                    var ingridientForOneServing = _ingridientsForOneServing[i];
                    _calculatorTableData[i].Quantity = (Math.Round((double.Parse(ingridientForOneServing.Quantity) * ServingsCount), 2)).ToString();
                }

                double _dishCost = 0.0;
                double _dishWeight = 0.0;

                for (int i = 0; i < CalculatorTableData.Count(); i++)
                {
                    var ingridient = CalculatorTableData[i];

                    double localQuantity = double.Parse(ingridient.Quantity);

                    switch (ingridient.QuantityUnit)
                    {
                        case "кг":
                            if (localQuantity >= 1)
                            {
                                _dishWeight += (localQuantity * 1000);
                            }
                            else
                            {
                                _dishWeight += localQuantity;
                            }
                            break;

                        case "шт":
                            break;

                        case "л":
                            if (localQuantity >= 1)
                            {
                                _dishWeight += (localQuantity * 1000);
                            }
                            else
                            {
                                _dishWeight += localQuantity;
                            }
                            break;

                        default:
                            break;
                    }

                    _dishCost += localQuantity * double.Parse(ingridient.Price);
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                };

                CalculatorTableData = JsonSerializer.Deserialize<ObservableCollection<CalculatorTableItem>>(JsonSerializer.Serialize(CalculatorTableData, options), options);

                OnPropertyChanged(nameof(CalculatorTableData));
            }

        }

        #region Commands
        public ICommand AddNewRowCommand { get; }
        public ICommand RemoveRowCommand { get; }
        public ICommand DeleteAllRowsCommand { get; }
        public ICommand CalculateDishCommand { get; }
        public ICommand SaveDishCommand { get; }
        public ICommand CreateNewDishNameCommand { get; }
        public ICommand ExportToPdfCommand { get; }

        private void OnAddNewRowCommandExecuted(object p)
        {
            CalculatorTableData.Add(new CalculatorTableItem());
        }
        private bool CanAddNewRowCommand(object p) => true;

        private void OnRemoveRowCommandExecuted(object p)
        {
            if (p is CalculatorTableItem item)
            {
                CalculatorTableData.Remove(item);
            }
        }
        private bool CanRemoveRowCommand(object p) => true;

        private void OnDeleteAllRowsCommandExecuted(object p)
        {
            CalculatorTableData.Clear();
        }
        private bool CanDeleteAllRowsCommand(object p) => true;

        private void OnCreateNewDishNameCommandExecuted(object p)
        {
            foreach (var dish in Menu)
            {
                if (dish.Name == DishName)
                {
                    CalculatorTableData = dish.Ingridients;
                    _ingridientsForOneServing.Clear();
                    foreach (var ingridient in dish.Ingridients)
                    {
                        _ingridientsForOneServing.Add(ingridient);
                    }
                    DishPrice = dish.DishCost;
                    ServingsCount = dish.ServingsCount;
                    OneServing = dish.OneServing;
                    OnPropertyChanged(nameof(DishPrice));
                    OnPropertyChanged(nameof(ServingsCount));
                    OnPropertyChanged(nameof(OneServing));
                    OnPropertyChanged(nameof(CalculatorTableData));
                    break;
                } 
                else
                {
                    CalculatorTableData = new ObservableCollection<CalculatorTableItem>();
                    DishPrice = 0.0;
                    ServingsCount = 0.0;
                    OneServing = 0.0;
                    OnPropertyChanged(nameof(DishPrice));
                    OnPropertyChanged(nameof(ServingsCount));
                    OnPropertyChanged(nameof(OneServing));
                    OnPropertyChanged(nameof(CalculatorTableData));
                }
            }
        }
        private bool CanCreateNewDishNameCommand(object p) => true;

        private void OnCalculateDishCommandExecuted(object p)
        {
            _dishPrice = 0.0;
            _dishWeight = 0.0;
            _oneServing = 0;

            foreach (var item in CalculatorTableData)
            {
                double localQuantity = double.Parse(item.Quantity);

                switch (item.QuantityUnit)
                {
                    case "кг":
                        if(localQuantity >= 1)
                        {
                            _dishWeight += (localQuantity * 1000);
                        } 
                        else
                        {
                            _dishWeight += localQuantity;
                        }
                        break;

                    case "шт":
                        break;

                    case "л":
                        if (localQuantity >= 1)
                        {
                            _dishWeight += (localQuantity * 1000);
                        }
                        else
                        {
                            _dishWeight += localQuantity;
                        }
                        break;

                    default:
                    break;
                }

                _dishPrice += Math.Round((localQuantity * double.Parse(item.Price)), 1);
            }

            if (_servingsCount > 0)
            {
                _oneServing = Math.Round((_dishPrice / _servingsCount), 2);
            }

            OnPropertyChanged(nameof(OneServing));
            OnPropertyChanged(nameof(DishPrice));
            OnPropertyChanged(nameof(DishWeight));
        }
        private bool CanCalculateDishCommand(object p) => true;

        private void OnSaveDishCommandExecuted(object p)
        {

            if (string.IsNullOrEmpty(_dishName) ||  _servingsCount == 0 || CalculatorTableData.Count() == 0)
            {
                MessageBox.Show("Данные не верные!");
            }
            else
            {
                ObservableCollection<CalculatorTableItem> IngridientsForOneServing = new ObservableCollection<CalculatorTableItem>();

                foreach (var item in CalculatorTableData.ToList())
                {
                    if (item.Quantity != null)
                    {
                        double quantity = Math.Round((double.Parse(item.Quantity) / _servingsCount), 2);

                        IngridientsForOneServing.Add(new CalculatorTableItem
                        {
                            Name = item.Name,
                            Quantity = quantity.ToString(),
                            QuantityUnit = item.QuantityUnit,
                            Price = item.Price,
                        });
                    }
                    else
                    {
                        MessageBox.Show("Данные не верные!");
                    }
                   
                }

                double dishWeightForOneServing = 0.0;
                double dishPriceForOneServing = 0.0;

                foreach (var item in IngridientsForOneServing)
                {
                    double localQuantity = double.Parse(item.Quantity);

                    switch (item.QuantityUnit)
                    {
                        case "кг":
                            if (localQuantity >= 1)
                            {
                                dishWeightForOneServing += (localQuantity * 1000);
                            }
                            else
                            {
                                dishWeightForOneServing += localQuantity;
                            }
                            break;

                        case "шт":
                            break;

                        case "л":
                            if (localQuantity >= 1)
                            {
                                dishWeightForOneServing += (localQuantity * 1000);
                            }
                            else
                            {
                                dishWeightForOneServing += localQuantity;
                            }
                            break;

                        default:
                            break;
                    }

                    dishPriceForOneServing += localQuantity * double.Parse(item.Price);
                }

                var currentDish = Menu.FirstOrDefault(i => i.Name == DishName);

                if (currentDish != null)
                {
                    currentDish.Ingridients = IngridientsForOneServing;

                    int indexToReplace = GlobalStore.Instance.Menu.IndexOf(GlobalStore.Instance.Menu.FirstOrDefault(item => item.ID == currentDish.ID));

                    if (indexToReplace >= 0)
                    {
                        GlobalStore.Instance.Menu.RemoveAt(indexToReplace);
                        GlobalStore.Instance.Menu.Insert(indexToReplace, currentDish);
                    }
                }
                else
                {
                    Menu.Add(new Dish
                        {
                            ID = Menu.Count + 1,
                            Name = _dishName,
                            DishPrice = Math.Round(dishPriceForOneServing, 2),
                            DishCost = Math.Round(dishPriceForOneServing, 2),
                            DishWeight = Math.Round(dishWeightForOneServing, 2),
                            OneServing = Math.Round((dishPriceForOneServing / 1), 2),
                            ServingsCount = 1,
                            Markup = 0,
                            Ingridients = IngridientsForOneServing,
                        }
                    );
                }

                GlobalStore.Instance.SaveToJsonFile(GlobalStore.Instance.Menu, "menu.json");
                MessageBox.Show("Блюдо добавленно в меню!");
            }
        }
        private bool CannSaveDishCommand(object p) => true;

        private void OnExportToExelCommandExecuted(object p)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.DefaultExt = "xlsx";

            DataGrid dataGrid = CalcDataGrid;
            if (saveFile.ShowDialog() == true)
            {
                try
                {
                    using (SLDocument Doc = new SLDocument())
                    {
                        int currentRow = 1;
                        int currentCol = 1;

                        Doc.SetCellValue(1, 1, "");
                        Doc.SetCellValue(2, 1, "Название блюда:");
                        Doc.SetCellValue(2, 2, DishName);
                        Doc.SetCellValue(3, 1, "Кол-во порций:");
                        Doc.SetCellValue(3, 2, ServingsCount);

                        currentRow += 4;

                        Doc.SetCellValue(currentRow, currentCol, dataGrid.Columns[currentCol - 1].Header.ToString());
                        Doc.SetCellValue(currentRow, currentCol + 1, dataGrid.Columns[currentCol].Header.ToString());
                        Doc.SetCellValue(currentRow, currentCol + 2, dataGrid.Columns[currentCol + 1].Header.ToString());
                        Doc.SetCellValue(currentRow, currentCol + 3, dataGrid.Columns[currentCol + 2].Header.ToString());

                        currentRow++;

                        foreach (CalculatorTableItem item in dataGrid.Items)
                        {

                            Doc.SetCellValue(currentRow, currentCol, item.Name);
                            Doc.SetCellValue(currentRow, currentCol + 1, double.Parse(item.Quantity));
                            Doc.SetCellValue(currentRow, currentCol + 2, item.QuantityUnit);
                            Doc.SetCellValue(currentRow, currentCol + 3, double.Parse(item.Price));

                            currentRow++;
                        }
                        SLStyle styleBold = new SLStyle();
                        styleBold.Font.Bold = true;
                        styleBold.Font.FontSize = 13;

                        SLStyle styleItalic = new SLStyle();
                        styleItalic.Font.Italic = true;
                        styleItalic.SetHorizontalAlignment(HorizontalAlignmentValues.Center);

                        SLStyle styleCenter = new SLStyle();
                        styleCenter.SetHorizontalAlignment(HorizontalAlignmentValues.Center);

                        SLStyle styleBorder = new SLStyle();
                        styleBorder.SetBottomBorder(BorderStyleValues.Thin, SLThemeColorIndexValues.Hyperlink);
                        styleBorder.SetRightBorder(BorderStyleValues.Thin, SLThemeColorIndexValues.Hyperlink);
                        styleBorder.SetLeftBorder(BorderStyleValues.Thin, SLThemeColorIndexValues.Hyperlink);
                        styleBorder.SetTopBorder(BorderStyleValues.Thin, SLThemeColorIndexValues.Hyperlink);

                        Doc.SetCellStyle(2, 1, styleBold);
                        Doc.SetCellStyle(2, 2, styleItalic);
                        Doc.SetCellStyle(3, 1, styleBold);
                        Doc.SetCellStyle(3, 2, styleCenter);
                        Doc.SetRowStyle(5, styleBold);

                        for (int col = 1; col <= dataGrid.Columns.Count; col++)
                        {
                            Doc.SetColumnWidth(col, 20);
                        }

                        for (int row = 1; row <= dataGrid.Items.Count + 1; row++)
                        {

                            for (int col = 1; col <= dataGrid.Items.Count; col++)
                            {
                                if (row + 4 == 5 && col == 1)
                                {
                                    Doc.SetCellStyle(row + 4, col, styleCenter);
                                }
                                else if (col != 1)
                                {
                                    Doc.SetCellStyle(row + 4, col, styleCenter);
                                }

                                if (col < 5)
                                {
                                    Doc.SetCellStyle(row+4, col, styleBorder);
                                }
                            }
                        }

                        Doc.SaveAs(saveFile.FileName);
                        MessageBox.Show("Файл сохранен");

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Что-то пошло не так! Возможно вы забыли закрыть файл");
                }
            }
        }
        private bool CanExportToExelCommand(object p) => true;
        #endregion

        public CalculatorViewModel(UserControl window = null)
        {
            _window = window;
            Menu = GlobalStore.Instance.Menu;

            AddNewRowCommand = new LambdaCommand(OnAddNewRowCommandExecuted, CanAddNewRowCommand);
            RemoveRowCommand = new LambdaCommand(OnRemoveRowCommandExecuted, CanRemoveRowCommand);
            DeleteAllRowsCommand = new LambdaCommand(OnDeleteAllRowsCommandExecuted, CanDeleteAllRowsCommand);
            CalculateDishCommand = new LambdaCommand(OnCalculateDishCommandExecuted, CanCalculateDishCommand);
            SaveDishCommand = new LambdaCommand(OnSaveDishCommandExecuted, CannSaveDishCommand);
            CreateNewDishNameCommand = new LambdaCommand(OnCreateNewDishNameCommandExecuted, CanCreateNewDishNameCommand);
            ExportToPdfCommand = new LambdaCommand(OnExportToExelCommandExecuted, CanExportToExelCommand);

            _ingridientsForOneServing = new ObservableCollection<CalculatorTableItem>();
            CalculatorTableData = GlobalStore.Instance.CalculatorTableData;
            IngridientNames = GlobalStore.Instance.IngridientNames;
            QuantityUnits = GlobalStore.Instance.QuantityUnits;

            if (Application.Current.Properties.Contains("DishName"))
            {
                DishName = Application.Current.Properties["DishName"] as string;
            }
            if (Application.Current.Properties.Contains("ServingsCount"))
            {
                ServingsCount = double.Parse(Application.Current.Properties["ServingsCount"].ToString());
            }

        }
    }
}
