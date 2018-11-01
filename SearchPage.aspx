<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchPage.aspx.cs" Inherits="InventorySearch.SearchPage" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
      
    <title></title>


    <style type="text/css">
        body
        {
            margin:15px;
            background:#d4cce3;
            background-image:url("images/bg_gradient.gif"); 
            background-repeat:repeat-x;
            color:#594b63;
            font-family:Tahoma;
            font-size:.75em;
        }
        .divider
        {
            background-image:url("images/divider.gif");
            background-repeat:repeat-x;
            width:790px;
            height:2px;
            margin:10px 0 10px 0;
        }
        .dividerSmall
        {
            background-image:url("images/divider.gif");
            background-repeat:repeat-x;
            width:100%;
            height:2px;
            margin:10px 0 10px 0;
        }
        .auto-style3 {
            height: 33px;
        }
        .auto-style4 {
            width: 5px;
        }
        .auto-style6 {
            width: 262px;
        }
        .auto-style8 {
            width: 237px;
            height: 33px;
        }
        .auto-style9 {
            width: 237px;
        }
        .auto-style10 {
            width: 195px;
            height: 33px;
        }
        .auto-style11 {
            width: 195px;
        }
        .auto-style12 {
            width: 53%;
            float: left;
        }
    </style>
</head>
<body>
    
    <script type="text/javascript">
        function OpenPopup() {            
            var item = CommandArgument
            showModalDialog("details.aspx", "", "dialogHeight=100,dialogWidth = 200");
             
              return false;
          }
    </script>       

    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"> </asp:ScriptManager>
        <table cellpadding="0" cellspacing="0">
            <tr>
                <td style="width:275px;">
                    <asp:Image ID="imgUWLogo" runat="server" ImageUrl="~/images/hmcLogo.gif" />
                </td>
                <td id="tdRightHeader" runat="server" style="width:515px;" align="right">
                    <asp:HyperLink ID="hypTopLink1" runat="server" CssClass="upperLinks" NavigateUrl="https://hmc.uwmedicine.org/" Target="_blank">HMC Home</asp:HyperLink> &nbsp;|&nbsp;
                    <asp:HyperLink ID="hypTopLink3" runat="server" CssClass="upperLinks" NavigateUrl="https://info.medical.washington.edu/" Target="_blank">Online Info</asp:HyperLink> &nbsp;|&nbsp;
                    <asp:HyperLink ID="hypTopLink4" runat="server" CssClass="upperLinks" NavigateUrl="http://myuw.washington.edu/" Target="_blank">My UW</asp:HyperLink><br />
                  <%--   <div style="float:right;margin-top:15px;"><asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/btn_search.gif" /></div>
                    <div style="float:right;margin:15px 3px 0 0;"><asp:TextBox ID="TextBox1" runat="server"></asp:TextBox></div>--%>
                </td>
                
            </tr>
        </table>
        <div id="divDivider" runat="server" class="divider"></div>
     <asp:Label ID="lblPageName" runat="server" style="margin-left:130px" Font-Size="X-Large" Font-Bold="true">HMC Materials Management - Item Catalog</asp:Label>
        <table cellpadding="0" cellspacing="0" style="margin-top:15px;">
            <tr>  
                <td valign="top" style="padding-right:5px;">
                    &nbsp;</td>
               
            </tr>
        </table>
