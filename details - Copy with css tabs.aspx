<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="details.aspx.cs" Inherits="InventorySearch.details" %>

  <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>   
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >


<head runat="server">
  <meta charset="UTF-8">
  <title></title>
  
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/normalize/5.0.0/normalize.min.css">

  <link rel='stylesheet prefetch' href='https://fonts.googleapis.com/css?family=Overpass:300,400,600,800'>

      <link rel="stylesheet" href="css/style.css">
    <style type="text/css">
       
        
        .dividerSmall
        {
            background-image:url("images/divider.gif");
            background-repeat:repeat-x;
            width:100%;
            height:2px;
            margin:10px 0 10px 0;
        }
        </style>
  
</head>

<body>
    <form id="form1" runat="server">
<div>      
        <cc1:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0">
       
            <cc1:TabPanel ID="TabPanel1" runat="server" HeaderText="TabPanel1">
                <ContentTemplate>
                    <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox><br />
                    <asp:Button ID="Button1" runat="server" Text="Button" />
                </ContentTemplate>
            </cc1:TabPanel>
           
            <cc1:TabPanel ID="TabPanel2" runat="server" HeaderText="TabPanel2">
                <ContentTemplate> 
                    <asp:Image ID="imgItem" runat="server" ImageUrl= "~/catalog_images/" + ViewState["item"] + ".jpg" Width="416px" BorderWidth="1px"                                                                        
                                  BorderStyle="Solid" BorderColor="#5e5e5e" />
                    <br />
                    <asp:Button ID="Button2" runat="server" Text="Button 2" />
                </ContentTemplate>
            </cc1:TabPanel>                                 
           
        </cc1:TabContainer> 
        <br />
   
    </div>
    </div>

  


<!-- <p><small>Source: <cite><a href="https://www.bjcp.org/stylecenter.php">BJCP Style Guidelines</a></cite></small></p> -->
  
  
        
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
  
  
        
    </form>
  
  
</body>
</html>
