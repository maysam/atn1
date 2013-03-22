<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="theory.aspx.cs" Inherits="ATN.Web.theory" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><asp:Label ID="lblNetworkName" runat="server" /></h2>
    <br />
    <asp:GridView ID="grdFirstLevelSources" runat="server" OnRowDataBound="grdFirstLevelSources_RowDataBound" AutoGenerateColumns="false" Visible="true" >
        <Columns>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:LinkButton ID="lnkTitleHeader" runat="server" Text="Title" />
                    <asp:Image ID="imgTitleHeader" runat="server" Visible="false" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:LinkButton ID="lnkTitle" runat="server" Text='<% #Bind("ArticleTitle") %>' OnClientClick="lnkTitle_OnClientClick"/>
                    <asp:Label ID="lblTitle" runat="server" Text='<% #Bind("ArticleTitle") %>' />
                </ItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtTitle" runat="server" />
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblSourceIdHeader" runat="server" Text="Source ID" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblSourceId" runat="server" />
                </ItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtSourceId" runat="server" />
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblMetaAnalysisHeader" runat="server" Text="Meta-Analysis" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chkMetaAnalysis" runat="server" />
                </ItemTemplate>
                <FooterTemplate>

                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblContributingHeader" runat="server" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblContributing" runat="server" />
                    <asp:RadioButtonList ID="rblContributing" runat="server" RepeatDirection="Vertical" >
                        <asp:ListItem Text="Yes" Value="Yes"/>
                        <asp:ListItem Text="Unknown" Value="Unknown"/>
                    </asp:RadioButtonList>
                </ItemTemplate>
                <FooterTemplate>
                    <asp:CheckBox ID="chkContributing" runat="server" />
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblYearHeader" runat="server" Text="Year"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblYear" runat="server" />
                </ItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtYear" runat="server" />
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblEigenfactorHeader" runat="server" Text="Eigenfactor Score"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblEigenfactor" runat="server" />
                </ItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtEigenfactor" runat="server" />
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblCitationLevelHeader" runat="server" Text="Citation Level"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblCitationLevel" runat="server" />
                </ItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtCitationLevel" runat="server" />
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblAuthorsHeader" runat="server" Text="Authors"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblAuthors" runat="server" />
                </ItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtAuthors" runat="server" />
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblJournalHeader" runat="server" Text="Journal"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblJournal" runat="server" />
                </ItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtJournal" runat="server" />
                </FooterTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <br />
    <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClientClick="btnSubmit_OnClientClick" />

</asp:Content>
