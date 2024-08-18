using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;
using System.Data;
using System.Text;

namespace TestRadComboBox
{
public class DataManager
{
public List<Product> GetProducts(ProductFilters filters) {
    StringBuilder commandText = new StringBuilder("SELECT product_id, ")
    .Append("product_code, ")
    .Append("product_name ")
    .Append("FROM Products ")
    .AppendFormat("WHERE product_name Like '%{0}%'",filters.ProductName.ToUpper());
    List<Product> productList = null;
    Product p = null;
    using(NpgsqlDataReader reader = PostgreSQLCommand.GetReader(commandText.ToString(),
        null,
        CommandType.Text)){
        productList = new List<Product>();
        while (reader.Read()) {
            p = new Product();
            p.Product_ID = Convert.ToInt32(reader["product_id"]);
            p.ProductCode = reader["product_code"].ToString();
            p.ProductName = reader["product_name"].ToString();
            productList.Add(p);
        } 
    }
    return productList;
}
}
}
