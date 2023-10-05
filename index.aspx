<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="tx_checkboxes.index" %>

<%@ Register assembly="TXTextControl.Web, Version=32.0.1200.500, Culture=neutral, PublicKeyToken=6b83fe9a75cfb638" namespace="TXTextControl.Web" tagprefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TX Text Control Sample: Using Checkboxes</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager> 
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional"> 
        </asp:UpdatePanel> 

        <cc1:TextControl ID="TextControl1" runat="server" />
    
        <script type="text/javascript">

            // attach the 'textFieldClicked' event
            TXTextControl.addEventListener("textFieldClicked", function (e) {
                fieldClicked(e.fieldName, e.fieldType, e.typeName);
            });

            // do an AJAX postback on the UpdatePanel
            function fieldClicked(fieldName, fieldType, typeName) {
                __doPostBack('<%= UpdatePanel1.ClientID %>', fieldName);
            }

        </script>
    </div>
    </form>
</body>
</html>
