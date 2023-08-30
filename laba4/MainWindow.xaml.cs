using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;

namespace laba4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> strList = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            ToGrid();
          
            MainGrid.SelectedIndex = 0; // Устанавливаем первый элемент как выбранный
            MainGrid.IsReadOnly = true;
        }

        public void ToGrid()
        {
            using (var context = new CodeFirstModel())
            {
                try
                {
                    var goodsList = context.Goods
                        .ToList() // Fetch all data to memory
                        .Select(g => new Goods
                        {
                            id = g.id,
                            name = g.name,
                            desc = g.desc,
                            category = g.category,
                            rate = g.rate ?? 0,
                            price = g.price ?? 0,
                            amount = g.amount ?? 0,
                            other = File.Exists(g.other) ? new Uri(g.other).ToString() : null
                        })
                        .ToList(); // Materialize the projection

                    MainGrid.ItemsSource = goodsList;

                    strList.Clear();
                    for (int i = 0; i < goodsList.Count; i++)
                    {
                        strList.Add(goodsList[i].category);
                    }
                    var strList2 = strList.Distinct();
                    FilterBOx.ItemsSource = strList2;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        MessageBox.Show("Inner Exception: " + ex.InnerException.Message);
                    }
                    else
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) // delete button
        {
            if (MainGrid.SelectedItem != null)
            {
                var selectedObject = MainGrid.SelectedItem as Goods;

                using (var context = new CodeFirstModel())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var goodsToDelete = context.Goods.FirstOrDefault((System.Linq.Expressions.Expression<Func<Goods, bool>>)(g => g.id == selectedObject.id));

                            if (goodsToDelete != null)
                            {
                                if (goodsToDelete.Category1 != null)
                                {
                                    context.Categories.Remove(goodsToDelete.Category1);
                                }

                                context.Goods.Remove(goodsToDelete);
                                context.SaveChanges();
                                dbContextTransaction.Commit();

                                ToGrid();
                            }
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }

            MainGrid.SelectedIndex = 0; // Set the first item as selected
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int minPrice = int.Parse(SearchText.Text);
                int maxPrice = int.Parse(SearchText2.Text);

                using (var context = new CodeFirstModel())
                {
                    var filteredGoods = context.Goods
                        .Where(g => g.price >= minPrice && g.price <= maxPrice)
                        .ToList();

                    MainGrid.ItemsSource = filteredGoods;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            MainGrid.SelectedIndex = 0; // Set the first item as selected
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) // categorysort
        {
            if (FilterBOx.SelectedItem != null)
            {
                string selectedCategory = FilterBOx.Text;

                using (var context = new CodeFirstModel())
                {
                    try
                    {
                        var goodsInCategory = context.Goods
                            .Where(g => g.category == selectedCategory)
                            .ToList();

                        MainGrid.ItemsSource = goodsInCategory;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

                MainGrid.SelectedIndex = 0; // Set the first item as selected
            }
        }


        private void Button_Click_2(object sender, RoutedEventArgs e) // add window
        {
            AddWindow add = new AddWindow();
            add.Show();
            
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            ToGrid();
            FilterBOx.SelectedItem= null;
            SearchText.Clear();
            SearchText2.Clear();
            SearchText3.Clear();
            MainGrid.SelectedIndex = 0; // Устанавливаем первый элемент как выбранный
        }

        private void SearchButton2_Click(object sender, RoutedEventArgs e) // partSearch
        {
            string searchText = SearchText3.Text;

            using (var context = new CodeFirstModel())
            {
                var searchResults = context.Goods
                    .Where(g =>
                        g.name.Contains(searchText) ||
                        g.desc.Contains(searchText) ||
                        g.other.Contains(searchText) ||
                        g.category.Contains(searchText) ||
                        SqlFunctions.StringConvert((double)g.price).Contains(searchText) ||
                        SqlFunctions.StringConvert((double)g.amount).Contains(searchText))
                    .ToList();

                MainGrid.ItemsSource = searchResults;
            }

            MainGrid.SelectedIndex = 0; // Set the first item as selected
        }


        private void Button_Click_3(object sender, RoutedEventArgs e) // edit
        {
            try
            {
                File.Delete("TempEditGood.xml");
            }
            catch
            {

            }
            if (MainGrid.SelectedItem!= null) 
            {
                var selectedObject = MainGrid.SelectedItem as Goods;
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Goods));
                using (FileStream stream = new FileStream($"TempEditGood.xml", FileMode.OpenOrCreate))
                {
                    xmlSerializer.Serialize(stream, selectedObject);
                }

                EditWindow editWindow = new EditWindow();
                editWindow.Show();
            }
        }
        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainGrid.SelectedItem != null)
            {
                int index = MainGrid.SelectedIndex;
                if (index < MainGrid.Items.Count - 1)
                {
                    MainGrid.SelectedIndex = index + 1;
                    MainGrid.ScrollIntoView(MainGrid.SelectedItem);
                }
            }
        }
    }
}
