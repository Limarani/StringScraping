<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="ScrapMaricopa.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:TextBox ID="TextBox1" runat="server"  Style="width: 239px; margin: 0px;padding: 0;border-top: 0px;border-left: 0px;border-right: 0px;border-bottom: 1px solid black;" ></asp:TextBox>
    
        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="TextBox1" ErrorMessage="RegularExpressionValidator" ValidationExpression="^\d{6}(-\d{4})?$"></asp:RegularExpressionValidator>
    
    </div>
    </form>
</body>
</html>
