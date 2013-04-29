<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="metaAnalysis.aspx.cs" Inherits="ATN.Web.metaAnalysis" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><asp:Label ID="lblNetworkName" runat="server" /></h2><h2><asp:LinkButton ID="lnkNetworkName" runat="server" /></h2>
    <h3><asp:Label ID="lblMetaAnalysisName" runat="server" /></h3><asp:Button ID="btnMarkAsMetaAnalysis" runat="server" Text="Mark as Meta Analysis" />
    <br />
    <asp:GridView ID="grdFirstLevelSources" runat="server" OnRowDataBound="grdFirstLevelSources_RowDataBound" AutoGenerateColumns="false" Visible="true" ShowFooter="false">
        <Columns>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Linkbutton ID="lnkAuthorsHeader" runat="server" Text="Authors"/>
                    <asp:Image ID="imgAuthorsHeader" runat="server" Visible="false" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblAuthors" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:LinkButton ID="lnkTitleHeader" runat="server" Text="Title" />
                    <asp:Image ID="imgTitleHeader" runat="server" Visible="false" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblTitle" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:LinkButton ID="lnkSourceIdHeader" runat="server" Text="Source ID" />
                    <asp:Image ID="imgSourceIdHeader" runat="server" Visible="false" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblSourceId" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblContributingHeader" runat="server" Text="Contributing?" CssClass="ContributingHeader"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:RadioButtonList ID="rblContributing" runat="server" RepeatDirection="Vertical" TextAlign="Right" CssClass="MetaAnalysisRadio"> 
                        <asp:ListItem Text="Yes" Value="Yes"/>
                        <asp:ListItem Text="No" Value="No" />
                        <asp:ListItem Text="?" Value="Unknown" />
                    </asp:RadioButtonList>
                    <asp:HiddenField ID="hdnSourceId" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:LinkButton ID="lnkYearHeader" runat="server" Text="Year"/>
                    <asp:Image ID="imgYearHeader" runat="server" Visible="false" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblYear" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:LinkButton ID="lnkAEFHeader" runat="server" Text="Eigenfactor Score"/>
                    <asp:Image ID="imgAEFHeader" runat="server" Visible="false" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblEigenfactor" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:LinkButton ID="lnkDepthHeader" runat="server" Text="Citation Level"/>
                    <asp:Image ID="imgDepthHeader" runat="server" Visible="false" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblDepth" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:LinkButton ID="lnkJournalHeader" runat="server" Text="Journal"/>
                    <asp:Image ID="imgJournalHeader" runat="server" Visible="false" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblJournal" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:LinkButton ID="lnkPredictionHeader" runat="server" Text="Machine Learning Prediction" />
                    <asp:Image ID="imgpredictionProbabilityHeader" runat="server" Visible="false" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblPrediction" runat="server" />
                    <asp:Label ID="lblPredictionScore" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <br />
    <asp:Button ID="btnPrevious" runat="server" Text="Previous Page" />
    <asp:Button ID="btnSubmit" runat="server" Text="Save" />
    <asp:Button ID="btnNext" runat="server" Text="Next Page" />

</asp:Content>
