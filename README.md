# Usando el control RadComboBox de Telerik con PostgreSQL

Aunque ASP.NET ofrece una extensa colección de Web Controls que proporcionan al desarrollador una programación fácil, estandarizada y orientada a objetos, también existe como parte del ecosistema de ASP.NET la suite de controles Telerik que complementan y ofrecen más funcionalidad que los Web Controls predeterminados incluidos en el conjunto estándar de ASP.NET.

Del conjunto de controles de la suite Telerik, mostraré brevemente el uso del control RadComboBox de Telerik, el cual extiende la funcionalidad proporcionada por el control DropDownList de ASP.NET al tener no solamente las características de este control sino capacidades adicionales como:la carga de datos de diferentes colecciones de datos, API con funciones de servidor y de cliente, el uso de templates para mejorar la aparencia visual y la propiedad de cargar items bajo demanda para mejorar el rendimiento.

A continuación un sencillo ejemplo para mostrar dos características básicas de este control:

La carga de items bajo demanda (Load on Demand)
El uso de templates para la aparencia visual de los items.
El ejemplo consiste de una página ASP.NET que consulta una tabla de products que contiene una lista de productos clínicos de una base de datos myinvoices, la tabla tiene el siguiente esquema:


Consultamos la lista de productos



El ejemplo esta compuesto por 6 clases: DataManager, Logger, PostgreSQLCommand,PostgreSQLDataBase, Product, ProductFilters y dos páginas ASP.NET: Default.aspx y Default2.aspx.

El código de la clase DataManager

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
public List GetProducts(ProductFilters filters) {
    StringBuilder commandText = new StringBuilder("SELECT product_id, ")
    .Append("product_code, ")
    .Append("product_name ")
    .Append("FROM Products ")
    .AppendFormat("WHERE product_name Like '%{0}%'",filters.ProductName.ToUpper());
    List productList = null;
    Product p = null;
    using(NpgsqlDataReader reader = PostgreSQLCommand.GetReader(commandText.ToString(),
        null,
        CommandType.Text)){
        productList = new List();
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
El código de la clase Logger

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TestRadComboBox
{
public class Logger
{
public static void LogWriteError(string s)
{
using (FileStream stream = new FileStream("log.txt", 
    FileMode.OpenOrCreate, FileAccess.ReadWrite))
{
    StreamWriter sw = new StreamWriter(stream);
    sw.BaseStream.Seek(0, SeekOrigin.End);
    sw.Write(s);
    sw.Flush();
    sw.Close();
}

}
}
}
El código de la clase PostgreSQLCommand

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Npgsql;

namespace TestRadComboBox
{
internal sealed class PostgreSQLCommand
{

internal static NpgsqlDataReader GetReader(string commandText,
NpgsqlParameter[] parameters,System.Data.CommandType cmdtype){
NpgsqlDataReader resp = null;
try{
 NpgsqlConnection conn = PostgreSQLDataBase.GetInstance().GetConnection();
 using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, conn))
 {
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);
  resp = cmd.ExecuteReader(CommandBehavior.CloseConnection);
 }
 return resp;
}catch(Exception ex){
 Logger.LogWriteError(ex.Message);
 throw ex;
}
}
}
}
El código de la clase PostgreSQLDataBase
<pre>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace TestRadComboBox
{
internal sealed class PostgreSQLDataBase
{
static NpgsqlConnection _conn = null;
static PostgreSQLDataBase Instance = null;
string ConnectionString = null;
private PostgreSQLDataBase()
{
    try
    {
        ConnectionString = ConfigurationManager.
            ConnectionStrings["myinvoices"].ConnectionString;
    }
    catch (Exception ex)
    {
        Logger.LogWriteError(ex.Message);
        throw ex;
    }
}
private static void CreateInstance()
{
    if (Instance == null)
    { Instance = new PostgreSQLDataBase(); }
}
public static PostgreSQLDataBase GetInstance()
{
    if (Instance == null)
        CreateInstance();
    return Instance;
}
public NpgsqlConnection GetConnection()
{
    try
    {
        _conn = new NpgsqlConnection(ConnectionString);
        _conn.Open();
        return _conn;
    }
    catch (Exception ex)
    {
        Logger.LogWriteError(ex.Message);
        throw ex;
    }
}
}
}
</pre>
El código de la clase Product
<pre>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestRadComboBox
{
public class Product
{
    public int Product_ID { set; get; }
    public string ProductCode { set; get; }
    public string ProductName { set; get; }
}
}
</pre>
El código de la clase ProductFilters

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestRadComboBox
{
public struct ProductFilters
{
    public string ProductName { set; get; }

}
}
El código de la página ASP.NET Default.aspx
<pre>
<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="TestRadComboBox" %>
<%@ Register assembly="Telerik.Web.UI" 
namespace="Telerik.Web.UI" 
tagprefix="telerik" %>
<!DOCTYPE html
PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<title></title>
</head>
<script runat="server">

    protected void cmbProducts_OnItemsRequested(object sender,
        RadComboBoxItemsRequestedEventArgs args) {
        DataManager dm = new DataManager();
        ProductFilters filter = new ProductFilters { 
            ProductName = args.Text
        };

        if (cmbProducts.DataSource == null)
        {
            cmbProducts.DataSource = dm.GetProducts(filter);
            cmbProducts.DataValueField = "Product_ID";
            cmbProducts.DataTextField = "ProductName";
            cmbProducts.DataBind();
        }
    }
