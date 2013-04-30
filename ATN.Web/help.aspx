<%@ Page Title="Help" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="help.aspx.cs" Inherits="ATN.Web.help" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">

    <h2>What is Microsoft Academic Search ID? Where do I find it?</h2>
    <br/>
    <p>Every Electronic paper has its own ID. Some has more than one ID but two papers can't share the same ID 
        The steps to find a paper ID are as follows:</p> 
   
    <h3>1. Search for your paper. </h3> 
    <asp:Image ID="Image1" ImageUrl="Images/ATN1.jpg" runat="server" Height="397px" style="margin-left:10px; margin-top:10px" Width="690px" />
     <br />
    <h3>2. Once you found your paper, click on it:</h3>
    <asp:Image ID="Image2" ImageUrl="Images/ATN2.jpg" runat="server" Height="397px" style="margin-left:10px; margin-top:10px" Width="690px" />
    <br /> 
    <h3>3. The paper ID would probably be shown in the paper URL right. It's the digits right afte Publications </h3>
    <asp:Image ID="Image3" ImageUrl="Images/ATN3.jpg" runat="server" Height="397px" style="margin-left:10px; margin-top:10px" Width="690px" />
    <br /> 

</asp:Content>
