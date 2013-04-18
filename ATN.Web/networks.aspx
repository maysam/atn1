<%@ Page Title="Theory Networks" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="networks.aspx.cs" Inherits="ATN.Web.networks" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h1>Theory Networks Overview</h1>
        <asp:GridView ID="grdNetworks" runat="server" AutoGenerateColumns="false" OnRowDataBound="grdNetworks_RowDataBound" Visible="true">
            <Columns>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="lblVisualizationHeader" runat="server" Text="Visualization" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:ImageButton ID="ImgVisualizationLink" runat="server" visible="false"/>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:LinkButton ID="lnkTheoryNameHeader" runat="server" Text="Network Name" />
                        <asp:Image ID="imgTheoryNameHeader" runat="server" Visible="false" ImageAlign="Left" />
                    </HeaderTemplate> 
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkNetwork" runat="server" Text='<% #Bind("TheoryName") %>' />
                        <br />
                        <asp:Label ID="lblNetworkComment" runat="server" Text='<% #Bind("TheoryComment") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:LinkButton ID="lnkDateAddedHeader" runat="server" Text="Date Added" />
                        <asp:Image ID="imgDateDateAddedHeader" runat="server" Visible="false" ImageAlign="Left" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblDate" runat="server" Text='<% #Bind("DateAdded") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:LinkButton ID="lnkLastRunHeader" runat="server" Text="Last Run" />
                        <asp:Image ID="imgLastRunHeader" runat="server" Visible="false" ImageAlign="Left" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblLastRun" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:LinkButton ID="lnkLastEigenfactorHeader" runat="server" Text="Eigenfactor Score" />
                        <asp:Image ID="imgLastEigenfactorHeader" runat="server" Visible="false" ImageAlign="Left" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblLastEigenfactor" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:LinkButton ID="lnkLastMachineLearningHeader" runat="server" Text="Last Machine Learning" />
                        <asp:Image ID="imgLastMachineLearningHeader" runat="server" Visible="false" ImageAlign="Left" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblLastMachineLearning" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:LinkButton ID="lnkStatusHeader" runat="server" Text="Status" />
                        <asp:Image ID="imgStatusHeader" runat="server" Visible="false" ImageAlign="Left" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblStatus" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:LinkButton ID="lnkSecondLevelHeader" runat="server" Text="Size of 2nd Level Network" />
                        <asp:Image ID="imgSecondLevelHeader" runat="server" Visible="false" ImageAlign="Left" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblSecondLevel" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:LinkButton ID="lnkThirdLevelHeader" runat="server" Text="Size of 3rd Level Network" />
                        <asp:Image ID="imgThirdLevelHeader" runat="server" Visible="false" ImageAlign="Left" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblThirdLevel" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:LinkButton ID="lnkTheoryContributingHeader" runat="server" Text="Size of Theory Contributing Network" />
                        <asp:Image ID="imgTheoryContributingHeader" runat="server" Visible="false" ImageAlign="Left" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblTheoryContributing" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEdit" runat="server" Text="Edit Settings" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    
</asp:Content>