</script>
<body>
<form id="form1" runat="server">
<telerik:RadScriptManager ID="RadScriptManager1" runat="server">
    <Scripts>
<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js">
</asp:ScriptReference>
<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js">
</asp:ScriptReference>
<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js">
</asp:ScriptReference>
</Scripts>
</telerik:RadScriptManager>
<div>
Please select a product:
    <telerik:RadComboBox ID="cmbProducts" 
    DropDownWidth="333px"
    OnItemsRequested="cmbProducts_OnItemsRequested"
    EnableLoadOnDemand="true"
    Runat="server">
    </telerik:RadComboBox>
</div>
</form>
</body>
</html>
</pre>
La Carga de items bajo demanda
Una de las capacidades más utilizadas del control RadComboBox es la capacidad de cargar las opciones o items bajo demanda es decir construir la consulta en base a las coincidencias de los items y lo teclado por el usuario, esta capacidad se programa con el metódo cmbProducts_OnItemsRequested
<pre>
protected void cmbProducts_OnItemsRequested(object sender,
        RadComboBoxItemsRequestedEventArgs args) {
        DataManager dm = new DataManager();
        ProductFilters filter = new ProductFilters { 
            ProductName = args.Text
        };

        if (cmbProducts.DataSource == null)
        {
            cmbProducts.DataSource = dm.GetProducts(filter);
            cmbProducts.DataValueField = "Product_ID";
            cmbProducts.DataTextField = "ProductName";
            cmbProducts.DataBind();
        }
    }
    </pre>
En este metódo se contruye una clase ProductFilter que contine el texto tecleado por el usuario en el RadComboBox, una vez creada esta clase se pasa como argumento al metódo GetProducts de la clase DataManager dentro de este metódo se construye la consulta SQL que filtra mediante un comando LIKE la consulta de la tabla Products hacia la lista (List) que sirve de DataSource para el RadComboBox.
<pre>
//Here we past the filter in order to build the SQL Query
public List GetProducts(ProductFilters filters) {
    StringBuilder commandText = new StringBuilder("SELECT product_id, ")
    .Append("product_code, ")
    .Append("product_name ")
    .Append("FROM Products ")
    .AppendFormat("WHERE product_name Like '%{0}%'",filters.ProductName.ToUpper());
    List productList = null;
    Product p = null;
    using(NpgsqlDataReader reader = PostgreSQLCommand.GetReader(commandText.ToString(),
        null,
        CommandType.Text)){
        productList = new List();
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
</pre>
Al ejecutar la página vemos el resultado como en las siguientes imágenes:



Se realiza la búsqueda de las coincidencias entre lo tecleado por el usuario y el listado de productos



Es característica mejora el rendimiento de los formularios.





Uso de un template para los items.

Uno de los aspectos visuales más llamativos de este control Telerik es el utilizar un template para personalizar la aparencia de la lista de elementos seleccionables, como ejemplo en la siguiente página Default2.aspx utilizamos la misma funcionalidad de la página Default.aspx con la diferencia de utilizar una tabla como template para los items.

El código de la página ASP.NET Default2.aspx que muestra el uso de un template para los items.
<pre>
<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="TestRadComboBox" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" 
tagprefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<title></title>
</head>
<script runat="server">


protected void cmbProducts_OnItemsRequested(object sender,
    RadComboBoxItemsRequestedEventArgs args) {
    DataManager dm = new DataManager();
    ProductFilters filter = new ProductFilters { 
        ProductName = args.Text
    };

    if (cmbProducts.DataSource == null)
    {
        cmbProducts.DataSource = dm.GetProducts(filter);
        cmbProducts.DataValueField = "Product_ID";
        cmbProducts.DataTextField = "ProductName";
        cmbProducts.DataBind();
    }
}
</script>
<body>
<form id="form1" runat="server">
<telerik:RadScriptManager ID="RadScriptManager1" runat="server">
<Scripts>
<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js">
</asp:ScriptReference>
<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js">
</asp:ScriptReference>
<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js">
</asp:ScriptReference>
</Scripts>
</telerik:RadScriptManager>
<div>
Please select a product:
<telerik:RadComboBox ID="cmbProducts" 
DropDownWidth="333px"
OnItemsRequested="cmbProducts_OnItemsRequested"
EnableLoadOnDemand="true"
HighlightTemplatedItems="true"
Runat="server">
<HeaderTemplate>
<table cellspacing="3" width="99%">
<tr>
<td width="44%">Code</td>
<td>Name</td>
</tr>
</table>
</HeaderTemplate>
<ItemTemplate>
<table cellspacing="3" width="99%">
<tr>
<td width="44%"><%#DataBinder.Eval(Container.DataItem, "ProductCode")%></td>
<td><%#DataBinder.Eval(Container.DataItem, "ProductName")%></td>
</tr>
</table>
</ItemTemplate>
</telerik:RadComboBox>
</div>
</form>
</body>
</html>
</pre>
Al ejecutar la página Default2.aspx, veremos el resultado como en la siguiente imágen:


Por último el fragmento de código del archivo Web.conf del ejemplo, en donde se pone la cadena de conexión.

  <connectionStrings>
    <add connectionString="Server=127.0.0.1;Port=5432;Database=myinvoices;User ID=postgres;Password=Pa$$W0rd" name="myinvoices" />
  </connectionStrings>
