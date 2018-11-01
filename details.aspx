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
    <form id="form1" runat="server" style="margin-left: 80px;">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div id="divMessage" runat="server" style="width:683px;color:#c23b1e;" visible="false">
                    <div style="background-color:#f3f7fb;border:2px solid #96b6da;margin-bottom:10px;">
                        <h5 style="margin:10px;"><asp:Literal ID="litMessage" runat="server"></asp:Literal></h5>
                    </div>
        </div>
    <div>      
         <cc1:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" Width="720px" Height="630" style="margin-right: 0px;">       
            <cc1:TabPanel ID="Details" runat="server" HeaderText="Details" > 
                <ContentTemplate>
                                            <div>
                                                <table class="detailsTable" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td colspan="2" style="padding:0px; ">
                                                            <div style="background-color:#7d7898;color:White;margin-top:-20px;  padding:-1px;width:700px; height:30px;">
                                                                <h4>Item #<asp:Literal ID="litItemNumDetails" runat="server"></asp:Literal> Details</h4>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="tdBold" >
                                                            Primary Description:
                                                        </td>
                                                        <td class="tdDetailsTable">
                                                            <asp:Label ID="lblDescription" runat="server"></asp:Label>
                                                            <br>
                                                            </br>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="tdBold">
                                                            Nursing Description:
                                                        </td>
                                                        <td class="tdDetailsTable">
                                                            <asp:Label ID="lblNursing" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="tdBold">
                                                            Surgery Description:
                                                        </td>
                                                        <td class="tdDetailsTable">
                                                            <asp:Label ID="lblSurgery" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>&nbsp;</td>
                                                        <td class="tdBold">                                                            
                                                            <asp:LinkButton ID="lbtnSuggest" runat="server" onclick="lbtnSuggest_Click" >Suggest a Description</asp:LinkButton>                                                        
                                                            <br>
                                                            </br>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="tdBold">
                                                            Packaging Info:
                                                        </td>
                                                        <td class="tdDetailsTable">
                                                            <asp:GridView ID="gvPackaging" runat="server" CellPadding="4" 
                                                                ForeColor="#333333" Width="95%" Font-Size="0.9em" BorderColor="#CCCCCC">
                                                                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Center"/>
                                                                <FooterStyle BackColor="#7D7898" Font-Bold="True" ForeColor="White" />
                                                                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                                                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                                                <HeaderStyle BackColor="#7D7898" Font-Bold="True" ForeColor="White" />
                                                                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                                            </asp:GridView>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="tdBold">
                                                            Order UOM:                  <!-- Account Category -->
                                                        </td>
                                                        <td class="tdDetailsTable">
                                                            <asp:Label ID="lblOUOM" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="tdBold">
                                                            Stock Item:
                                                        </td>
                                                        <td class="tdDetailsTable">
                                                            <asp:Label ID="lblStock" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="tdBold">
                                                            QTY On Hand:
                                                        </td>
                                                        <td class="tdDetailsTable">
                                                            <asp:Label ID="lblQTY" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2" style="padding:0;">
                                                            <div style="background-color:#7d7898;color:White;margin-top:-20px; padding:-1px;width:700px; height:30px;">
                                                                <h4>Manufacturer Details</h4>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="tdBold">
                                                            Mfg Name:
                                                        </td>
                                                        <td class="tdDetailsTable">
                                                            <asp:Label ID="lblMfgName" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="tdBold">
                                                            Mfg Catalog Number:
                                                        </td>
                                                        <td class="tdDetailsTable">
                                                            <asp:Label ID="lblMfgNum" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>

                </ContentTemplate>                        
             </cc1:TabPanel>


            <cc1:TabPanel ID="Picture" runat="server" HeaderText="Picture" >
                <ContentTemplate> 
                    <asp:Image ID="imgItem" runat="server" Width="100%" BorderWidth="1px"                                                                        
                                  BorderStyle="Solid" BorderColor="#5e5e5e" />
                    <br />
                </ContentTemplate>
            </cc1:TabPanel>                                 
           
         </cc1:TabContainer> 
        <br />
   
    </div>



<!-- <p><small>Source: <cite><a href="https://www.bjcp.org/stylecenter.php">BJCP Style Guidelines</a></cite></small></p> -->
  
  
    </form>
  
  
</body>
</html>
