using CRM_Store.MVVM.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CRM_Store.Services
{
    class GlobalStore
    {
        private static GlobalStore instance;
        public ObservableCollection<CalculatorTableItem> CalculatorTableData { get; set; }
        public ObservableCollection<IngridientName> IngridientNames { get; set; }
        public ObservableCollection<QuantityUnit> QuantityUnits { get; set; }
        public ObservableCollection<Dish> Menu { get; set; }

        public static GlobalStore Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GlobalStore();
                }
                return instance;
            }
        }

        public void SaveToJsonFile<T>(T data, string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            string jsonData = JsonConvert.SerializeObject(data);

            File.WriteAllText(filePath, jsonData);
        }

        public T ReadFromJsonFile<T>(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            T data;

            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                data = JsonConvert.DeserializeObject<T>(jsonData);
            }
            else
            {
                if (typeof(T).IsClass)
                {
                    data = Activator.CreateInstance<T>();
                }
                else
                {
                    data = default(T);
                }
            }

            return data;
        }
        private GlobalStore() 
        {
            Menu = ReadFromJsonFile<ObservableCollection<Dish>>("menu.json");
            IngridientNames = ReadFromJsonFile<ObservableCollection<IngridientName>>("ingridients.json");
            CalculatorTableData = new ObservableCollection<CalculatorTableItem>();
           /* {
                new CalculatorTableItem{
                    Name = "Картошка",
                    Price = "12.5",
                    Quantity = "0.3",
                    QuantityUnit = "кг",
                },
                new CalculatorTableItem{
                    Name = "Лук",
                    Price = "12",
                    Quantity = "0.2",
                    QuantityUnit = "кг",
                },
                new CalculatorTableItem{
                    Name = "Морковка",
                    Price = "10",
                    Quantity = "0.1",
                    QuantityUnit = "кг",
                },
                new CalculatorTableItem{
                    Name = "Сливки",
                    Price = "200",
                    Quantity = "0.15",
                    QuantityUnit = "л",
                },
                new CalculatorTableItem{
                    Name = "Грибы",
                    Price = "100",
                    Quantity = "0.4",
                    QuantityUnit = "кг",
                },
                new CalculatorTableItem{
                    Name = "Мясо Свинина",
                    Price = "70",
                    Quantity = "1.1",
                    QuantityUnit = "кг",
                },
            };
*/
           

            QuantityUnits = new ObservableCollection<QuantityUnit>()
            {
                new QuantityUnit
                {
                    ID = 1,
                    Name = "кг"
                },
                new QuantityUnit
                {
                    ID = 2,
                    Name = "шт"
                },
                new QuantityUnit
                {
                    ID = 3,
                    Name = "л"
                },
            };
        }

    }
}
