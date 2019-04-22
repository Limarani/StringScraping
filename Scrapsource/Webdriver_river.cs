using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Drawing.Imaging;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Drawing;
using System.Data;
using HtmlAgilityPack;
using iTextSharp.text;
using System.Text.RegularExpressions;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Support.Extensions;
using System.Net;
using OpenQA.Selenium.Firefox;
using System.Diagnostics;
using OpenQA.Selenium.Remote;


namespace ScrapMaricopa
{
    public class Webdriver_river
    {
        string Outparcelno = "", outputPath = "";
        IWebDriver driver;
        DBconnection db = new DBconnection();
        GlobalClass gc = new GlobalClass();
        string fulladdress, assyear = "", fullassess = "", hometownasses = "", totalnetasses = "", exemption_type = "Homeowner Exemption";
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ToString());
        public string FTP_river(string houseno, string sname, string sttype, string parcelNumber, string searchType, string orderNumber, string directParcel)
        {
            string StartTime = "", AssessmentTime = "", TaxTime = "", CitytaxTime = "", LastEndTime = "", AssessTakenTime = "", TaxTakentime = "", CityTaxtakentime = "";
            string TotaltakenTime = "";

            string address = houseno + " " + sname + " " + sttype;
            GlobalClass.global_orderNo = orderNumber;
            HttpContext.Current.Session["orderNo"] = orderNumber;
            GlobalClass.global_parcelNo = parcelNumber;

            var driverService = PhantomJSDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            using (driver = new PhantomJSDriver())
            {
                StartTime = DateTime.Now.ToString("HH:mm:ss");
                //if (searchType == "address")
                //{
                //    StartTime = DateTime.Now.ToString("HH:mm:ss");
                //    driver.Navigate().GoToUrl("http://www.asrclkrec.com/Assessor/AssessorServices/ValueNoticeLookup.aspx");
                //    //find ifram using xpath
                //    IWebElement iframeElement = driver.FindElement(By.XPath("//*[@id='dnn_ctr861_HtmlModule_lblContent']/iframe"));
                //    //now use the switch command
                //    driver.SwitchTo().Frame(iframeElement);
                //    driver.FindElement(By.Id("STREETNUMBER")).SendKeys(houseno);
                //    driver.FindElement(By.Id("STREETNAME")).SendKeys(sname);
                //    driver.FindElement(By.Id("ddlSNSuffix")).SendKeys(sttype);
                //    gc.CreatePdf_WOP(orderNumber, "InputPassed_AddressSearch", driver, "CA", "RiverSide");
                //    driver.FindElement(By.Id("btnSearchDetails")).SendKeys(Keys.Enter);

                //}
                //if (searchType == "titleflex")
                //{
                //    string address = houseno + " " + sname + " " + sttype;
                //    gc.TitleFlexSearch(orderNumber,"", directParcel, address, "CA", "Riverside");

                //    if ((HttpContext.Current.Session["TitleFlex_Search"] != null && HttpContext.Current.Session["TitleFlex_Search"].ToString() == "Yes"))
                //    {
                //        return "MultiParcel";
                //    }

                //    //searchType = "parcel";
                //    parcelNumber = HttpContext.Current.Session["titleparcel"].ToString();
                //    Outparcelno = parcelNumber.Replace("-", "");
                //}

                //if (searchType == "parcel")
                //{
                //    driver.Navigate().GoToUrl("http://www.asrclkrec.com/Assessor/AssessorServices/ValueNoticeLookup.aspx");
                //    //find ifram using xpath
                //    IWebElement iframeElement = driver.FindElement(By.XPath("//*[@id='dnn_ctr861_HtmlModule_lblContent']/iframe"));
                //    //now use the switch command
                //    driver.SwitchTo().Frame(iframeElement);
                //    driver.FindElement(By.Id("txtAssessment")).SendKeys(parcelNumber);
                //    gc.CreatePdf(orderNumber, parcelNumber, "InputPassed_Parselsearch", driver, "CA", "RiverSide");
                //    driver.FindElement(By.Id("btnSearchDetails")).SendKeys(Keys.Enter);

                //}


                //try
                //{
                //    IWebElement fulltext = driver.FindElement(By.XPath("//*[@id='content-left']/div/div/div/table[1]"));
                //    string Sfulltext = fulltext.Text.ToString();
                //    assyear = Between(Sfulltext, "For the ", " assessment year").Trim();
                //}

                //catch (NoSuchElementException ex1)
                //{
                //    driver.Quit();
                //    throw ex1;
                //}

                //IWebElement webparcelno = driver.FindElement(By.Id("txtParcelNum"));
                //Outparcelno = webparcelno.GetAttribute("value");
                //IWebElement webaddr1 = driver.FindElement(By.Id("txtPropertyAddress1"));
                //string addr1 = webaddr1.GetAttribute("value");
                //IWebElement webaddr2 = driver.FindElement(By.Id("txtPropertyAddress2"));
                //string addr2 = webaddr2.GetAttribute("value");
                //fulladdress = addr1 + "," + addr2;

                //IWebElement webfullassessed = driver.FindElement(By.Id("txtFullValAssessed"));
                //fullassess = webfullassessed.GetAttribute("value");
                //IWebElement webhometown = driver.FindElement(By.Id("txtHomeExemptAssessedVal"));
                //hometownasses = webhometown.GetAttribute("value");
                //IWebElement webnetasses = driver.FindElement(By.Id("txtTotalAssessedVal"));
                //totalnetasses = webnetasses.GetAttribute("value");

                ////db.ExecuteQuery("insert into la_assessor (order_no,parcel_no,property_address,year,total,exemption_type,exempt_value,net_asses_value) values ('" + orderNumber + "','" + Outparcelno + "','" + fulladdress + "','" + assyear + "','" + fullassess + "','" + exemption_type + "','" + hometownasses + "','" + totalnetasses + "')");


                //if (searchType == "address")
                //{
                //    parcelNumber = Outparcelno;
                //}
                //gc.CreatePdf(orderNumber, parcelNumber, "Assessor", driver, "CA", "RiverSide");
                //tax details
                AssessmentTime = DateTime.Now.ToString("HH:mm:ss");
                try
                {
                    driver.Navigate().GoToUrl("https://taxpayments.co.riverside.ca.us/taxpayments/Search.aspx");
                    driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_ParcelStreetLine1")).SendKeys(address);
                    driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_SearchButton")).SendKeys(Keys.Enter);
                    gc.CreatePdf(orderNumber, parcelNumber, "TaxSearch_Results", driver, "CA", "RiverSide");
                    string supplemental = "";
                    try
                    {
                        supplemental = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_lblSuppl")).Text;
                    }
                    catch
                    {

                    }
                    string prior = "";
                    try
                    {
                        prior = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_lblSecuredDelinq")).Text;
                    }
                    catch { }


                    driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_SecuredTaxGridView_ctl02_ViewDetails")).Click();
                    insertInstallment(orderNumber, parcelNumber, "Taxbill_secured", "secured");

                    if (prior.Contains("Prior Year Assessments"))
                    {
                        driver.Navigate().GoToUrl("https://taxpayments.co.riverside.ca.us/taxpayments/Search.aspx");
                        driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_tbAssessmentNumber")).SendKeys(Outparcelno);
                        driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_SearchButton")).SendKeys(Keys.Enter);
                        driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_SecuredDelinquentTaxGridView_ctl02_btnSecDelPaymentOptions")).SendKeys(Keys.Enter);
                        Thread.Sleep(2000);
                        driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_btnSecDelTaxInfo")).SendKeys(Keys.Enter);
                        gc.CreatePdf(orderNumber, Outparcelno, "Secured Prior Year Assessments", driver, "CA", "RiverSide");
                        Thread.Sleep(2000);
                        string TaxYear = "", Taxes = "", Costs = "", Penalties = "", Total = "";
                        //ctl00_ContentPlaceHolder1_gvSecuredDelinqTaxSum
                        IWebElement taxbilltable = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_gvSecuredDelinqTaxSum"));
                        IList<IWebElement> taxbilltableRow = taxbilltable.FindElements(By.TagName("tr"));
                        int rowcount = taxbilltableRow.Count;
                        IList<IWebElement> taxbillrowTD;

                        foreach (IWebElement rowid in taxbilltableRow)
                        {
                            taxbillrowTD = rowid.FindElements(By.TagName("td"));
                            if (taxbillrowTD.Count != 0 && !rowid.Text.Contains("Tax Year"))
                            {
                                string prioryear = taxbillrowTD[0].Text + "~" + taxbillrowTD[2].Text + "~" + taxbillrowTD[3].Text + "~" + taxbillrowTD[4].Text + "~" + taxbillrowTD[5].Text;
                                gc.insert_date(orderNumber, parcelNumber, 81, prioryear, 1, DateTime.Now);

                            }
                        }
                    }
                    //supplemental informarions...
                    if (supplemental.Contains("Supplemental"))
                    {
                        driver.Navigate().GoToUrl("https://taxpayments.co.riverside.ca.us/taxpayments/Search.aspx");
                        driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_tbAssessmentNumber")).SendKeys(parcelNumber);
                        driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_SearchButton")).SendKeys(Keys.Enter);

                        driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_SupplementalTaxGridView_ctl02_ViewDetails")).SendKeys(Keys.Enter);
                        Thread.Sleep(2000);
                        gc.CreatePdf(orderNumber, parcelNumber, "Supplemental_TaxDetails", driver, "CA", "RiverSide");
                        parcelNumber = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_CellAssessNum")).Text;
                        insertInstallment(orderNumber, parcelNumber, "Taxbill_supplemental", "supplemental");
                    }
                    TaxTime = DateTime.Now.ToString("HH:mm:ss");
                    LastEndTime = DateTime.Now.ToString("HH:mm:ss");
                    gc.insert_TakenTime(orderNumber, "CA", "RiverSide", StartTime, AssessmentTime, TaxTime, CitytaxTime, LastEndTime);

                    driver.Quit();
                    GlobalClass.titleparcel = "";
                    gc.mergpdf(orderNumber, "CA", "RiverSide");
                    return "Data Inserted Successfully";
                }
                catch (FormatException ex2)
                {
                    //Website Not Working 
                    if(ex2.ToString().Contains(""))
                    //
                    driver.Quit();
                    GlobalClass.LogError(ex2, orderNumber);
                    throw ex2;
                }
                
            }

        }
        public void insertInstallment(string orderNumber, string parcelNumber, string fileName, string taxType)
        {
            string tax_type = "-", tax_year = "-", land = "-", build = "-", tax_ratearea = "-", legal_description = "-", inst_type = "-", due_date = "-", tax_status = "-", amount_due = "-", Penalties_Due = "-", Additional_Fees_Due = "-", Paid_Amount = "-", Paid_Date = "-", Total_Due_Amount = "-", delinquent = "-", output = "";

            string total_assessement = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_TableCell15")).Text;
            tax_type = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_lblDetailsTitle")).Text;
            output = tax_type.Substring(tax_type.LastIndexOf("YEAR") + 4);
            land = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_TableCell10")).Text;
            build = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_TableCell11")).Text;
            tax_ratearea = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_TaxRateArea")).Text;
            legal_description = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_PropertyData")).Text;
            parcelNumber = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_CellAssessNum")).Text;
            string prop_addr = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_SitusAddress")).Text;
            totalnetasses = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_TableCell186")).Text;

            string[] yrArray = output.Split('-');
            string yr1 = WebDriverTest.After(yrArray[0], ",").TrimStart();
            string yr2 = WebDriverTest.After(yrArray[1], ","); ;
            tax_year = yr1 + "-" + yr2;
            if (taxType != "supplemental")
            {
                string property_details = prop_addr + "~" + legal_description;
                gc.insert_date(orderNumber, parcelNumber, 30, property_details, 1, DateTime.Now);
            }

            if (taxType == "secured")
            {
                string assess = tax_year + "~" + land + "~" + build + "~" + fullassess + "~" + total_assessement + "~" + hometownasses + "~" + totalnetasses;
                gc.insert_date(orderNumber, parcelNumber, 31, assess, 1, DateTime.Now);
                //db.ExecuteQuery("update la_assessor set land ='" + land + "', improvements = '" + build + "',legal_description = '" + legal_description + "',total_assessment='" + total_assessement + "' where order_no ='" + orderNumber + "'");
            }
            //Assessment_Year~Land~Building/Structure~Full_Value~Total_Assessed_Value~Homeowner_Exemption~Net_Assessed_Value
            string assess_sup = tax_year + "~" + land + "~" + build + "~" + fullassess + "~" + total_assessement + "~" + hometownasses + "~" + totalnetasses;
            // gc.insert_date(orderNumber,DateTime.Now , parcelNumber, 31, assess_sup, 1);
            //tax payemnt history
            tax_type = Between(tax_type, "RIVERSIDE COUNTY", "PROPERTY TAX");
            //tax_year = output;


            inst_type = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_TableCell22")).Text;
            if (inst_type.Contains("1st"))
            {
                due_date = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_Inst1DueDate")).Text;
                tax_status = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_Inst1Status")).Text;
                amount_due = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_Inst1TaxesDue")).Text;
                Penalties_Due = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_Inst1PenaltiesDue")).Text;
                Additional_Fees_Due = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_Inst1AddlFeesDue")).Text;
                Total_Due_Amount = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_Inst1TotalDue")).Text;
                //Tax_Rate_Area~Tax_Year~Tax_Type~Installment_Type~Due_Date~Tax_Status~Amount_Due~Penalties_Due~Additional_Fees_Due~Paid_ Amount~Paid_Date~Total_Due_Amount
                string ins1 = tax_ratearea + "~" + tax_year + "~" + tax_type + "~" + inst_type + "~" + due_date + "~" + tax_status + "~" + amount_due + "~" + Penalties_Due + "~" + Additional_Fees_Due + "~" + Paid_Amount + "~" + Paid_Date + "~" + Total_Due_Amount;
                gc.insert_date(orderNumber, parcelNumber, 32, ins1, 1, DateTime.Now);
                //db.ExecuteQuery("insert into la_tax_summary (parcel_no, order_no,installment,due_date, tax_amount, tax_status,balance_due,Penalties_Due,total_due,paid_amount,delinquent,tax_year,add_charges,tax_type,tax_ratearea) values ('" + parcelNumber + "','" + orderNumber + "', '" + inst_type + "','" + due_date + "','" + amount_due + "','" + tax_status + "','','" + Penalties_Due + "','" + Total_Due_Amount + "','" + Paid_Amount + "','" + delinquent + "','" + tax_year + "','" + Additional_Fees_Due + "','" + tax_type + "','" + tax_ratearea + "') ");
            }
            inst_type = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_TableCell25")).Text;
            if (inst_type.Contains("2nd"))
            {
                due_date = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_Inst2DueDate")).Text;
                tax_status = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_Inst2Status")).Text;
                amount_due = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_Inst2TaxesDue")).Text;
                Penalties_Due = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_Inst2PenaltiesDue")).Text;
                Additional_Fees_Due = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_Inst2AddlFeesDue")).Text;
                Total_Due_Amount = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_Inst2TotalDue")).Text;
                string ins2 = tax_ratearea + "~" + tax_year + "~" + tax_type + "~" + inst_type + "~" + due_date + "~" + tax_status + "~" + amount_due + "~" + Penalties_Due + "~" + Additional_Fees_Due + "~" + Paid_Amount + "~" + Paid_Date + "~" + Total_Due_Amount;
                gc.insert_date(orderNumber, parcelNumber, 32, ins2, 1, DateTime.Now);
                //db.ExecuteQuery("insert into la_tax_summary (parcel_no, order_no,installment,due_date, tax_amount, tax_status,balance_due,Penalties_Due,total_due,paid_amount,delinquent,tax_year,add_charges,tax_type,tax_ratearea) values ('" + parcelNumber + "','" + orderNumber + "', '" + inst_type + "','" + due_date + "','" + amount_due + "','" + tax_status + "','','" + Penalties_Due + "','" + Total_Due_Amount + "','" + Paid_Amount + "','" + delinquent + "','" + tax_year + "','" + Additional_Fees_Due + "','" + tax_type + "','" + tax_ratearea + "') ");
                gc.CreatePdf(orderNumber, Outparcelno, "tax_details", driver, "CA", "RiverSide");


            }
        }

        public string Between(string Text, string FirstString, string LastString)
        {

            string STR = Text;
            string STRFirst = FirstString;
            string STRLast = LastString;
            string FinalString;
            int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
            int Pos2 = STR.IndexOf(LastString);
            FinalString = STR.Substring(Pos1, Pos2 - Pos1);
            FinalString = FinalString.Replace(System.Environment.NewLine, string.Empty);
            return FinalString;

        }

    }
}