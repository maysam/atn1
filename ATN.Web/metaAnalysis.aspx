<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="metaAnalysis.aspx.cs" Inherits="ATN.Web.metaAnalysis" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><asp:Label ID="lblNetworkName" runat="server" /></h2><h2><asp:LinkButton ID="lnkNetworkName" runat="server" OnClick="lnkNetworkName_OnClick"/></h2>
    <h3><asp:Label ID="lblMetaAnalysisName" runat="server" /></h3>
    <br />
    <asp:GridView ID="grdFirstLevelSources" runat="server" OnRowDataBound="grdFirstLevelSources_RowDataBound" AutoGenerateColumns="false" Visible="true" ShowFooter="false">
        <Columns>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblAuthorsHeader" runat="server" Text="Authors"/>
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
                    <asp:Label ID="lblSourceIdHeader" runat="server" Text="Source ID" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblSourceId" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblContributingHeader" runat="server" Text="Contributing?" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:RadioButtonList ID="rblContributing" runat="server" RepeatDirection="Vertical" TextAlign="Right"> 
                        <asp:ListItem Text="Yes" Value="Yes"/>
                        <asp:ListItem Text="No" Value="No" />
                        <asp:ListItem Text="Unknown" Value="Unknown" />
                    </asp:RadioButtonList>
                    <asp:HiddenField ID="hdnRadioValue" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblYearHeader" runat="server" Text="Year"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblYear" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblEigenfactorHeader" runat="server" Text="Eigenfactor Score"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblEigenfactor" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblDepthHeader" runat="server" Text="Citation Level"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblDepth" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblJournalHeader" runat="server" Text="Journal"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblJournal" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblPredictionHeader" runat="server" Text="Machine Learning Prediction" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblPrediction" runat="server" />
                    <asp:Label ID="lblPredictionScore" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <br />
    <asp:Button ID="btnPrevious" runat="server" Text="Previous Page" OnClick="btnPrevious_OnClick" />
    <asp:Button ID="btnSubmit" runat="server" Text="Save" OnClick="btnSubmit_OnClick" />
    <asp:Button ID="btnNext" runat="server" Text="Next Page" OnClick="btnNext_OnClick" />

</asp:Content>
