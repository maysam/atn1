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
                        <asp:LinkButton ID="lnkNetworkHeader" runat="server" Text="Network Name" />
                        <asp:Image ID="imgNetworkHeader" runat="server" Visible="false" ImageAlign="Left" />
                    </HeaderTemplate> 
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkNetwork" runat="server" Text='<% #Bind("TheoryName") %>' />
                        <br />
                        <asp:Label ID="lblNetworkComment" runat="server" Text='<% #Bind("TheoryComment") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:LinkButton ID="lnkDateHeader" runat="server" Text="Date Added" />
                        <asp:Image ID="imgDateHeader" runat="server" Visible="false" ImageAlign="Left" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblDate" runat="server" Text='<% #Bind("DateAdded") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:LinkButton ID="lnkLastRun" runat="server" Text="Last Run" />
                        <asp:Image ID="imgLastRunHeader" runat="server" Visible="false" ImageAlign="Left" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblLastRun" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:LinkButton ID="lnkLastEigenfactor" runat="server" Text="Eigenfactor Score" />
                        <asp:Image ID="imgLastEigenfactorHeader" runat="server" Visible="false" ImageAlign="Left" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblLastEigenfactor" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:LinkButton ID="lnkLastMachineLearning" runat="server" Text="Last Machine Learning" />
                        <asp:Image ID="imgLastMachineLearningHeader" runat="server" Visible="false" ImageAlign="Left" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblLastMachineLearning" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:LinkButton ID="lnkStatus" runat="server" Text="Status" />
                        <asp:Image ID="imgStatusHeader" runat="server" Visible="false" ImageAlign="Left" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblStatus" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:LinkButton ID="lnkSecondLevel" runat="server" Text="Size of 2nd Level Network" />
                        <asp:Image ID="imgSecondLevelHeader" runat="server" Visible="false" ImageAlign="Left" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblSecondLevel" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:LinkButton ID="lnkThirdLevel" runat="server" Text="Size of 3rd Level Network" />
                        <asp:Image ID="imgThirdLevelHeader" runat="server" Visible="false" ImageAlign="Left" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblThirdLevel" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:LinkButton ID="lnkTheoryContributing" runat="server" Text="Size of Theory Contributing Network" />
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
