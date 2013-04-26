<%@ Page Title="Launcher" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="launcher.aspx.cs" Inherits="ATN.Web.launcher" %>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
        <h3><asp:Label ID="lblNetworkName" runat="server" Text="Network Name"></asp:Label></h3>
        <asp:TextBox ID="txtNetworkName" runat="server"></asp:TextBox>
        <br />
        <h3><asp:Label ID="lblNetworkComments" runat="server" Text="Comments"></asp:Label></h3>
        <asp:TextBox ID="txtNetworkComments" runat="server" TextMode="MultiLine" Rows="3" Width="400px" Height="48px" />
        <br />

        <br />
        <br />
        <br />
        <asp:GridView ID="DataSourceGrid" runat="server" AutoGenerateColumns="False" ShowFooter="True" Width="549px" >
            <Columns>
               
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="lbMasId1" runat="server" Text="MS Academic Search ID(s)"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:TextBox ID="txtMasId1" runat="server" ></asp:TextBox>
                    </ItemTemplate>

                   

                    </asp:TemplateField>
               
     

                
                
            </Columns>
       
             </asp:GridView>


   
                 
                  
    <asp:Button ID="btnNewSource" runat="server" Text="Add Another Source" OnCommand="AddNewDataSourceToGrid"/>
        <br />
        <br />
        <asp:Label ID="recrawl" runat="server" Text="Recrawl:" style="font-weight: 700; font-size: medium"></asp:Label>
        &nbsp;&nbsp;
        <asp:DropDownList ID="crawlperiod" runat="server" Height="35px" Width="77px">
            <asp:ListItem Value="1">Daily</asp:ListItem>
            <asp:ListItem Value="7">Weekly</asp:ListItem>
            <asp:ListItem Value="14">Bi-weekly</asp:ListItem>
            <asp:ListItem Value="30">Monthly</asp:ListItem>
            <asp:ListItem Value="120">Quarterly</asp:ListItem>
            <asp:ListItem Value="365">Yearly</asp:ListItem>
        </asp:DropDownList>
        <br />
        <br />
        <span class="auto-style1"><strong>Analysis Engine Options:<br />
        </strong></span><br />
         <asp:CheckBox ID="AEF" runat="server" TextAlign="Left" Text="AEF" OnCheckedChanged="AEF_CheckedChanged" AutoPostBack="True"/>
         <asp:CheckBox ID="ImpactFactor" runat="server" TextAlign="Left" Text="Impact Factor" OnCheckedChanged="ImpactFactor_CheckedChanged" AutoPostBack="True" />       
         <asp:CheckBox ID="TAR" runat="server" TextAlign="Left" Text="TAR" Enabled="False" ForeColor="Silver"  />
         <asp:CheckBox ID="DataMining" runat="server" TextAlign="Left" Text="Data Mining"  />

         <asp:CheckBox ID="Clustring" runat="server" TextAlign="Left" Text="Clustering"  />
        <br />
     

    <asp:Button ID="btnSubmit1" runat="server"  OnCommand="btnSubmit_LaunchCrawler" Text="Start Crawler" />
        <br />
        </asp:Content>
<asp:Content ID="Content1" runat="server" contentplaceholderid="HeadContent">
    <style type="text/css">
        .auto-style1 {
            font-size: medium;
        }
    </style>
</asp:Content>

