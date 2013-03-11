﻿<%@ Page Title="Launcher" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="launcher.aspx.cs" Inherits="ATN.Web.launcher" %>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
        <h3><asp:Label ID="lblNetworkName" runat="server" Text="Network Name"></asp:Label></h3>
        <asp:TextBox ID="txtNetworkName" runat="server"></asp:TextBox>
        <br />
        <h3><asp:Label ID="lblNetworkComments" runat="server" Text="Comments"></asp:Label></h3>
        <asp:TextBox ID="txtNetworkComments" runat="server" TextMode="MultiLine" Rows="3" Width="400" />
        <br />
        <asp:GridView ID="DataSourceGrid" runat="server" AutoGenerateDeleteButton="false" AutoGenerateColumns="false" ShowFooter="true">
            <Columns>
                <asp:TemplateField >
                    <HeaderTemplate>
                        <asp:Label ID="lblPaperName" runat="server" Text="Paper Name"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:TextBox ID="txtPaperName" runat="server" Width="182px"></asp:TextBox>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Button ID="btnNewSource" runat="server" Text="Add Another Source" OnCommand="AddNewDataSourceToGrid"/>
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="lblSocialCitationIndexId" runat="server" Text="Social Citation Index ID"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:TextBox ID="txtSocialCitationIndexId" runat="server" ReadOnly="true"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="lblMsAcademicSearchId" runat="server" Text="MS Academic Search ID"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:TextBox ID="txtMsAcademicSearchId" runat="server"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="lblPubMedId" runat="server" Text="PubMed ID"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:TextBox ID="txtPubMedId" runat="server" ReadOnly="true"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <br />
        <asp:Button ID="btnSubmit" runat="server"  OnCommand="btnSubmit_LaunchCrawler" Text="Start Crawler" />
        <br />
</asp:Content>