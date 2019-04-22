<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BulkUpload.aspx.cs" Inherits="ScrapMaricopa.BulkUpload" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head class="ff" runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="description" content="" />
    <meta name="author" content="" />
    <title>Bulk Upload</title>

    <!-- Bootstrap Core CSS -->
    <link rel="stylesheet" href="Bootstrapp.css" />
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
    <script type="text/javascript" src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.10.1/jquery.min.js"></script>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js"></script>
    <script type="text/javascript" src="http://code.jquery.com/jquery-1.12.4.min.js"></script>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
    <link rel="stylesheet" href="dist/Select2.css" />
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/select2/4.0.3/js/select2.min.js"></script>
    <script type="text/javascript" src="dist/jquery.loading.min.js"></script>
    <link rel="stylesheet" href="designcss.css" />
    <link href="dist/loading.min.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript">
        $(function () {
            $(".js-example-placeholder-single").select2({
                placeholder: "Select",
                allowClear: true
            });
        });
    </script>

    <script type="text/javascript">

        function SetTarget()
        {

            document.forms[0].target = "_blank";

        }

    </script>
    <style type="text/css">
        .GridLabel {
            font-family: Calibri;
            font-size: 20px;
            font-style: oblique;
            text-decoration: underline;
            cursor: pointer;
        }

        th {
            white-space: nowrap;
            font-weight: 400;
        }

        #bloc1, #bloc2 {
            display: inline;
        }

        .order {
            display: none;
            visibility: hidden;
        }

        .fullwidth {
            background: #ece9e9;
        }

        .sub-menu {
            padding: 0;
            border: 1px solid black;
            overflow-y: auto;
        }

        #myInput {
            border-box: box-sizing;
            background-image: url(searchicon.png);
            background-position: 14px 12px;
            background-repeat: no-repeat;
            font-size: 16px;
            padding: 14px 20px 12px 45px;
            border: 0px solid black;
            border-bottom: 1px solid #ddd;
            background-repeat: no-repeat;
            background-size: 50px 30px;
        }

            #myInput:focus {
                outline: 3px solid #ddd;
            }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <div class="container-fluid">
            <nav class="navbar navbar-default" role="navigation">
                <div class="container">
                    <div class="navbar-header">
                        <button type="button" class="navbar-toggle" data-toggle="collapse" data-target="#navbar-collapse-1">
                            <span class="sr-only">Toggle navigation</span>
                            <span class="icon-bar"></span>
                            <span class="icon-bar"></span>
                            <span class="icon-bar"></span>
                        </button>
                    </div>
                </div>
                <div class="navbar-brand" style="color: #151414cc; bottom: 10px; font-weight: bold; font-style: italic">STARS</div>

                <div class="collapse navbar-collapse" id="navbar-collapse-1">
                    <a class="nav navbar-nav navbar-left">
                        <img src="logo.png" class="img-responsive" alt="logo" />
                    </a>

                    <%--                    <div class="nav navbar-nav navbar-right" data-toggle="dropdown" style="width: 130px;">
                        <asp:DropDownList class="form-control js-example-placeholder-single col-sm-6 form-group form-inline dropdown-toggle dropdown" runat="server" ID="DropDownList2" AutoPostBack="true" Style="width: 180px;" OnSelectedIndexChanged="DropDownList2_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>--%>
                    <%-- <div class="nav navbar-nav navbar-right" style="margin-right:10px;">
                         <asp:Image onclick="window.location='Stars.aspx'" ID="Image4" runat="server" ImageUrl="~/House.png" ToolTip="Star Home Page" />
                    </div>--%>
                     <div class="nav navbar-nav navbar-right">
                        <asp:Button runat="server" ID="btnhome" Text="Home" class="form-control col-sm-6 js-example-placeholder-single form-inline dropdown-toggle dropdown" Style="height:37px;background-color:#d43f3a;color:#fff;padding-left:25px;" OnClick="btnhome_Click"/>
                    </div>

                    <div class="nav navbar-nav navbar-right" style ="padding-right:20px;">
                        <asp:Button runat="server" ID="btplacer" Text="Placer" class="form-control col-sm-6 js-example-placeholder-single form-inline dropdown-toggle dropdown" Style="height:37px;background-color:#d43f3a;color:#fff;padding-left:25px;" OnClick="btnplacer_Click"/>
                    </div>
                  
                </div>
            </nav>
        </div>

        <div class="container">
            <div class="col-lg-12 well">
                <div class="row" align="center">

                    <div class="col-sm-12" align="center">
                        <asp:TextBox ID="txtordertype" runat="server" Height="150px" TextMode="MultiLine" Width="900px"></asp:TextBox>
                        <br />
                        <asp:Label ID="Label1" runat="server" Font-Names="Verdana" Text="Format: BatchNo | OrderNo | Address | State | County"></asp:Label>
                        <br />
                        <br />
                        <asp:Button ID="BtnSubmit" runat="server" OnClick="BtnSubmit_Click" Text="Transmit" Width="68px" />
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnupload" runat="server" Text="Upload" Width="68px" OnClick="btnupload_Click" />
                        <br />

                        <asp:Label ID="lblstatus" runat="server" Font-Names="Verdana"></asp:Label>
                        <br />
                        <asp:GridView ID="gridtransmit" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="false" Visible="false">
                            <HeaderStyle BackColor="dimgray" ForeColor="White" />
                            <Columns>
                                <asp:BoundField DataField="Batch No" HeaderText="Batch No" />
                                <asp:BoundField DataField="Order No" HeaderText="Order No" />
                                <asp:BoundField DataField="Pdate" HeaderText="Pdate" />
                                <asp:BoundField DataField="Address" HeaderText="Address" />
                                <asp:BoundField DataField="State" HeaderText="State" />
                                <asp:BoundField DataField="County" HeaderText="County" />

                            </Columns>
                        </asp:GridView>

                        <br />
                        <asp:Panel ID="Panelinformation" runat="server" Visible="false">
                            <span class="InsideHeading">No. of orders Uploaded# :
                        <asp:Label ID="lblordersassigned" runat="server" ForeColor="Red"></asp:Label>
                                <br />
                                No. of orders Duplicates# :
                        <asp:Label ID="lblorderduplicates" runat="server" ForeColor="Red"></asp:Label>
                                <br />
                                Duplicate Orders# :
                        <asp:Label ID="LblDuplicate" runat="server" ForeColor="Red"></asp:Label>
                            </span>
                        </asp:Panel>

                    </div>
                </div>
            </div>

            <div class="col-lg-12 well">
                <div class="row">
                    <div class="col-sm-12" align="center">
                        <h3>Bulk Order Status</h3>
                    </div>
                    <div class="row" align="center">
                        <div >
                            <label>From: </label>
                            <asp:TextBox ID="txtfrmdate" runat="server" CssClass="txtuser" Width="100px"></asp:TextBox>
                            <cc1:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtfrmdate" Format="dd-MMM-yyyy">
                            </cc1:CalendarExtender>
                            <label>To: </label>
                            <asp:TextBox ID="txttodate" runat="server" CssClass="txtuser" Width="100px"></asp:TextBox>
                            <cc1:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="txttodate" Format="dd-MMM-yyyy">
                            </cc1:CalendarExtender>
                            <asp:Button runat="server" ID="btngetdetails" Text="Submit" OnClick="btngetdetails_Click" />
                            <%-- <label>Select Batch: </label>
                                <asp:DropDownList class="form-control" runat="server" ID="dropbatch" AutoPostBack="true" Style="width: 170px" OnSelectedIndexChanged="dropbatch_SelectedIndexChanged">
                                </asp:DropDownList>--%>
                        </div>
                        <%--   <div class="col-sm-6 form-group form-inline">
                                <label>County:</label>
                                <asp:DropDownList class="form-control" runat="server" ID="dropCounty" AutoPostBack="true" Style="width: 170px" OnSelectedIndexChanged="dropCounty_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>--%>
                        <div class="col-sm-6 form-group form -inline">
                            <asp:HiddenField ID="hdnField1" runat="server" />
                        </div>
                    </div>
                    <div style="margin-left: 250px; width: 500px;" align="center">
                        <%--<asp:Label runat="server" ID="Label2" CssClass="GridLabel" onclick="test('GridView2')"></asp:Label>
                             <br />--%>
                        <asp:GridView ID="grdorderbatch" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="false" OnRowCommand="grdorderbatch_RowCommand" DataKeyNames="batch_no">
                            <HeaderStyle BackColor="dimgray" ForeColor="White" />
                            <Columns>
                                <asp:TemplateField HeaderText="SNo#">
                                    <ItemTemplate>
                                        <%# Container.DataItemIndex + 1 %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="pdate" HeaderText="Pdate" />
                                <asp:TemplateField HeaderText="Batch">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkbatch" runat="server" Text='<%# Eval("batch_no") %>' CommandName="batchwise"></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Count" HeaderText="Count" />
                                
                            </Columns>

                        </asp:GridView>
                        <%--   <asp:DetailsView ID="DetailsView2" runat="server" Height="150px" CssClass="Gnowrap" AutoGenerateRows="false" AlternatingRowStyle-CssClass="alt" Width="100%" Font-Names="Verdana" Font-Size="Smaller">
                            <Fields>
                                <asp:TemplateField HeaderText="YTS">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="ftlnkbtnyts" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.YTS") %>' ForeColor="Black" OnClick="ftlnkbtnyts_Click"> </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Retrieved">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="ftlnkbtnscrapcomp" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ScrapingCompleted") %>' ForeColor="Black" OnClick="ftlnkbtnscrapcomp_Click"> </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="MultiParcel">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="ftlnkbtnmulti" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.MULTIPARCEL") %>' ForeColor="Black" OnClick="ftlnkbtnmulti_Click"> </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="ErrorList">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="ftlnkbtnerror" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SCRAPINGERROR") %>' ForeColor="Black" OnClick="ftlnkbtnerror_Click"> </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Fields>
                        </asp:DetailsView>--%>
                    </div>
                    <br />
                    <div>
                        <asp:GridView ID="grdorderdet" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="false" DataKeyNames="id" OnRowCommand="grdorderdet_RowCommand">
                            <HeaderStyle BackColor="dimgray" ForeColor="White" />
                            <Columns>
                                <asp:TemplateField HeaderText="SNo#">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkViewParcel" runat="server" Visible="false" AutoPostBack="True" />
                                        <%# Container.DataItemIndex + 1 %>
                                    </ItemTemplate>

                                </asp:TemplateField>
                                <asp:BoundField DataField="Batch No" HeaderText="Batch No" />
                                <asp:TemplateField HeaderText="Order No">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="Lnkorder" runat="server" Text='<%# Eval("Order No") %>' CommandName="Process" OnClientClick = "SetTarget();"></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <%--<asp:BoundField DataField="Order No" HeaderText="Order No" />--%>
                                <asp:BoundField DataField="Date" HeaderText="Pdate" />
                                <asp:BoundField DataField="State" HeaderText="State" />
                                <asp:BoundField DataField="County" HeaderText="County" />
                                <asp:BoundField DataField="Address" HeaderText="Address" />
                                <asp:BoundField DataField="Scrape Status" HeaderText="Scrape Status" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
