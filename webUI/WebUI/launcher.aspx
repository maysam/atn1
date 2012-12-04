<%@ Page Title="Launcher" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="launcher.aspx.cs" Inherits="launcher" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">



    <p>
        <asp:Label ID="lblNetworkName" runat="server" Text="Network Name"></asp:Label>
        <br />
        <asp:TextBox ID="txtNetworkName" runat="server"></asp:TextBox>
        <br />
        <asp:Label ID="lblPaperName" runat="server" Text="Paper Name"></asp:Label>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;
        <asp:Label ID="lblSocialCitationIndexId" runat="server" Text="Social Citation Index ID"></asp:Label>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Label ID="lblMsAcademicSearchId" runat="server" Text="MS Academic Search ID"></asp:Label>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Label ID="lblPubMedId" runat="server" Text="PubMed ID"></asp:Label>
        <br />
        <asp:TextBox ID="txtPaperName" runat="server" Width="182px"></asp:TextBox>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="txtSocialCitationIndexId" runat="server"></asp:TextBox>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="txtMsAcademicSearchId" runat="server"></asp:TextBox>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="txtPubMedId" runat="server"></asp:TextBox>
        <br />
        <asp:Button ID="btnNewSource" runat="server" Text="Add Another Source" />
        <br />
        <br />
        <asp:Button ID="btnSubmit" runat="server" Text="Start Crawler" />
        <br />
    </p>



</asp:Content>

