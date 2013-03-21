<%@ Page Title="Launcher" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="launcher.aspx.cs" Inherits="ATN.Web.launcher" %>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
        <h3><asp:Label ID="lblNetworkName" runat="server" Text="Network Name"></asp:Label></h3>
        <asp:TextBox ID="txtNetworkName" runat="server"></asp:TextBox>
        <br />
        <h3><asp:Label ID="lblNetworkComments" runat="server" Text="Comments"></asp:Label></h3>
        <asp:TextBox ID="txtNetworkComments" runat="server" TextMode="MultiLine" Rows="3" Width="400px" Height="24px" />
        <br />
        <asp:GridView ID="DataSourceGrid" runat="server" AutoGenerateColumns="False" ShowFooter="True" Width="673px" Height="94px">
            <Columns>
               
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="lbMasId1" runat="server" Text="MS Academic Search ID 1"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:TextBox ID="txtMasId1" runat="server" ></asp:TextBox>
                    </ItemTemplate>

                    <FooterTemplate>
                        <asp:Button ID="btnNewSource" runat="server" Text="Add Another Source" OnCommand="AddNewDataSourceToGrid"/>
                    </FooterTemplate>

                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="lbMasId2" runat="server" Text="MS Academic Search ID 2"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:TextBox ID="txtMasId2" runat="server"></asp:TextBox>
                    </ItemTemplate>

                     
                </asp:TemplateField>
                
                
                
                <asp:ButtonField ButtonType="Button" CommandName="AddNewDataSourcId" Text="Add Another MSAS ID" />
                
                
                
            </Columns>
        </asp:GridView>
        <asp:CheckBox ID="recrawl" runat="server" Text="Re-Crawl:" OnCheckedChanged="recrawl_CheckedChanged" />
        <asp:DropDownList ID="crawlperiod" runat="server" Height="37px">
            <asp:ListItem Value="1">Daily</asp:ListItem>
            <asp:ListItem Value="7">Weekly</asp:ListItem>
            <asp:ListItem Value="14">Bi-weekly</asp:ListItem>
            <asp:ListItem Value="30">Monthly</asp:ListItem>
            <asp:ListItem Value="120">Quarterly</asp:ListItem>
            <asp:ListItem Value="365">Yearly</asp:ListItem>
        </asp:DropDownList>
        <br />
        <br />
        <asp:Button ID="btnSubmit" runat="server"  OnCommand="btnSubmit_LaunchCrawler" Text="Start Crawler" OnClick="btnSubmit_Click" />
        <br />
</asp:Content>
