using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;

namespace ScrapMaricopa.lereta
{
    public class scrape
    {
   
        public object Newtonsoft { get; private set; }
        public EventHandler ExceuteOrders;

        string mysqlConnection = ConfigurationManager.ConnectionStrings["lereta"].ConnectionString;
        MySqlConnection mConnection;
        MySqlDataAdapter mDa;
        MySqlCommand mCmd;
        MySqlDataReader mDr;
        MySqlParameter[] mParam;
        static string ErrMsg = "";
        static bool IsErr;


        public string RunOrderList(string orderNumber, string county, string state, string borrowername, string address,string parcelno)
        {

            string  ownerName = "", propertyAddress = "", taddress = "", postdir = "", json = "";
            string cityName = "", streetNumber = "", direction = "", streetName = "", zipCode = "", streetLine = "", streetType = "", unitnumber = "";

            //get county id...
            DataSet ds = GetCountyId(state, county);
            string addresstype = ds.Tables[0].Rows[0]["Address_Type"].ToString();
            string countyID = ds.Tables[0].Rows[0]["State_County_Id"].ToString();
            string apiUrl = ds.Tables[0].Rows[0]["service_url"].ToString();
            string team = ds.Tables[0].Rows[0]["team"].ToString();
            //parcel search
            if (parcelno != "")
            {
                if (team == "External")
                {
                    object input = new
                    {
                        Address = "",
                        StreetNumber = "",
                        HouseNumberFrom = "",
                        HouseNumberTo = "",
                        AssessorNumber = "",
                        AlternateID = "",
                        PPIN = "",
                        StreetName = "",
                        OwnerName = "",
                        OwnerLastName = "",
                        OwnerFirstName = "",
                        ParcelNumber = parcelno,
                        City = "",
                        Zipcode = "",
                        DistrictCode = "",
                        AccountNumber = "",
                        Direction = "",
                        StreetType = "",
                        SubDivision = "",
                        UnitNumber = "",
                        Folio = "",
                        TaxNumber = "",
                        OrganizationName = "",
                        County = "",
                        CountyID = "",
                        OrderID = orderNumber,
                        titleflexSearchId = ""
                    };
                    json = ScrapOrder(apiUrl, input);
                    updateRecordStatus(json, orderNumber);
                    return json;
                }

                else if (team == "Internal")
                {
                    object input = new
                    {
                        address = "",
                        houseno = "",
                        sname = "",
                        sttype = "",
                        parcelNumber = parcelno,
                        searchType = "",
                        orderNumber = orderNumber,
                        ownername = "",
                        directParcel = "",
                        account = "",
                        direction = "",
                        unitNumber = "",
                        assessmentID = "",
                        city = "",
                        state = "",
                        county = ""
                    };
                    json = ScrapOrder(apiUrl, input);
                    updateRecordStatusSST(json, orderNumber);
                    return json;
                }
                
            }
        

            county = county.Trim();
            state = state.Trim();
            ownerName = borrowername;
            propertyAddress = address;
           
            try
            {
                AddressParser.AddressParser splitAddr = new AddressParser.AddressParser();
                var splitAddrList = splitAddr.ParseAddress(propertyAddress);
                cityName = splitAddrList.City;
                streetNumber = splitAddrList.Number;
                direction = splitAddrList.Predirectional;
                streetName = splitAddrList.Street;
                zipCode = splitAddrList.Zip;
                streetLine = splitAddrList.StreetLine;
                streetType = splitAddrList.Suffix;
                unitnumber = splitAddrList.SecondaryNumber;
                if (streetType != null)
                {
                    if (countyID == "2")
                    {
                        streetType = splitAddrList.Suffix;
                    }
                    else
                    {
                        streetType = ReturnStType(streetType);
                        if (streetType == null)
                        {
                            streetType = "";
                        }
                    }
                }
                else
                {
                    streetType = "";
                }
                if (direction == null)
                {
                    direction = "";
                }
                if (unitnumber == null)
                {
                    unitnumber = "";
                }
            }
            catch
            {

            }
            #region countlist
            if (countyID == "23")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }
                object input = new
                {
                    address = streetLine,
                    parcelNumber = "",
                    ownername = "",
                    SearchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = ""

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    //4 == Multiparcel
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }


            }
            else if (countyID == "20")
            {
                string[] sname = streetName.Split(' ');
                streetName = sname[0].ToString();
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }

                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    blockno = "",
                    parcelNumber = "",
                    SearchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = unitnumber

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }

            }
            else if (countyID == "40")
            {
                string stname = streetName;
                if (stname.Contains(" DR") || stname.Contains(" RD") || stname.Contains(" ST") || stname.Contains(" CT") || stname.Contains(" CIR"))
                {
                    stname = stname.Replace(" DR", "").Replace(" RD", "").Replace(" ST", "").Replace(" CT", "").Replace(" CIR", "");
                }
                propertyAddress = streetNumber + " " + direction + " " + stname;
                object input = new
                {
                    address = propertyAddress,
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    directParce = ""

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    //1 == Multiparcel
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }
            //gwinnet
            else if (countyID == "22")
            {
                object input = new
                {
                    address = streetLine,
                    parcelNumber = "",
                    ownername = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    directParcel = ""

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }

                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }
            //river
            else if (countyID == "19")
            {
                string stname = streetName;
                if (stname.Contains(" DR") || stname.Contains(" RD") || stname.Contains(" ST") || stname.Contains(" CT") || stname.Contains(" CIR"))
                {
                    stname = stname.Replace(" DR", "").Replace(" RD", "").Replace(" ST", "").Replace(" CT", "").Replace(" CIR", "");
                }

                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = stname,
                    sttype = "",
                    parcelNumber = "",
                    ownername = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = unitnumber

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }

                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }

            }

            else if (countyID == "34")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                    propertyAddress = streetLine;
                }
                else
                {
                    sear_type = "address"; propertyAddress = streetNumber + " " + streetName + " " + streetType;
                }

                object input = new
                {
                    address = propertyAddress,
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = ""
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }

                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }

            }

            else if (countyID == "33")
            {
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    parcelNumber = "",
                    ownername = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    directParcel = ""

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }


            else if (countyID == "30")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    // streetLine.Replace("APT", "").Replace("UNIT", "").Replace("#", "");
                    sear_type = "titleflex";
                    propertyAddress = streetLine;
                }
                else
                {
                    propertyAddress = streetNumber + " " + streetName;
                    sear_type = "address";
                }

                object input = new
                {
                    address = propertyAddress,
                    ownerName = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = ""
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }

            }
            else if (countyID == "32")
            {
                string[] sname = streetName.Split(' ');
                streetName = sname[0].ToString();
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }

                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    blockno = "",
                    parcelNumber = "",
                    ownername = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = unitnumber
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }
            //deklab
            else if (countyID == "29")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                    propertyAddress = streetNumber + " " + streetName + " " + streetType + " " + unitnumber;
                }
                else
                {
                    if (streetType == "WY")
                    {
                        streetType = "WAY";
                    }
                    sear_type = "address";
                    propertyAddress = streetNumber + " " + streetName + " " + streetType;
                }


                object input = new
                {
                    address = propertyAddress,
                    parcelNumber = "",
                    ownername = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = ""
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }
            else if (countyID == "31")
            {
                string stname = streetName;
                if (stname.Contains(" DR") || stname.Contains(" RD") || stname.Contains(" ST") || stname.Contains(" CT") || stname.Contains(" CIR") || stname.Contains(" LN "))
                {
                    stname = stname.Replace(" DR", "").Replace(" RD", "").Replace(" ST", "").Replace(" CT", "").Replace(" CIR", "").Replace(" LN", "");
                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = stname,
                    sttype = streetType,
                    parcelNumber = "",
                    ownername = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    directParcel = unitnumber
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }


            //placer
            else if (countyID == "56")
            {
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = "",
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    directParcel = "",
                    ownername = ""

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //san joaquin
            else if (countyID == "36")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                    propertyAddress = streetLine;
                }
                else
                {
                    sear_type = "address";
                    propertyAddress = streetNumber + " " + streetName;
                }


                object input = new
                {

                    address = propertyAddress,
                    unitNumber = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                    ownername = ""
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //san francisco
            else if (countyID == "35")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }
                //propertyAddress = streetNumber + " " + streetName + " " + streetType;
                object input = new
                {
                    address = streetLine,
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //fresno
            else if (countyID == "37")
            {

                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    parcelNumber = "",
                    searchType = "titleflex",
                    orderNumber = orderNumber,
                    directParcel = "",
                    ownername = ""

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //harrison
            else if (countyID == "41")
            {
                if (streetName.Contains("HIGHWAY"))
                {
                    streetName = streetName.Replace("HIGHWAY", "HWY");
                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = "",
                    account = ""

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //bernalillo
            else if (countyID == "54")
            {
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = "",
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    ownername = "",
                    directSearch = ""


                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //stcharles
            else if (countyID == "55")
            {
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = "",
                    account = ""


                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //tulsa
            else if (countyID == "57")
            {

                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    direction = direction,
                    sttype = "",
                    parcelNumber = "",
                    searchType = "address",
                    accountnumber = "",
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = ""


                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {

                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //utah
            else if (countyID == "58")
            {
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    direction = "",
                    sttype = streetType,
                    parcelNumber = "",
                    searchType = "address",
                    accountnumber = "",
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = ""


                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //summit
            else if (countyID == "60")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                    propertyAddress = streetLine;
                }
                else
                {
                    sear_type = "address";
                    propertyAddress = streetNumber + " " + streetName;
                }

                object input = new
                {
                    address = propertyAddress,
                    accountnumber = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = ""

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //hennepin
            else if (countyID == "49")
            {
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    direction = "",
                    sttype = "",
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = ""
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //new castle
            else if (countyID == "59")
            {
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = ""
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //pinal
            else if (countyID == "61")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    direction = direction,
                    sttype = streetType,
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = "",
                    unitNumber = ""
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //marion
            else if (countyID == "76")
            {
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = "",
                    accountNumber = ""
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //shelby
            else if (countyID == "74")
            {
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = "",
                    account = ""
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //clackamas
            else if (countyID == "73")
            {
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = "",
                    direction = direction,
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = "",
                    account = ""
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //east baton rouge
            else if (countyID == "78")
            {
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = "",
                    account = ""
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //baltimore
            else if (countyID == "72")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = "",
                    unitNumber = unitnumber
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //polk
            else if (countyID == "63")
            {
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    direction = direction,
                    sttype = streetType,
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = ""

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }
            //charleston
            else if (countyID == "82")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                    propertyAddress = streetLine;
                }
                else
                {
                    sear_type = "address";
                    propertyAddress = streetNumber + " " + streetName;
                }

                propertyAddress = streetNumber + " " + streetName;
                object input = new
                {
                    address = propertyAddress,
                    account = "",
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = ""

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }



            //jeferson

            else if (countyID == "93")
            {
                propertyAddress = streetNumber + " " + streetName;
                object input = new
                {
                    address = propertyAddress,
                    ownername = "",
                    assessment_id = "",
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    directParcel = ""

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //douglas

            else if (countyID == "83")
            {
                propertyAddress = streetNumber + " " + streetName;
                object input = new
                {

                    houseno = streetNumber,
                    sname = streetName,
                    ownername = "",
                    assessment_id = "",
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    directParcel = ""

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //Anoka



            else if (countyID == "100")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                    propertyAddress = streetLine;
                }
                else
                {
                    sear_type = "address";
                    propertyAddress = streetNumber + " " + streetName;
                }

                object input = new
                {
                    address = propertyAddress,
                    ownername = "",
                    assessment_id = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = ""

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //stark

            else if (countyID == "101")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }
                object input = new
                {
                    houseno = streetNumber,
                    direction = "",
                    sname = streetName,
                    unitNumber = unitnumber,
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = ""

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //cherokee

            else if (countyID == "103")
            {
                object input = new
                {
                    houseno = streetNumber,
                    direction = direction,
                    sname = streetName,
                    sttype = streetType,
                    unitNumber = "",
                    ownername = "",
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    directParcel = ""

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //hillsborough

            else if (countyID == "2")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }
                object input = new
                {
                    houseno = streetNumber,
                    sttype = streetType,
                    sname = streetName,
                    unitNumber = unitnumber,
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = ""

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //denver

            else if (countyID == "8")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }

                object input = new
                {
                    address = streetLine,
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = ""

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //harford

            else if (countyID == "114")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }

                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    unitNumber = unitnumber,
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = ""

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //Yolo


            else if (countyID == "152")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                    propertyAddress = streetLine;
                }
                else
                {
                    sear_type = "address";
                    propertyAddress = streetNumber + " " + streetName;
                }

                object input = new
                {
                    address = propertyAddress,
                    assessment_id = "",
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    directParcel = "",
                    ownername = ""
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //clayton

            else if (countyID == "141")
            {

                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    unitNumber = "",
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = ""
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }


            //Berkeley


            else if (countyID == "128")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    unitNumber = unitnumber,
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = ""
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //Newton
            else if (countyID == "158")
            {
                propertyAddress = streetNumber + " " + streetName;
                object input = new
                {
                    address = propertyAddress,
                    assessment_id = "",
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    directParcel = "",
                    ownername = "",
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //dakota

            else if (countyID == "132")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }

                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    unitNumber = unitnumber,
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = ""
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }


            //ALMadison

            else if (countyID == "116")
            {
                propertyAddress = streetNumber + " " + streetName;
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    account = "",
                    parcelNumber = "",
                    searchType = "address",
                    ownername = "",
                    orderNumber = orderNumber,
                    directParcel = ""
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }
            //ARWashington

            else if (countyID == "149")
            {
                propertyAddress = streetNumber + " " + streetName;
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    direction = direction,
                    account = "",
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = ""
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //AZMaricopa

            else if (countyID == "13")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                    propertyAddress = streetLine;
                }
                else
                {
                    sear_type = "address";
                    if (direction != "")
                    {
                        propertyAddress = streetNumber + " " + direction + " " + streetName;
                    }
                    else
                    {
                        propertyAddress = streetNumber + " " + streetName;
                    }
                }

                object input = new
                {
                    address = propertyAddress,
                    parcelNumber = "",
                    ownername = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = ""
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }
            //CA Alameda

            else if (countyID == "9")
            {
                //string sear_type = "";
                //if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                //{
                //    sear_type = "titleflex";
                //}
                //else
                //{
                //    sear_type = "address";
                //}

                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                TextInfo textInfo = cultureInfo.TextInfo;
                object input = new
                {
                    houseno = streetNumber,
                    direction = "",
                    sname = streetName,
                    sttype = streetType,
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    ownername = "",
                    city = textInfo.ToTitleCase(cityName.ToLower()),
                    unitNumber = unitnumber

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }
            //CA Contracosta

            else if (countyID == "11")
            {
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = "",
                    city = cityName,
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = direction

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //CAEldorado


            else if (countyID == "95")
            {
                object input = new
                {
                    address = streetLine,
                    account = "",
                    parcelNumber = "",
                    ownername = "",
                    searchType = "titleflex",
                    orderNumber = orderNumber,
                    directParcel = ""

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //CAMonterery
            else if (countyID == "86")
            {

                propertyAddress = streetNumber + " " + streetName;
                object input = new
                {
                    address = propertyAddress,
                    parcelNumber = "",
                    searchType = "titleflex",
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = "",
                    state = "CA",
                    county = "Monterey"

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }
            //CASacramento

            else if (countyID == "10")
            {

                string sear_type = "titleflex";
                propertyAddress = streetLine;


                object input = new
                {
                    address = propertyAddress,
                    account = "",
                    parcelNumber = "",
                    ownername = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //CASantabarabara

            else if (countyID == "88")
            {
                propertyAddress = streetNumber + " " + streetName + " " + streetType;
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = ""

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //FLDuval

            else if (countyID == "4")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }

                object input = new
                {

                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    unitNumber = unitnumber,
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber


                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //FLBroward

            else if (countyID == "3")
            {


                //if (streetName.Any(char.IsDigit))
                //{
                //    streetName = Regex.Match(streetName, @"\d+").Value;
                //}

                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    direction = direction,
                    parcelNumber = "",
                    ownername = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    directParcel = "",
                    unitNumber = unitnumber
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //FLCollier

            else if (countyID == "138")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }
                object input = new
                {

                    address = streetLine,
                    parcelNumber = "",
                    ownername = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",


                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //FLManatee
            else if (countyID == "122")
            {
                propertyAddress = streetNumber + " " + streetName;
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = ""

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }
            //FLMiamedade

            else if (countyID == "6")
            {
                if (streetName.ToUpper().Contains("STREET"))
                {
                    streetName = streetName.Replace("STREET", "");
                }
                object input = new
                {

                    houseno = streetNumber,
                    sname = streetName,
                    direction = direction,
                    account = "",
                    parcelNumber = "",
                    ownername = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    directParcel = "",

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }
            //FLorange

            else if (countyID == "7")
            {

                object input = new
                {

                    address = streetLine,
                    parcelNumber = "",
                    ownername = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    directParcel = "",

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //FLOsceola

            else if (countyID == "151")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                    propertyAddress = streetLine;
                }
                else
                {
                    sear_type = "address";
                    propertyAddress = streetNumber + " " + streetName;
                }

                object input = new
                {

                    address = propertyAddress,
                    parcelNumber = "",
                    ownername = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //FLPalmBeach
            else if (countyID == "5")
            {
                string sear_type = "";
                string stname = streetName;
                if (stname.Contains(" DR") || stname.Contains(" RD") || stname.Contains(" ST") || stname.Contains(" CT") || stname.Contains(" CIR") || stname.Contains(" AVE") || stname.Contains(" LN") || stname.Contains(" PL"))
                {
                    stname = stname.Replace(" DR", "").Replace(" RD", "").Replace(" ST", "").Replace(" CT", "").Replace(" CIR", "").Replace(" AVE", "").Replace(" LN", "").Replace(" PL", "");
                }
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    streetLine.Replace("APT", "").Replace("UNIT", "").Replace("#", "");
                    sear_type = "titleflex";
                    propertyAddress = streetLine;
                }
                else
                {
                    if (streetType == "WY")
                    {
                        streetType = "WAY";
                    }
                    if (direction != "")
                    {
                        if (streetType == "LK")
                        {
                            propertyAddress = streetNumber + " " + direction + " " + stname;
                        }
                        else
                        {
                            propertyAddress = streetNumber + " " + direction + " " + stname + " " + streetType;
                        }

                    }
                    else
                    {
                        if (streetType == "LK")
                        {
                            propertyAddress = streetNumber + " " + stname;
                        }
                        else
                        {
                            propertyAddress = streetNumber + " " + stname + " " + streetType;
                        }
                    }
                    sear_type = "address";
                }


                object input = new
                {

                    address = propertyAddress,
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
            }

            //Polk

            else if (countyID == "129")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                    propertyAddress = streetLine;
                }
                else
                {
                    sear_type = "address";
                    propertyAddress = streetNumber + " " + streetName;
                }

                object input = new
                {

                    address = propertyAddress,
                    parcelNumber = "",
                    ownername = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //Pasco

            else if (countyID == "99")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                    propertyAddress = streetLine;
                }
                else
                {
                    sear_type = "address";
                    streetName = streetName.Replace("\r\n", "").Trim();
                    if (streetName.Contains(" DRNEW") || streetName.Contains(" RDNEW") || streetName.Contains(" CIRNEW") || streetName.Contains(" STNEW") || streetName.Contains(" LNNEW") || streetName.Contains(" AVENEW") || streetName.Contains(" CTNEW"))
                    {
                        streetName = streetName.Replace(" DRNEW", "").Replace(" RDNEW", "").Replace(" CIRNEW", "").Replace(" STNEW", "").Replace(" LNNEW", "").Replace(" AVENEW", "").Replace(" CTNEW", "");
                        propertyAddress = streetNumber + " " + streetName;
                    }
                }


                object input = new
                {
                    houseno = streetNumber,
                    direction = "",
                    sname = streetName,
                    sttype = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = "",


                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //FLSarasotta

            else if (countyID == "96")
            {
                propertyAddress = streetNumber + " " + streetName;
                object input = new
                {

                    address = propertyAddress,
                    parcelNumber = "",
                    ownername = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    directParcel = "",



                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //FLStlucie

            else if (countyID == "112")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }
                object input = new
                {
                    houseno = streetNumber,
                    direction = "",
                    sname = streetName,
                    sttype = streetType,
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = "",
                    unitNumber = unitnumber



                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //FlVolusia

            else if (countyID == "80")
            {
                string stname = streetName;
                if (stname.Contains(" DR") || stname.Contains(" RD") || stname.Contains(" ST") || stname.Contains(" CT") || stname.Contains(" CIR") || stname.Contains(" AVE"))
                {
                    stname = stname.Replace(" DR", "").Replace(" RD", "").Replace(" ST", "").Replace(" CT", "").Replace(" CIR", "").Replace(" AVE", "");
                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = stname,
                    direction = "",
                    account = "",
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = "",

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }
            //ILWill

            else if (countyID == "115")
            {

                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    city = "",
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = "",


                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //INHamilton
            else if (countyID == "139")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                    propertyAddress = streetLine;
                }
                else
                {
                    sear_type = "address";
                    propertyAddress = streetNumber + " " + streetName;
                }

                object input = new
                {

                    address = propertyAddress,
                    parcelNumber = "",
                    ownername = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //INMarion
            else if (countyID == "97")
            {
                propertyAddress = streetNumber + " " + direction + " " + streetName;
                object input = new
                {
                    address = propertyAddress,
                    ownername = "",
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    directParcel = "",

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //MdCarrol
            else if (countyID == "153")
            {
                string stname = streetName;
                if (stname.Contains(" DR") || stname.Contains(" RD") || stname.Contains(" ST") || stname.Contains(" CT") || stname.Contains(" CIR"))
                {
                    stname = stname.Replace(" DR", "").Replace(" RD", "").Replace(" ST", "").Replace(" CT", "").Replace(" CIR", "");
                }

                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";
                }

                object input = new
                {
                    houseno = streetNumber,
                    sname = stname,
                    direction = direction,
                    account = "",
                    parcelNumber = "",
                    ownername = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = unitnumber

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //NVClark

            else if (countyID == "1")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else

                {
                    sear_type = "address";
                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    unitNumber = unitnumber,
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //OHhamilton

            else if (countyID == "94")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }

                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    parcelNumber = "",
                    ownername = "",
                    searchType = sear_type,
                    unitNumber = unitnumber,
                    orderNumber = orderNumber,

                    directParcel = "",

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //OkCleveland

            else if (countyID == "119")
            {

                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    direction = direction,
                    account = "",
                    parcelNumber = "",
                    ownername = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    directParcel = ""

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }

            //ORDeschutes



            else if (countyID == "98")
            {
                propertyAddress = streetNumber + " " + streetName;
                object input = new
                {
                    address = propertyAddress,
                    ownername = "",
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    directParcel = "",


                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }
            //Sanlouispoca

            else if (countyID == "125")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }
                propertyAddress = streetNumber + " " + streetName;
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    unitNumber = unitnumber,
                    parcelNumber = "",
                    ownername = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",

                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }
            //WYLaramine

            else if (countyID == "140")
            {
                propertyAddress = streetNumber + " " + streetName;
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    unitNumber = "",
                    ownername = "",
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
                {
                    // 2 completed                        
                    string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else if (json.Contains("An error has occured"))
                {
                    //3 == scraping error                        
                    string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);
                }
                else
                {
                    string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                    int result1 = ExecuteSPNonQuery(taxquery);

                }
            }
            // san bernardino
            else if (countyID == "17")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    unitNumber = unitnumber,
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                    city = cityName,
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            //Orange
            else if (countyID == "16")
            {

                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    address = streetLine,
                    unitNumber = "",
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }


            //Santa Clara
            else if (countyID == "15")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    address = streetLine,
                    assessmentid = "",
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            // WA king
            else if (countyID == "18")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    unitNumber = unitnumber,
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    direction = direction
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            //NC Forsyth
            else if (countyID == "185")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    direction = direction,
                    sttype = streetType,
                    unitNumber = unitnumber,
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            //KS Johnson
            else if (countyID == "108")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    unitNumber = unitnumber,
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            //NC Guilford
            else if (countyID == "159")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    unitNumber = unitnumber,
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            //SC spartanburg
            else if (countyID == "111")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = "",
                    accno = "",
                    unitNumber = unitnumber,
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            //WV Berkeley
            else if (countyID == "173")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "titleflex";

                }
                object input = new
                {
                    houseno = streetNumber,
                    housedir = direction,
                    sname = streetName,
                    sttype = streetType,
                    unitNumber = unitnumber,
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            //NE Sarpy
            else if (countyID == "160")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    address = streetLine,
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            //AR Saline
            else if (countyID == "174")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    unitNumber = unitnumber,
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            //Dorchester SC
            else if (countyID == "165")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    direction = direction,
                    sttype = streetType,
                    unitNumber = unitnumber,
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            //Kings NY
            else if (countyID == "164")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    direction = direction,
                    sttype = streetType,
                    city = cityName,
                    unitNumber = unitnumber,
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            //Medina OH
            else if (countyID == "186")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    address = streetLine,
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    ownername = "",
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            //Shasta CA
            else if (countyID == "192")
            {

                object input = new
                {
                    houseno = streetNumber,
                    direction = direction,
                    sname = streetName,
                    sttype = streetType,
                    unitNumber = unitnumber,
                    ownername = "",
                    parcelNumber = "",
                    searchType = "titleflex",
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            //Larimer CO 
            else if (countyID == "62")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    unitNumber = unitnumber,
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            //Jefferson
            else if (countyID == "190")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    unitNumber = unitnumber,
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }
            else if (countyID == "176")
            {
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    direction = direction,
                    sttype = streetType,
                    unitNumber = unitnumber,
                    account = "",
                    ownername = "",
                    parcelNumber = "",
                    searchType = "address",
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            //Charlotte
            else if (countyID == "178")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    direction = direction,
                    unitNumber = unitnumber,
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            //calcasieu
            else if (countyID == "181")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    direction = direction,
                    unitNumber = unitnumber,
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                    city = cityName,
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            //Fayette
            else if (countyID == "189")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    direction = direction,
                    unitNumber = unitnumber,
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            //Marion FL
            else if (countyID == "193")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    address = streetLine,
                    assessmentid = "",
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }




            //coweta
            else if (countyID == "166")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    address = streetLine,
                    assessmentid = "",
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            //williamston
            else if (countyID == "38")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    address = streetLine,
                    assessmentid = "",
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            //knox
            else if (countyID == "196")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    address = streetLine,
                    assessmentid = "",
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            //stafford
            else if (countyID == "197")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    address = streetLine,
                    assessmentid = "",
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            //AB Team Counties Started.....
            //Adams
            else if (countyID == "69")
            {
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                    streetNumber = "";
                }
                object input = new
                {
                    address = taddress,
                    StreetNumber = streetNumber,
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,
                    ParcelNumber = "",
                    AccountNumber = ""
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }
            //beaufort
            else if (countyID == "148")
            {

                object input = new
                {
                    Address = streetLine,
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,
                    ParcelNumber = "",
                    OwnerName = "",
                    AlternateID = ""
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }
            //cobb
            else if (countyID == "27")
            {

                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                    streetNumber = "";
                    streetName = "";
                }
                object input = new
                {
                    Address = taddress,
                    StreetNumber = streetNumber,
                    StreetName = streetName,
                    ParcelNumber = "",
                    OwnerName = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,

                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }

            //anne
            else if (countyID == "44")
            {
                string stname = streetName;
                if (stname.Contains(" DR") || stname.Contains(" RD") || stname.Contains(" ST") || stname.Contains(" CT") || stname.Contains(" CIR") || stname.Contains(" AVE") || stname.Contains(" LN") || stname.Contains(" PL") || stname.Contains(" HWY") || stname.Contains(" BLVD"))
                {
                    stname = stname.Replace(" DR", "").Replace(" RD", "").Replace(" ST", "").Replace(" CT", "").Replace(" CIR", "").Replace(" AVE", "").Replace(" LN", "").Replace(" PL", "").Replace(" HWY", "").Replace("BLVD", "");
                }
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                    streetName = "";
                    streetNumber = "";
                }
                object input = new
                {
                    Address = taddress,
                    StreetNumber = streetNumber,
                    StreetName = stname,
                    DistrictCode = "",
                    SubDivision = "",
                    AccountNumber = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,

                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }

            //Cuyahoga
            else if (countyID == "26")
            {
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                }
                else
                {
                    taddress = streetNumber + " " + streetName;
                }
                object input = new
                {
                    Address = taddress,
                    ParcelNumber = "",
                    OwnerName = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,

                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }

            //Montgomery
            else if (countyID == "25")
            {
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                    streetName = "";
                    streetNumber = "";
                }

                object input = new
                {
                    Address = taddress,
                    StreetNumber = streetNumber,
                    StreetName = streetName,
                    DistrictCode = "",
                    AccountNumber = "",
                    ParcelNumber = "",
                    OwnerName = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,

                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }

            //Honolulu - HI
            else if (countyID == "47")
            {

                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                    streetNumber = "";
                    streetName = "";
                    unitnumber = "";
                    direction = "";
                }
                object input = new
                {
                    taddress = streetLine,
                    StreetNumber = streetNumber,
                    StreetName = streetName,
                    UnitNumber = unitnumber,
                    Direction = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,

                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }

            //Jackson - MO
            else if (countyID == "45")
            {
                object input = new
                {
                    Address = streetLine,
                    OwnerName = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,

                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }

            //Lee - FL
            else if (countyID == "46")
            {
                string stname = streetName;

                propertyAddress = streetNumber + " " + streetName;
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    if (streetLine.Contains(" FT") || streetLine.Contains(" CPE"))
                    {
                        streetLine = streetLine.Replace(" FT", "").Replace("CPE", "");
                        propertyAddress = streetLine.Replace("\r\n", " ");
                    }
                    else
                    {
                        propertyAddress = streetLine.Replace("\r\n", " ");
                    }

                }

                object input = new
                {
                    Address = propertyAddress,
                    OwnerName = "",
                    ParcelNumber = "",
                    Folio = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,

                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }


            //Pima AZ
            else if (countyID == "24")
            {
                streetName = streetName + " " + streetType;
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                    streetNumber = "";
                    streetType = "";
                    streetName = "";
                    unitnumber = "";
                    direction = "";
                }

                object input = new
                {
                    address = taddress,
                    StreetNumber = streetNumber,
                    StreetName = streetName,
                    Direction = direction,
                    SubDivision = "",
                    OwnerName = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,

                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }

            //Pinellas - FL
            else if (countyID == "50")
            {
                object input = new
                {
                    Address = streetLine,
                    OwnerName = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,

                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }

            //Snohomish - WA
            else if (countyID == "28")
            {

                if (postdir != "")
                {
                    streetName = streetName + " " + streetType + " " + postdir;
                }
                else
                {
                    streetName = streetName + " " + streetType;
                }
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                    streetNumber = "";
                    streetName = "";
                    streetType = "";
                    postdir = "";
                }
                object input = new
                {
                    Address = taddress,
                    StreetNumber = streetNumber,
                    StreetName = streetName,
                    OwnerName = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,

                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }


            //Thurston WA
            else if (countyID == "91")
            {

                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                    streetNumber = "";
                    streetName = "";
                    streetType = "";
                    postdir = "";
                }
                object input = new
                {
                    Address = taddress,
                    StreetNumber = streetNumber,
                    StreetName = streetName,
                    OrganizationName = "",
                    OwnerLastName = "",
                    OwnerFirstName = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }

            //York SC
            else if (countyID == "71")
            {
                string stname = streetName;
                if (stname.Contains(" DR") || stname.Contains(" RD") || stname.Contains(" ST") || stname.Contains(" CT") || stname.Contains(" CIR") || stname.Contains(" TRL"))
                {
                    stname = stname.Replace(" DR", "").Replace(" RD", "").Replace(" ST", "").Replace(" CT", "").Replace(" CIR", "").Replace(" TRL", "");
                }

                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    propertyAddress = streetLine;
                }
                else
                {
                    propertyAddress = streetNumber + " " + stname;
                }
                object input = new
                {
                    Address = propertyAddress,
                    OwnerName = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }
            //brevard FL
            else if (countyID == "65")
            {
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    propertyAddress = streetLine;
                }
                else
                {
                    if (direction != "")
                    {
                        propertyAddress = streetNumber + " " + direction + " " + streetName + " " + streetType;
                    }
                    else
                    { propertyAddress = streetNumber + " " + streetName + " " + streetType; }

                }
                object input = new
                {
                    Address = propertyAddress,
                    OwnerName = "",
                    ParcelNumber = "",
                    AccountNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,

                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }


            //Elpasco CO
            else if (countyID == "66")
            {
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                    streetNumber = "";
                    streetName = "";
                    streetType = "";
                    direction = "";
                }
                object input = new
                {
                    address = taddress,
                    StreetName = streetName,
                    HouseNumberFrom = streetNumber,
                    HouseNumberTo = streetNumber,
                    StreetNumber = "",
                    StreetType = streetType,
                    Direction = direction,
                    OwnerLastName = "",
                    OwnerFirstName = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,

                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }

            //Horry SC
            else if (countyID == "70")
            {
                object input = new
                {
                    Address = streetLine,
                    OwnerName = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,

                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }



            //Benton AR
            else if (countyID == "84")
            {
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                    streetNumber = "";
                    streetName = "";
                    cityName = "";
                }
                object input = new
                {

                    StreetNumber = streetNumber,
                    StreetName = streetName,
                    City = "",
                    Address = taddress,
                    OwnerName = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }

            //Henry GA
            else if (countyID == "87")
            {
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                    streetNumber = "";
                    streetName = "";
                    streetType = "";

                }
                object input = new
                {
                    address = taddress,
                    StreetNumber = streetNumber,
                    StreetName = streetName,
                    StreetType = streetType,
                    OwnerName = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }
            //Richland SC..
            else if (countyID == "90")
            {
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    propertyAddress = streetLine;
                }
                else
                {
                    propertyAddress = streetNumber + " " + streetName;
                }
                object input = new
                {
                    Address = propertyAddress,
                    TaxNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }



            //Wed CO
            else if (countyID == "89")
            {
                object input = new
                {
                    Address = streetLine,
                    AccountNumber = "",
                    OwnerName = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,


                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }
            //frederick MD

            else if (countyID == "107")
            {
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                    streetName = "";
                    streetNumber = "";
                }
                object input = new
                {
                    Address = streetLine,
                    StreetNumber = streetNumber,
                    StreetName = streetName,
                    DistrictCode = "",
                    AccountNumber = "",
                    OwnerName = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }
            //Hawai HI
            else if (countyID == "104")
            {
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                    streetNumber = "";
                    streetName = "";
                    streetType = "";
                    direction = "";
                    unitnumber = "";
                }
                object input = new
                {
                    address = taddress,
                    StreetNumber = streetNumber,
                    StreetName = streetName,
                    StreetType = streetType,
                    Direction = direction,
                    UnitNumber = unitnumber,
                    OwnerName = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }
            //Lake OH

            else if (countyID == "118")
            {
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                    streetNumber = "";
                    streetName = "";
                }
                object input = new
                {
                    Address = taddress,
                    StreetNumber = streetNumber,
                    StreetName = streetName,
                    OwnerLastName = "",
                    OwnerFirstName = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }


            //Mobile AL

            else if (countyID == "105")
            {
                object input = new
                {
                    Address = streetLine,
                    OwnerLastName = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }

            //seminole

            else if (countyID == "110")
            {
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                    streetName = "";
                    streetNumber = "";
                    direction = "";
                }

                object input = new
                {
                    Address = taddress,
                    StreetNumber = streetNumber,
                    StreetName = streetName,
                    Direction = direction,
                    OwnerName = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }

            //Forsyth
            else if (countyID == "126")
            {
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                    streetNumber = "";
                    streetName = "";
                    streetType = "";
                    direction = "";
                    unitnumber = "";
                }

                object input = new
                {
                    address = taddress,
                    StreetNumber = streetNumber,
                    StreetName = streetName,
                    StreetType = streetType,
                    Direction = direction,
                    UnitNumber = unitnumber,
                    OwnerName = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }

            //LakeIndiana

            else if (countyID == "136")
            {

                object input = new
                {
                    Address = streetLine,
                    OwnerName = "",
                    ParcelNumber = "",
                    TaxNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }

           

            //Lucas
            else if (countyID == "131")
            {
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                    streetNumber = "";
                    streetName = "";

                }
                object input = new
                {
                    address = taddress,
                    StreetNumber = streetNumber,
                    StreetName = streetName,
                    AssessorNumber = "",
                    OwnerName = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }




            //Baldwin


            else if (countyID == "167")
            {
                streetName = streetNumber + " " + streetType;
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                    streetNumber = "";
                    streetType = "";
                    streetName = "";
                }

                object input = new
                {
                    Address = taddress,
                    StreetNumber = streetNumber,
                    StreetName = streetName,
                    PPIN = "",
                    OwnerName = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }






            //DeSoto
            else if (countyID == "144")
            {
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                    streetNumber = "";
                    streetName = "";
                }

                object input = new
                {
                    Address = taddress,
                    StreetNumber = streetNumber,
                    StreetName = streetName,
                    OwnerName = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }



            //Douglas
            else if (countyID == "145")
            {
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                    streetNumber = "";
                    streetType = "";
                    streetName = "";
                    direction = "";
                }

                object input = new
                {
                    OwnerName = "",
                    TaxNumber = "",
                    StreetType = streetType,
                    Address = taddress,
                    Direction = direction,
                    StreetNumber = streetNumber,
                    StreetName = streetName,
                    HouseNumberFrom = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }



            //kane
            else if (countyID == "168")
            {
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                    streetNumber = "";
                    streetType = "";
                    streetName = "";
                }

                object input = new
                {
                    address = taddress,
                    OwnerName = "",
                    StreetName = streetName,
                    HouseNumberFrom = streetNumber,
                    HouseNumberTo = streetNumber,
                    City = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }


            //LakeIllinois
            else if (countyID == "169")
            {

                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                    streetName = "";
                    streetNumber = "";
                }
                object input = new
                {
                    Address = taddress,
                    StreetNumber = streetNumber,
                    StreetName = streetName,
                    Zipcode = "",
                    OwnerName = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }

            //Maui
            else if (countyID == "147")
            {
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                    streetName = "";
                    streetNumber = "";
                    streetType = "";
                    direction = "";
                }
                object input = new
                {
                    OwnerName = "",
                    TaxNumber = "",
                    StreetType = streetType,
                    Address = taddress,
                    Direction = direction,
                    StreetNumber = streetNumber,
                    StreetName = streetName,
                    HouseNumberFrom = "",
                    ParcelNumber = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }

            //McHenry
            else if (countyID == "172")
            {
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                    streetName = "";
                    streetNumber = "";
                }
                object input = new
                {
                    address = taddress,
                    StreetName = streetName,
                    HouseNumberFrom = streetNumber,
                    city = "",
                    ParcelNumber = "",
                    OwnerName = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }




            //Paulding

            else if (countyID == "146")
            {
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    taddress = streetLine;
                    streetNumber = "";
                    streetType = "";
                    streetName = "";
                    direction = "";
                }

                object input = new
                {
                    ParcelNumber = "",
                    TaxNumber = "",
                    Address = taddress,
                    OwnerName = "",
                    StreetName = streetName,
                    StreetNumber = streetNumber,
                    StreetType = streetType,
                    Direction = direction,
                    HouseNumberFrom = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }






            //Washington
            else if (countyID == "170")
            {
                object input = new
                {


                    ParcelNumber = "",
                    Address = streetLine,
                    OwnerName = "",
                    County = "",
                    CountyID = "",
                    OrderID = orderNumber,
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatus(json, orderNumber);
            }

            //Hall
            else if (countyID == "204")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    address = streetLine,
                    assessmentid = "",
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }


            //St John
            else if (countyID == "175")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }
                object input = new
                {
                    address = streetLine,
                    ownername = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            //Kootenai
            else if (countyID == "200")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    streetno = streetNumber,
                    streetname = streetName,
                    streettype = streetType,
                    direction = direction,
                    city = cityName,
                    unitnumber = unitnumber,
                    ownernm = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            //Lubbock
            else if (countyID == "154")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    streetno = streetNumber,
                    streetname = streetName,
                    streettype = streetType,
                    direction = direction,
                    unitnumber = unitnumber,
                    ownernm = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            //Hidalgo
            else if (countyID == "85")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    streetno = streetNumber,
                    streetname = streetName,
                    streettype = streetType,
                    direction = direction,
                    unitnumber = unitnumber,
                    ownernm = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            // TN Hamilton
            else if (countyID == "208")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    streetno = streetNumber,
                    streetname = streetName,
                    streettype = streetType,
                    direction = direction,
                    unitnumber = unitnumber,
                    ownernm = "",
                    accno = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }
            // TX Nueces
            else if (countyID == "142")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    direction = direction,
                    unitNumber = unitnumber,
                    ownernm = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }
            // WV Kanawha
            else if (countyID == "201")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    direction = direction,
                    unitNumber = unitnumber,
                    ownernm = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            // CO Boulder
            else if (countyID == "207")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";

                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    direction = direction,
                    unitNumber = unitnumber,
                    city = "",
                    ownernm = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            // WA Cowlitz
            else if (countyID == "206")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    direction = direction,
                    unitNumber = unitnumber,
                    ownernm = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            // GA Fayette
            else if (countyID == "211")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";

                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    direction = direction,
                    unitNumber = unitnumber,
                    ownernm = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            // MO St Louis City
            else if (countyID == "212")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }
                object input = new
                {
                    address = streetLine,
                    ownernm = "",
                    assessment_id = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            // Bell TX
            else if (countyID == "137")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    direction = direction,
                    unitNumber = unitnumber,
                    ownernm = "",
                    assessment_id = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            // OH Delaware
            else if (countyID == "205")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    direction = direction,
                    unitNumber = unitnumber,
                    city = "",
                    ownernm = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }
            // TX Ellis
            else if (countyID == "161")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    direction = direction,
                    unitNumber = unitnumber,
                    city = "",
                    ownernm = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            // OH  Mahoning
            else if (countyID == "214")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    direction = direction,
                    unitNumber = unitnumber,
                    ownernm = "",
                    city = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }
            // CO Araphoe
            else if (countyID == "48")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    direction = direction,
                    unitNumber = unitnumber,
                    city = "",
                    ownernm = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }

            // TX comal
            else if (countyID == "180")
            {
                string sear_type = "";
                if (streetLine.Contains("APT") || streetLine.Contains("UNIT") || streetLine.Contains("#"))
                {
                    sear_type = "titleflex";
                }
                else
                {
                    sear_type = "address";
                }
                object input = new
                {
                    houseno = streetNumber,
                    sname = streetName,
                    sttype = streetType,
                    direction = direction,
                    unitNumber = unitnumber,
                    ownernm = "",
                    parcelNumber = "",
                    searchType = sear_type,
                    orderNumber = orderNumber,
                    directParcel = "",
                };
                json = ScrapOrder(apiUrl, input);
                updateRecordStatusSST(json, orderNumber);
            }
            return json;
           //last
            #endregion

        }
        public void updateRecordStatusSST(string json, string orderNumber)
        {
            if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout"))
            {
                // 2 completed                        
                string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                int result1 = ExecuteSPNonQuery(taxquery);
            }
            else if (json.Contains("An error has occured"))
            {
                //3 == scraping error                        
                string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                int result1 = ExecuteSPNonQuery(taxquery);
            }
            else
            {
                //1 == Multiparcel
                string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                int result1 = ExecuteSPNonQuery(taxquery);

            }
        }

        public string GetOrderDetails(string apiUrl, object inputObj)
        {

            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Thread.Sleep(10000);
                var response = client.PostAsJsonAsync(apiUrl, inputObj).Result;
                var result = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return "An error has occured";
                }
                else
                {
                    return result.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public string ScrapOrder(string apiUrl, object inputObj)
        {

            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.PostAsJsonAsync(apiUrl, inputObj).Result;
                // response.EnsureSuccessStatusCode();                        
                var result = response.Content.ReadAsStringAsync().Result;
                JavaScriptSerializer js = new JavaScriptSerializer();
                dynamic blogObject = js.Deserialize<dynamic>(result);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return "An error has occured";
                }
                else
                {
                    return blogObject.ToString();
                }
            }
            catch
            {
                return "Timeout";
            }
        }
        public void updateRecordStatus(string json, string orderNumber)
        {
            if (json.Contains("Success") || json.Contains("Timeout"))
            {
                string taxquery = "update tbl_record_status set scrape_status = 2 where orderno = '" + orderNumber + "'";
                int result1 = ExecuteSPNonQuery(taxquery);
            }
            else if (json.Contains("MultiRecord"))
            {
                string taxquery = "update tbl_record_status set scrape_status = 5 where orderno = '" + orderNumber + "'";
                int result1 = ExecuteSPNonQuery(taxquery);
            }
            else if (json.Contains("MultiRecordInserted") || json.Contains("Order_No") || json.Contains("[]"))
            {
                string taxquery = "update tbl_record_status set scrape_status = 4 where orderno = '" + orderNumber + "'";
                int result1 = ExecuteSPNonQuery(taxquery);
            }
            else
            {
                string taxquery = "update tbl_record_status set scrape_status = 3 where orderno = '" + orderNumber + "'";
                int result1 = ExecuteSPNonQuery(taxquery);
            }
        }

        public DataSet GetCountyId(string state, string county)
        {
            using (MySqlConnection cnn = new MySqlConnection(ConfigurationManager.ConnectionStrings["lereta"].ConnectionString))
            {
                string query = "SELECT Address_Type,State_County_Id,state_name,county_name ,service_url,team FROM state_county_master where State_Name = '" + state + "' and County_Name='" + county + "'";
                cnn.Open();
                MySqlCommand cmd = new MySqlCommand(query, cnn);
                MySqlDataAdapter sda = new MySqlDataAdapter();
                DataSet ds = new DataSet();
                sda.SelectCommand = cmd;
                sda.Fill(ds);
                cnn.Close();
                return ds;
            }

        }
        public int ExecuteSPNonQuery(string Query)
        {
            int result;
            delError();
            openConnection();
            mCmd = new MySqlCommand(Query, mConnection);

            try
            {
                result = mCmd.ExecuteNonQuery();
            }
            catch (MySqlException mye)
            {
                if (mye.Number == 1062)
                {
                    setError("Duplicate Entry: Name already found.");
                }
                else
                {
                    setError(mye.Number + " " + mye.Message);
                }
                return -1;

            }
            finally
            {
                mConnection.Close();
                mConnection.Dispose();
            }
            return result;

        }
        public static void delError()
        {
            IsErr = false;
            ErrMsg = "";
        }
        private void openConnection()
        {
            mConnection = new MySqlConnection(mysqlConnection);
            if (mConnection.State == ConnectionState.Open)
            {
                mConnection.Close();
            }
            mConnection.Open();
        }
        public static void setError(string Message)
        {
            IsErr = true;
            ErrMsg = Message;
        }
        public string ReturnStType(string name)
        {
            MySqlConnection ftp = new MySqlConnection(ConfigurationManager.ConnectionStrings["MysqlConnection"].ConnectionString);
            ftp.Open();
            MySqlCommand cmd = new MySqlCommand("SELECT short_name from street_type WHERE name = '" + name.ToUpper() + "'", ftp);
            MySqlDataReader reader = cmd.ExecuteReader();
            String result = null;
            while (reader.Read())
            {
                result = reader["short_name"].ToString();
            }
            reader.Close();
            ftp.Close();
            return result;
        }

    }
    public class orderDetails
    {
        public string order_no { get; set; }
        public string pdate { get; set; }
        public string county { get; set; }
        public string state { get; set; }
        public string borrowername { get; set; }
        public string address { get; set; }
    }
}
