<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Suggestion.aspx.cs" Inherits="InventorySearch.Suggestion" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="css/style.css">

    <script type="text/javascript">
        function pageclose() {         
            window.close();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server" style="margin-left: 80px;">
        <div>
            <table class="detailsTable" cellpadding="0" cellspacing="0">
                <tr>
                    <td colspan="2" style="padding:0;">
                        <div style="background-color:#7d7898;color:White;margin-top:-20px; padding:-1px;width:500px;height:30px;">
                            <h4>Alternate Description Suggestion Form</h4>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="tdBold">
                        Name:
                    </td>
                    <td class="tdDetailsTable">
                        <asp:TextBox ID="txtName" runat="server" Width="225px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvName" runat="server" 
                            ControlToValidate="txtName" ErrorMessage="*" ValidationGroup="Submit"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="tdBold">
                        Your Email:
                    </td>
                    <td class="tdDetailsTable">
                        <asp:TextBox ID="txtEmail" runat="server" Width="225px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvEMail" runat="server" 
                            ControlToValidate="txtEmail" ErrorMessage="*" ValidationGroup="Submit"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="tdBold">
                        Department:
                    </td>
                    <td class="tdDetailsTable">
                        <asp:TextBox ID="txtDepartment" runat="server" Width="225px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvDepartment" runat="server" 
                            ControlToValidate="txtDepartment" ErrorMessage="*" ValidationGroup="Submit"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="tdBold">
                        Phone:
                    </td>
                    <td class="tdDetailsTable">
                        <asp:TextBox ID="txtPhone" runat="server" Width="225px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="tdBold">
                        Item Num:
                    </td>
                    <td class="tdDetailsTable">
                        <asp:Label ID="lblItemNum" runat="server" Width="225px"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="tdBold">
                        Description:
                    </td>
                    <td class="tdDetailsTable">
                        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" 
                            Width="370px" Rows="3"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvDescription" runat="server" 
                            ControlToValidate="txtDescription" ErrorMessage="*" ValidationGroup="Submit"></asp:RequiredFieldValidator>
                        <br />This is a form for suggestions only, order requests will not be processed.
                    </td>
                </tr>
                <tr>
                    <td class="tdBold">
                    </td>
                    <td class="tdDetailsTable">
                        <asp:Button ID="btnSubmit" runat="server" Text="  Submit  " 
                            onclick="btnSubmit_Click" ValidationGroup="Submit" style="margin-left:60px;"/>
                        &nbsp;
                        <asp:Button ID="btnCancel" Text="Cancel" runat="server" onclientclick="pageclose()" />
                        &nbsp;&nbsp;&nbsp;
                    </td>
                </tr>
            </table>
                                            
    </form>
</body>
</html>
