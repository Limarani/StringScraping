<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TaxDetails.aspx.cs" Inherits="ScrapMaricopa.TaxDetails" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head class="ff" runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="description" content="" />
    <meta name="author" content="" />
    <title>Bulk Order Status</title>
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
        <div class="container-fluid">
            <nav class="navbar navbar-default" role="navigation">
                <div class="navbar-brand" style="color: #151414cc; bottom: 10px; font-weight: bold; font-style: italic">STARS</div>

                <div class="collapse navbar-collapse" id="navbar-collapse-1">
                    <a class="nav navbar-nav navbar-left">
                        <img src="logo.png" class="img-responsive" alt="logo" />
                    </a>

                    <%--<div class="nav navbar-nav navbar-right" data-toggle="dropdown" style="width: 130px;">
                        <asp:DropDownList class="form-control js-example-placeholder-single col-sm-6 form-group form-inline dropdown-toggle dropdown" runat="server" ID="DropDownList2" AutoPostBack="true" Style="width: 180px;" OnSelectedIndexChanged="DropDownList2_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>--%>
                </div>
            </nav>
        </div>
        <div>
            <div class="col-sm-12" align="center">
                <h3>Tax Details</h3>
           
            <div id="bloc2" runat="server" class="form-group form-inline" style="position: absolute" visible="false">
                    <%--<label style="text-align: right; width: 1050px;">ScreenShots:</label>--%>
                    <asp:HyperLink ID="hyperlink2" Font-Bold="True" Font-Size="Large" NavigateUrl="~/TaxPagesBulk.aspx" Target="_blank" Text="Tax Pages" runat="server" Style="text-decoration: underline;padding-left:300px; text-align:right;" />
                </div>
                 </div>
            <asp:Panel runat="server" ID="pnldet">
                <div id="bloc1" runat="server" class="form-group form-inline" style="position: absolute" visible="false">
                    <label style="text-align: right; margin-left: 105px">Property Address:</label>
                    <asp:Label runat="server" ID="mylabel"></asp:Label>
                </div>
                
                <br />
                <br />

                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label3" CssClass="GridLabel"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView3" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                        <Columns>
                           <%-- <asp:TemplateField>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkselect" runat="server" class="demo3 demo66" OnCheckedChanged="chkselect_CheckedChanged" OnClick="makeProgress()"
                                        AutoPostBack="True" />
                                </ItemTemplate>
                            </asp:TemplateField>--%>
                        </Columns>
                    </asp:GridView>
                </div>

                <div style="margin-left: 38px;">
                    <asp:Label runat="server" ID="mobileTax" CssClass="GridLabel"></asp:Label>
                    <br />
                    <asp:GridView ID="mobiletax_multiparcel" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                        <Columns>
                           <%-- <asp:TemplateField>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkselect" runat="server" class="demo3 demo66" OnCheckedChanged="mobile_chkselect_CheckedChanged" OnClick="makeProgress()"
                                        AutoPostBack="True" />
                                </ItemTemplate>
                            </asp:TemplateField>--%>
                        </Columns>
                    </asp:GridView>
                </div>

                <div id="mydiv" style="margin-left: 38px;">
                    <asp:Label runat="server" ID="Label7" CssClass="GridLabel" onclick="test('GridView7')">     
                    </asp:Label>
                    <br />
                    <asp:GridView ID="GridView7" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true" Style="width: 1280px;">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>

                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label2" CssClass="GridLabel" onclick="test('GridView2')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView2" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label1" CssClass="GridLabel" onclick="test('GridView1')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView1" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div id="div3" style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label4" CssClass="GridLabel" onclick="test('GridView4')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView4" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label5" CssClass="GridLabel" onclick="test('GridView5')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView5" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div id="div4" style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label6" CssClass="GridLabel" onclick="test('GridView6')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView6" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div id="div5" style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label8" CssClass="GridLabel" onclick="test('GridView8')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView8" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label9" CssClass="GridLabel" onclick="test('GridView9')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView9" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label10" CssClass="GridLabel" onclick="test('GridView10')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView10" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label11" CssClass="GridLabel" onclick="test('GridView11')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView11" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label12" CssClass="GridLabel" onclick="test('GridView12')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView12" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label13" CssClass="GridLabel" onclick="test('GridView13')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView13" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label14" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView14" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                 <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label15" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView15" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label16" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView16" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label17" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView17" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label18" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView18" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label19" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView19" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label20" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView20" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label21" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView21" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label22" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView22" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label23" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView23" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label24" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView24" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label25" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView25" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label26" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView26" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label27" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView27" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label28" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView28" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label29" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView29" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label30" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView30" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label31" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView31" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label32" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView32" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label33" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView33" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label34" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView34" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label35" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView35" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label36" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView36" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label37" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView37" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label38" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView38" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label39" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView39" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
                <div style="margin-left: 38px; margin-right: 30px;">
                    <asp:Label runat="server" ID="Label40" CssClass="GridLabel" onclick="test('GridView14')"></asp:Label>
                    <br />
                    <asp:GridView ID="GridView40" runat="server" CssClass="table table-striped table-bordered table-hover th" GridLines="None" AutoGenerateColumns="true">
                        <HeaderStyle BackColor="dimgray" ForeColor="White" />
                    </asp:GridView>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlnodata" runat="server" Visible="false">
                <div class="col-sm-12" align="center">
                    <h4>No Data Found</h4>
                </div>
            </asp:Panel>

        </div>

    </form>
</body>
</html>
