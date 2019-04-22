<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PlacerTitle.aspx.cs" Inherits="ScrapMaricopa.PlacerTitle" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head class="ff" runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="description" content="" />
    <meta name="author" content="" />
    <title>Placer Title</title>

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

        function SetTarget() {

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
        <div>
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

                        <div class="nav navbar-nav navbar-right">
                            <asp:Button runat="server" ID="btnhome" Text="Home" class="form-control col-sm-6 js-example-placeholder-single form-inline dropdown-toggle dropdown" Style="height: 37px; background-color: #d43f3a; color: #fff; padding-left: 25px;" OnClick="btnhome_Click" />
                        </div>
                    </div>
                </nav>
            </div>
            <div class="container">

                 <div class="col-sm-12" align="center">
                        <h3>Placer Order Details</h3>
                    </div>

                <div class="col-lg-12 well">
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
                            
                        </div>

                        <br />

                        <div >
                            <asp:GridView CssClass="table table-striped table-bordered table-hover th" GridLines="None"  ID="GridView1" runat="server" BackColor="#CCCCCC" BorderColor="#999999" BorderStyle="Solid" BorderWidth="3px" CellPadding="4" CellSpacing="2" ForeColor="Black">
                                <FooterStyle BackColor="#CCCCCC" />
                                <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#CCCCCC" ForeColor="Black" HorizontalAlign="Left" />
                                <RowStyle BackColor="White" />
                                <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
                                <SortedAscendingCellStyle BackColor="#F1F1F1" />
                                <SortedAscendingHeaderStyle BackColor="#808080" />
                                <SortedDescendingCellStyle BackColor="#CAC9C9" />
                                <SortedDescendingHeaderStyle BackColor="#383838" />
                            </asp:GridView>

                        </div>
                    </div>
                </div>
            </div>

        </div>
    </form>
</body>
</html>
