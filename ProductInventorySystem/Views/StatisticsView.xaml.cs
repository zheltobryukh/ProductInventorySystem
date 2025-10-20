using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
using ProductInventorySystem.Data;

namespace ProductInventorySystem.Views
{
    public partial class StatisticsView : Page
    {
        public StatisticsView()
        {
            InitializeComponent();
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                // Общая статистика
                string queryTotal = "SELECT COUNT(*), AVG(Price), SUM(Price * Quantity) FROM Products";
                using (SqlCommand cmd = new SqlCommand(queryTotal, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtTotal.Text = reader.IsDBNull(0) ? "0" : reader.GetInt32(0).ToString();
                        txtAvgPrice.Text = reader.IsDBNull(1) ? "0" : reader.GetDecimal(1).ToString("N2");
                        txtTotalValue.Text = reader.IsDBNull(2) ? "0" : reader.GetDecimal(2).ToString("N2");
                    }
                }

                // Статистика по категориям
                string queryStats = @"SELECT c.CategoryName, COUNT(p.ProductID), AVG(p.Price)
                                     FROM Products p
                                     JOIN Categories c ON p.CategoryID = c.CategoryID
                                     GROUP BY c.CategoryName";

                List<CategoryStat> stats = new List<CategoryStat>();
                using (SqlCommand cmd = new SqlCommand(queryStats, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stats.Add(new CategoryStat
                        {
                            Category = reader.GetString(0),
                            Count = reader.GetInt32(1),
                            AvgPrice = reader.GetDecimal(2).ToString("N2")
                        });
                    }
                }

                dgStats.ItemsSource = stats;
            }
        }

        private void BtnRefresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadStatistics();
        }

        public class CategoryStat
        {
            public string Category { get; set; }
            public int Count { get; set; }
            public string AvgPrice { get; set; }
        }
    }
}