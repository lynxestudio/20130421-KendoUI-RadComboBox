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
