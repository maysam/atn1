<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="theory.aspx.cs" Inherits="ATN.Web.theory" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><asp:Label ID="lblNetworkName" runat="server" /></h2>
    <div id="addnewcontainer">
        <input type="button" value="Add Missing Source" runat="server" id="addnew" />
    </div>
    <br />
    <strong><asp:Label ID="lblFindSource" runat="server" Text="Search for meta analysis by ID or Title" /></strong>
    <br />
    <asp:TextBox ID="txtFindSource" runat="server" /><asp:Button ID="btnFindSource" runat="server" Text="Search" />
    <br />
    <asp:Button ID="btnRandomize" runat="server" OnClick="btnRandomize_Click" Text="Random Sample" />
    <asp:GridView ID="grdFirstLevelSources" runat="server" OnRowDataBound="grdFirstLevelSources_RowDataBound" AutoGenerateColumns="false" Visible="true" EnableViewState="true">
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
                    <asp:LinkButton ID="lnkTitle" runat="server" />
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
                    <asp:Label ID="lblMetaAnalysisHeader" runat="server" Text="Meta-Analysis" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chkMetaAnalysis" runat="server" />
                    <asp:HiddenField ID="hdnSourceId" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    <asp:Label ID="lblContributingHeader" runat="server" Text="Number Contributing" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblContributing" runat="server" />
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
                    <br />
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
