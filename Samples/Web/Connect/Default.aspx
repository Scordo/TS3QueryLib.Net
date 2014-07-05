<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Connect.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Connection Sample</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label runat="server" ForeColor="Red" ID="ErrorLabel" /><br />

        Server Address:<br />
        <asp:TextBox runat="server" ID="ServerAddressTextBox" Text="localhost" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="ServerAddressTextBox" ErrorMessage="*" Display="Dynamic" EnableClientScript="true" ForeColor="Red"/><br />
        <br />
        
        Query Port:<br />
        <asp:TextBox runat="server" ID="ServerPortTextBox" Text="10011" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="ServerPortTextBox" ErrorMessage="*" Display="Dynamic" EnableClientScript="true" ForeColor="Red" />
        <asp:RangeValidator runat="server" ControlToValidate="ServerPortTextBox" MinimumValue="1" MaximumValue="65535" Type="Integer" ErrorMessage="*" Display="Dynamic" EnableClientScript="true" ForeColor="Red" />
        <br />

        <asp:Button runat="server" ID="ConnectButton" Text="Connect" OnClick="ConnectButton_OnClick" /><br />
        <asp:Label runat="server" ID="ResultLabel" />
    </div>
    </form>
</body>
</html>