<!-- ------------------ -->
    
        <div id="divMessage" runat="server" style="width:683px;color:#c23b1e; margin-left:55px" visible="false">
                    <div style="background-color:#f3f7fb;border:2px solid #96b6da;margin-bottom:10px;">
                        <h5 style="margin:10px;"><asp:Literal ID="litMessage" runat="server"></asp:Literal></h5>
                    </div>
                </div>
        <table width="645px" style="margin-left:70px">
                                    <tr class="controls" style="font-weight:bold">
                                        <td colspan="1">
                                            Search Text:</td>
                                        <td>
                                            Search Filter:</td>
                                        <td>
                                            Search By:</td>
                                        <td class="auto-style4">
                                            &nbsp;</td>
                                        <td>Location</td>  <!-- this aligns the Search button all the way to the right -->
                                         <td align="right">
                                            <asp:ImageButton ID="ibtnSearch" runat="server" 
                                                ImageUrl="~/images/btn_search.gif" onclick="ibtnSearch_Click" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtSearch" runat="server" CssClass="controls" Width="150px" TabIndex="1"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlSearchFilter" runat="server" CssClass="controls" Width="130px">
                                                <asp:ListItem Value="Partial">Partial Words</asp:ListItem>
                                                <asp:ListItem Value="Exact">Exact Phrase</asp:ListItem>
                                                <asp:ListItem Value="Any" Selected="True">Any of the Words</asp:ListItem>
                                                <asp:ListItem Value="All">All Words</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlSearchBy" runat="server" CssClass="controls" Width="150px">
                                                <asp:ListItem Value="t1.DESCR" Selected="True">Description &amp; Comments</asp:ListItem>
                                                <asp:ListItem Value="t1.ITEM_NO">Item #</asp:ListItem>
                                                <asp:ListItem Value="MFR.NAME">Manufacturer Name</asp:ListItem>
                                                <asp:ListItem Value="t1.CTLG_NO">Manufacturer Catalog #</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td class="auto-style4">
                                            &nbsp;</td>
                                        <td>
                                            <asp:DropDownList ID="ddlLocation" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlLocation_SelectedIndexChanged" CausesValidation="False">
                                                <asp:ListItem Value="ALL" Selected="True">All</asp:ListItem>
                                                <asp:ListItem Value="MED">Med Stores</asp:ListItem>
                                             <%--   <asp:ListItem Value="ANGIO">Angio</asp:ListItem>     --%>
                                                <asp:ListItem Value="ORSTORES">OR Stores</asp:ListItem>
                                                <asp:ListItem Value="ORIMPLANT">OR Implant</asp:ListItem>
                                                <asp:ListItem Value="WHSE">Warehouse</asp:ListItem>
                                                 <asp:ListItem Value="SEC">Standards</asp:ListItem>      
                                             <%--   <asp:ListItem Value="NON-STOCK">Non-Stock</asp:ListItem>   --%>
                                            </asp:DropDownList>
                                        </td>
                                        <td align="right">
                                            <asp:ImageButton ID="ibtnClear" runat="server"  Height="19px" Width="64px"
                                                ImageUrl="~/images/btn_clear.gif" onclick="ibtnClear_Click" />
                                        </td>
                                    </tr>
                                </table>
        <table style="background-color:#eeeeee;width:646px; font-weight:bold; margin-left:70px"  class="controls" cellpadding="4">                                   
                                     <tr class="controls" style="font-weight:bold">
                                        <td colspan="1" class="auto-style8">
                                            Items Per Page</td>
                                        <td class="auto-style10"></td>
                                           <!-- Location</td> -->
                                         
                                        <td class="auto-style3">
                                            Results</td>                                        
                                        <td class="auto-style3"></td>
                                    </tr>
            <%--You are viewing page <%=gvItems.PageIndex + 1%> of <%=gvItems.PageCount%>--%>
                                   <tr>                                                                                                                        
                                         <td class="auto-style9">
                                             <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
                                                 <asp:ListItem>5</asp:ListItem>
                                                 <asp:ListItem>10</asp:ListItem>
                                                 <asp:ListItem>20</asp:ListItem>
                                                 <asp:ListItem Selected="True">50</asp:ListItem>
                                                 <asp:ListItem>100</asp:ListItem>
                                             </asp:DropDownList>
                                        </td>
                                        <td class="auto-style11"></td>
                                        <td class="auto-style6" >
                                            <!--Results:  align="right"--> <asp:Literal ID="litResults" runat="server"></asp:Literal> item(s)
                                        </td>
                                    </tr>
                                </table>
        <div style="width: 527px">
            <asp:GridView ID="gvItemList" runat="server" Height="266px" Width="646px" style="margin-right: 0px; margin-left: 70px" CellPadding="4" ForeColor="#D7F3DB" GridLines="None" AllowPaging="True" ShowFooter="True" EnableTheming="False" OnPageIndexChanging="gvItemList_PageIndexChanging" PageSize="50" AutoGenerateColumns="False" DataSourceID="SqlDataSource1">
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    <asp:TemplateField HeaderText="Item#" SortExpression="ITEM_NO">
                        <ItemTemplate>
                           <asp:LinkButton ID="lbtnItemNum" runat="server" Text='<%# Bind("[Item #]") %>' CssClass="gvControls" 
                                onClick="lbtnItemNum_Click" CommandArgument='<%# Eval("[Item #]") + ";" + Eval("Location") %>'></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Description" SortExpression="DESCR">                       
                        <ItemTemplate>
                            <asp:LinkButton ID="lbtnDesc" runat="server" Text='<%# Bind("Description") %>' CssClass="gvControls"
                              onclick="lbtnItemNum_Click" CommandArgument='<%# Eval("[Item #]") + ";" + Eval("Location") %>'></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Catalog#" SortExpression="CTLG_NO">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbtnCtlg" runat="server" Text='<%# Bind("[Catalog #]") %>' CssClass="gvControls"
                                onclick="lbtnItemNum_Click" CommandArgument='<%# Eval("[Item #]") + ";" + Eval("Location") %>'></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                                        
                    <asp:CheckBoxField DataField="Stocked" HeaderText="Stocked" 
                        SortExpression="STOCKED" ItemStyle-Width="100px" 
                        ItemStyle-HorizontalAlign="Center" >
                        <ItemStyle HorizontalAlign="Center" Width="100px"></ItemStyle>
                    </asp:CheckBoxField>

                    <asp:TemplateField HeaderText="Location" SortExpression="LOC">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbtnLoc" runat="server" Text='<%# Bind("Location") %>' CssClass="gvControls" 
                               onclick="lbtnItemNum_Click" CommandArgument='<%# Eval("[Item #]") + ";" + Eval("Location")%>'></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>


                    <asp:TemplateField HeaderText="Image">
                        <ItemTemplate>
                            <asp:ImageButton ID="imgItem" runat="server" ImageUrl='<%# GetImageUrl(Eval("[Item #]") + ";" + Eval("Location"))%>' Width="96px" BorderWidth="1px" 
                                  BorderStyle="Solid" BorderColor="#5e5e5e" onclick="imgItem_Click" CommandArgument='<%# Eval("[Item #]") + ";" + Eval("Location") %>'/>
                        </ItemTemplate>
                    </asp:TemplateField>


                    <asp:TemplateField HeaderText="Price" SortExpression="PRICE">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbtnPrice" runat="server" Text='<%# Bind("Price") %>' CssClass="gvControls" 
                                                        onclick="lbtnItemNum_Click" CommandArgument='<%# Eval("[Item #]") + ";" + Eval("Location") %>'></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>


                </Columns>
                                <EditRowStyle BackColor="#999999" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" Font-Size="Large" />
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <SortedAscendingCellStyle BackColor="#E9E7E2" />
                <SortedAscendingHeaderStyle BackColor="#506C8C" />
                <SortedDescendingCellStyle BackColor="#FFFDF8" />
                <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
            </asp:GridView>
             <asp:HiddenField ID="hfdItemTarget" runat="server" />
           
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:amc_userConnectionString %>" SelectCommand=""/>
        </div>
    </form>
</body>
</html>
