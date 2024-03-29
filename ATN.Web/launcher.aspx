﻿<%@ Page Title="Launcher" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="launcher.aspx.cs" Inherits="ATN.Web.launcher" %>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <asp:Literal ID="confirmation" runat="server" Visible="false">
		<p style="padding:20px;border:2px solid green;">
			The crawl has started and my take several hours to complete. Analysis will be run after the crawl has completed. Following completion of the crawl, the machine learning system requires tagging the contribution of several individual theory members to serve as training data.
		</p>
    </asp:Literal>
    <h3>
        <asp:Label ID="lblNetworkName" runat="server" Text="Network Name" Style="width: 250px;"></asp:Label>
    </h3>
    <asp:TextBox ID="txtNetworkName" runat="server"></asp:TextBox>
    <br />
    <h3>
        <asp:Label ID="lblNetworkComments" runat="server" Text="Comments"></asp:Label></h3>
    <asp:TextBox ID="txtNetworkComments" runat="server" TextMode="MultiLine" Rows="3" Width="400px" Height="48px" />
    <br />
    <asp:Table runat="server" Width="100%" BorderWidth="1" BorderStyle="Dashed">
        <asp:TableHeaderRow>
            <asp:TableHeaderCell ID="MAS_HEADER" HorizontalAlign="Center">
                MAS
            </asp:TableHeaderCell>
            <asp:TableHeaderCell ID="WOK_HEADER" HorizontalAlign="Center">
                WOK
            </asp:TableHeaderCell>
        </asp:TableHeaderRow>
        <asp:TableRow VerticalAlign="Top">
            <asp:TableCell>
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" ShowFooter="True" Width="549px" ShowHeaderWhenEmpty="True">
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label ID="lbMasId1" runat="server" Text="Paper ID(s)"></asp:Label>
                                <div style="font-size: .8em; font-weight: normal;">Multiple MAS IDs may be separated by commas (e.g. 12345,567890)</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:TextBox ID="txtMasId1" runat="server" Text='<%# Bind("txtMasId1") %>' ReadOnly='<%# Bind("readonly") %>' ></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:Button ID="Button1" runat="server" Text="Add Another Source" OnCommand="AddNewDataSourceToGrid1" />

            </asp:TableCell>
            <asp:TableCell>
                <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="false" ShowFooter="True" ShowHeaderWhenEmpty="True">
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label ID="lbWokId1" runat="server" Text="Paper ID(s)"></asp:Label>
                                <div style="font-size: .8em; font-weight: normal;">Multiple WOK IDs may be separated by commas (e.g. 12345,567890)</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                 <asp:TextBox ID="txtWokId1" runat="server" Text='<%# Bind("txtWokId1") %>'  ReadOnly='<%# Bind("readonly") %>'></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:Button ID="Button2" runat="server" Text="Add Another Source" OnCommand="AddNewDataSourceToGrid2" />

            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <br />
    <br />
    <asp:Label ID="recrawl" runat="server" Text="Recrawl:" Style="font-weight: 700; font-size: medium"></asp:Label>
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
    </strong></span>
    <br />
    <asp:CheckBox ID="AEF" runat="server" TextAlign="Left" Text="AEF" OnCheckedChanged="AEF_CheckedChanged" AutoPostBack="True" />
    <asp:CheckBox ID="ImpactFactor" runat="server" TextAlign="Left" Text="Impact Factor" OnCheckedChanged="ImpactFactor_CheckedChanged" AutoPostBack="True" />
    <asp:CheckBox ID="TAR" runat="server" TextAlign="Left" Text="TAR" Enabled="False" ForeColor="Silver" />
    <asp:CheckBox ID="DataMining" runat="server" TextAlign="Left" Text="Data Mining" />

    <asp:CheckBox ID="Clustring" runat="server" TextAlign="Left" Text="Clustering" />
    <br />
    <asp:Button ID="btnSubmit1" runat="server" OnCommand="btnSubmit_LaunchCrawler" Text="Start Crawler" />
    <asp:Button ID="btnForceAnalysis" runat="server" Text="Force Analysis" OnClick="btnForceAnalysis_Click" Visible="false" />
    <br />
    <asp:HiddenField runat="server" ID="TheoryID" />
</asp:Content>
