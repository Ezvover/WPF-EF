using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.IO;
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
using System.Data;

namespace laba4
{
    /// <summary>
    /// Логика взаимодействия для Window2.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        public AddWindow()
        {
            InitializeComponent();
        }

        Goods goods = new Goods();

        List<Goods> goodsList = new List<Goods>();
        int freeId = 0;

        public void CheckId()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("usp_GetGoods", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Goods goods = new Goods();
                            goods.id = Convert.ToInt32(reader["id"]);

                            goodsList.Add(goods);
                        }
                    }
                }
            }
        }



        public async Task ToClass()
        {
            CheckId();

            if (goodsList.Count > 0)
            {
                freeId = goodsList.Last().id;
                freeId++;
            }
            else
            {
                freeId = 0;
            }
            goods.id = freeId;
            goods.name = NameTextBox.Text;
            goods.desc = DescTextBox.Text;
            goods.category = CategoryTextBox.Text;

            goods.rate = int.Parse(RateTextBox.Text);

            goods.price = int.Parse(PriceTextBox.Text);

            goods.amount = int.Parse(AmountTextBox.Text);


            string imagePath = $"C:\\Users\\vovas\\Desktop\\repos\\WPF-Shop-Service\\laba4\\bin\\Debug\\net7.0-windows\\images\\{NameTextBox.Text}.jpg";
            string defaultImagePath = $"C:\\Users\\vovas\\Desktop\\repos\\WPF-Shop-Service\\laba4\\bin\\Debug\\net7.0-windows\\images\\img0.jpg";
            byte[] imageBytes;

            if (File.Exists(imagePath))
            {
                goods.other = imagePath;
                imageBytes = File.ReadAllBytes(imagePath);
            }
            else
            {
                goods.other = defaultImagePath;
                imageBytes = File.ReadAllBytes(defaultImagePath);
            }

            goods.picture = imageBytes;

            using (var context = new CodeFirstModel())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        Goods goodsEntity = new Goods
                        {
                            id = goods.id,
                            name = goods.name,
                            desc = goods.desc,
                            category = goods.category,
                            rate = goods.rate,
                            price = goods.price,
                            amount = goods.amount,
                            other = goods.other,
                            picture = goods.picture
                        };

                        Categories categoryEntity = new Categories
                        {
                            id = goods.id,
                            category1 = goods.category
                        };

                        context.Goods.Add(goodsEntity);
                        context.Categories.Add(categoryEntity);
                        await context.SaveChangesAsync(); // Асинхронное сохранение изменений

                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        MessageBox.Show(ex.Message);
                    }
                }
            }

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((int.Parse(RateTextBox.Text) < 0) || (double.Parse(PriceTextBox.Text) < 0) || (int.Parse(AmountTextBox.Text) < 0) || string.IsNullOrWhiteSpace(NameTextBox.Text) || string.IsNullOrWhiteSpace(CategoryTextBox.Text) || string.IsNullOrWhiteSpace(DescTextBox.Text)
                {
                    MessageBox.Show("Ошибка");
                }
                else
                {
                    ToClass();
                    Window parentWindow = Window.GetWindow(this);
                    if (parentWindow != null)
                    {
                        parentWindow.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
