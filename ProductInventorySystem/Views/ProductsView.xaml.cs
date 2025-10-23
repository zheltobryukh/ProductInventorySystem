using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
using ProductInventorySystem.Data;
using ProductInventorySystem.Models;

namespace ProductInventorySystem.Views
{
    public partial class ProductsView : Page
    {
        public ProductsView()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            List<Product> products = new List<Product>();

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT p.ProductID, p.Name, c.CategoryName, p.Price, p.Quantity
                                FROM Products p
                                JOIN Categories c ON p.CategoryID = c.CategoryID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            ProductID = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            CategoryName = reader.GetString(2),
                            Price = reader.GetDecimal(3),
                            Quantity = reader.GetInt32(4)
                        });
                    }
                }
            }

            dgProducts.ItemsSource = products;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string name = Microsoft.VisualBasic.Interaction.InputBox("Название товара:", "Добавление");
            if (string.IsNullOrEmpty(name)) return;

            string priceStr = Microsoft.VisualBasic.Interaction.InputBox("Цена:", "Добавление");
            if (!decimal.TryParse(priceStr, out decimal price)) return;

            string qtyStr = Microsoft.VisualBasic.Interaction.InputBox("Количество:", "Добавление");
            if (!int.TryParse(qtyStr, out int quantity)) return;

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Products (Name, CategoryID, Price, Quantity) VALUES (@Name, 1, @Price, @Qty)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@Qty", quantity);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Товар добавлен!");
            LoadProducts();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgProducts.SelectedItem is Product product)
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM Products WHERE ProductID = @ID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", product.ProductID);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Товар удалён!");
                LoadProducts();
            }
        }
    }
}