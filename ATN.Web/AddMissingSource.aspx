<%@ Page Title="AddMissingSource" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AddMissingSource.aspx.cs" Inherits="ATN.Web.AddMissingSource" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent" Visible="False">



    <h2><strong>Add A Missing Source</strong></h2>
    <br />
    <br />

    <asp:Label ID="lblTheoryId" runat="server" Text="Theory ID  " style="font-size: medium; font-weight: 700"></asp:Label>
     <br />
    <asp:TextBox ID="txtTheoryId" runat="server" Width="327px"></asp:TextBox>

    <br />
    <br />
    <asp:Label ID="lbltitle" runat="server" Text="Title  " style="font-size: medium; font-weight: 700"></asp:Label>
     <br />
    <asp:TextBox ID="txtTitle" runat="server" Width="327px"></asp:TextBox>

    
    <br />
    <br />

    <asp:Label ID="lblYear" runat="server" Text="Year  " style="font-size: medium; font-weight: 700"></asp:Label>
     <br />
    <asp:TextBox ID="txtYear" runat="server" Width="325px"></asp:TextBox>

    <br />
    <br />
    <asp:CheckBox ID="IsmetAnalysis" Text= "This source is a Meta Analysis" runat="server" OnCheckedChanged="metAnalysis_CheckedChanged" AutoPostBack="true" />

    <br />
    <br />
    

     <asp:Label ID="lblCitations" runat="server" Text="References:  " style="font-size: medium; font-weight: 700" ForeColor="Silver" ></asp:Label>
    <br />
    <asp:DropDownList ID="citations" runat="server" Enabled="False" ForeColor="Silver" BorderColor="Silver" Height="30px" Width="144px"  AutoPostBack="True" Visible="True">
       
    </asp:DropDownList>


    <br />
    <br />


    
        <asp:Label ID="lbladdbyId" runat="server" Text="Add reference by ID"></asp:Label>
        <br />
        <asp:TextBox ID="addById" runat="server" Enabled="false"></asp:TextBox>
        <asp:Button ID="addreference" runat="server" Text="Add" OnClick="Add_citation_to_list"/>


        <br />


        <br />

 
        <asp:HyperLink ID="searchlnk" Target="_blank" runat="server">Search for IDs</asp:HyperLink>


        <br />
        


  
    


    <br />
    <br />
    <asp:Button ID="Submit" runat="server" Text="Submit" OnClick="Submit_Click" />

    


</asp:Content>


