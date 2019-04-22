<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Upload.aspx.cs" Inherits="ScrapMaricopa.Pages.Upload" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Upload Orders</title>
    <style type="text/css">
        .auto-style1 {
            height: 388px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="auto-style1">
    
        <asp:TextBox ID="txtordertype" runat="server" CssClass="txtuser" Height="150px" TextMode="MultiLine" Width="900px"></asp:TextBox>
        <br />
        <asp:Label ID="Label1" runat="server" Font-Names="Verdana" Text="Format: OrderNo | Address | State | County"></asp:Label>
        <br />
        <br />
        <asp:Button ID="BtnSubmit" runat="server" OnClick="BtnSubmit_Click" Text="Submit" Width="68px" />
        <asp:Label ID="lblstatus" runat="server" Font-Names="Verdana"></asp:Label>
    
    </div>
    </form>
</body>
</html>
