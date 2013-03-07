<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="theory.aspx.cs" Inherits="ATN.Web.theory" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><asp:Label ID="lblNetworkName" runat="server" /></h2>
    <br />
    <asp:GridView ID="grdFirstLevelSources" runat="server" OnRowDataBound="grdFirstLevelSources_RowDataBound" AutoGenerateColumns="false" Visible="true" >
        <Columns>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:LinkButton ID="lnkArticleNameHeader" runat="server" Text="Article Name" />
                    <asp:Image ID="imgArticleNameHeader" runat="server" Visible="false" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblArticleName" runat="server" Text='<% #Bind("ArticleTitle") %>' />
                </ItemTemplate>
                <FooterTemplate>

                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>

                </HeaderTemplate>
                <ItemTemplate>

                </ItemTemplate>
                <FooterTemplate>

                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>

                </HeaderTemplate>
                <ItemTemplate>

                </ItemTemplate>
                <FooterTemplate>

                </FooterTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <br />
    <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClientClick="btnSubmit_OnClientClick" />

</asp:Content>
