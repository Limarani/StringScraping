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
using System.ComponentModel;
using System.Text;
using HtmlAgilityPack;
using iTextSharp.text;
using System.Text.RegularExpressions;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Support.Extensions;
using System.Net;
using OpenQA.Selenium.Firefox;
using System.Diagnostics;
using OpenQA.Selenium.Remote;
using Org.BouncyCastle.Utilities;

namespace ScrapMaricopa.Scrapsource
{
    public class Webdriver_QueensNY
    {
        IWebDriver driver;
        DBconnection db = new DBconnection();
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ToString());
        GlobalClass gc = new GlobalClass();
        string Yearbuild, parcel_number, Addressmax, parcelsplit1, parcelsplit2, DUE, Owner_Name, multiparcel = "";
        int value = 0;
        string[] ParcelSplit; IWebElement PropertyValidation;
        public string FTP_QueensNY(string streetno, string direction, string streetname, string streettype, string unitnumber, string ownernm, string parcelNumber, string searchType, string orderNumber, string directParcel)
        {
            GlobalClass.global_orderNo = orderNumber;
            HttpContext.Current.Session["orderNo"] = orderNumber;
            GlobalClass.global_parcelNo = parcelNumber;
            var driverService = PhantomJSDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            // driver = new ChromeDriver();
            //driver = new PhantomJSDriver();
            string StartTime = "", AssessmentTime = "", TaxTime = "", CitytaxTime = "", LastEndTime = "";
            using (driver = new PhantomJSDriver())
            {
                try
                {
                    if (searchType == "titleflex")
                    {
                        string Address = "";
                        if (direction != "")
                        {
                            Address = streetno.Trim() + " " + direction.ToUpper().Trim() + " " + streetname.ToUpper().Trim() + " " + streettype.ToUpper().Trim();
                        }
                        else
                        {
                            Address = streetno + " " + streetname + " " + streettype + " " + unitnumber;
                        }
                        gc.TitleFlexSearch(orderNumber, "", ownernm, Address, "NY", "Queens");
                        if ((HttpContext.Current.Session["TitleFlex_Search"] != null && HttpContext.Current.Session["TitleFlex_Search"].ToString() == "Yes"))
                        {
                            return "MultiParcel";
                        }
                        searchType = "parcel";
                        parcelNumber = HttpContext.Current.Session["titleparcel"].ToString().Replace(".", "");
                    }
                    StartTime = DateTime.Now.ToString("HH:mm:ss");
                    driver.Navigate().GoToUrl("http://nycserv.nyc.gov/NYCServWeb/NYCSERVMain");
                    driver.FindElement(By.XPath("/html/body/form[1]/center/table[2]/tbody/tr[1]/td/table/tbody/tr/td[1]/center/table/tbody/tr[3]/td/table/tbody/tr[3]/td[2]/input")).SendKeys(Keys.Enter);
                    Thread.Sleep(2000);
                    if (searchType == "address")
                    {
                        driver.FindElement(By.XPath("/html/body/center/form/table[2]/tbody/tr/td/table/tbody/tr[6]/td[3]/input")).SendKeys(streetno.Trim());
                        driver.FindElement(By.XPath("/html/body/center/form/table[2]/tbody/tr/td/table/tbody/tr[8]/td[3]/input[1]")).SendKeys(direction.ToUpper() + " " + streetname + " " + streettype.ToUpper());
                        driver.FindElement(By.XPath("/html/body/center/form/table[2]/tbody/tr/td/table/tbody/tr[8]/td[3]/input[2]")).SendKeys(unitnumber);
                        IWebElement Icity = driver.FindElement(By.XPath("/html/body/center/form/table[2]/tbody/tr/td/table/tbody/tr[4]/td[2]/input[4]"));
                        Icity.Click();
                        Thread.Sleep(2000);
                        driver.FindElement(By.XPath("/html/body/center/form/table[2]/tbody/tr/td/table/tbody/tr[8]/td[3]/img[2]")).Click();
                        Thread.Sleep(6000);
                        gc.CreatePdf_WOP(orderNumber, "Address", driver, "NY", "Queens");
                        string AddressMerge;
                        if (streettype.ToUpper() == "ST")
                        {
                            streettype = "STREET";
                        }
                        if (streettype.ToUpper() == "CT")
                        {
                            streettype = "COURT";
                        }
                        if (streettype.ToUpper() == "RD")
                        {
                            streettype = "ROAD";
                        }
                        if (streettype.ToUpper() == "LN")
                        {
                            streettype = "LANE";
                        }
                        if (streettype.ToUpper() == "PKWY")
                        {
                            streettype = "PARKWAY";
                        }
                        if (streetname.Any(char.IsDigit))
                        {
                            streetname = Regex.Match(streetname, @"\d+").Value;
                        }
                        if (direction != "")
                        {
                            if (direction == "E")
                            {
                                direction = "EAST";
                            }
                            if (direction == "N")
                            {
                                direction = "NORTH";
                            }
                            if (direction == "S")
                            {
                                direction = "SOUTH";
                            }

                            AddressMerge = streetno.Trim() + " " + direction.ToUpper().Trim() + " " + streetname.ToUpper().Trim() + " " + streettype.ToUpper().Trim();
                        }
                        else
                        {
                            AddressMerge = streetno.Trim() + " " + streetname.ToUpper().Trim() + " " + streettype.ToUpper().Trim();
                        }

                        string Firststep = ""; int Max = 0;
                        IWebElement PropertyIteamTable = driver.FindElement(By.XPath("/html/body/center/form[1]/table[2]/tbody/tr[2]/td/table/tbody"));
                        IList<IWebElement> PropertyIeamRow = PropertyIteamTable.FindElements(By.TagName("tr"));
                        IList<IWebElement> PropertyIteamid;
                        foreach (IWebElement Property in PropertyIeamRow)
                        {
                            PropertyIteamid = Property.FindElements(By.TagName("td"));
                            if (PropertyIteamid.Count != 0 && !Property.Text.Contains("Apartment") && PropertyIteamid[0].Text != multiparcel)
                            {
                                if (PropertyIteamid[1].Text.Trim().Contains(streetno) && PropertyIteamid[1].Text.Trim().Contains(direction.ToUpper()) && PropertyIteamid[1].Text.Trim().Contains(streetname.ToUpper()) && PropertyIteamid[1].Text.Trim().Contains(streettype.ToUpper()) && PropertyIteamid[2].Text.Trim().Contains(unitnumber))
                                {
                                    PropertyValidation = PropertyIteamid[0].FindElement(By.TagName("a"));
                                    string Validation = PropertyValidation.GetAttribute("href");
                                    multiparcel = PropertyIteamid[0].Text;
                                    //Firststep.Add(Validation);
                                    Max++;
                                    Firststep = PropertyIteamid[1].Text + "~" + PropertyIteamid[3].Text;
                                    gc.insert_date(orderNumber, PropertyIteamid[0].Text, 1473, Firststep, 1, DateTime.Now);
                                }

                            }
                        }
                        if (Max == 1)
                        {
                            PropertyValidation.Click();
                            Thread.Sleep(2000);

                        }
                        if (Max > 1 && Max < 26)
                        {
                            HttpContext.Current.Session["multiparcel_QueensNY"] = "Yes";
                            gc.CreatePdf_WOP(orderNumber, "MultyAddressSearch", driver, "NY", "Queens");
                            driver.Quit();
                            return "MultiParcel";
                        }
                        if (Max > 25)
                        {
                            HttpContext.Current.Session["multiParcel_QueensNY_Multicount"] = "Maximum";
                            gc.CreatePdf_WOP(orderNumber, "MultyAddressSearch", driver, "NY", "Queens");
                            driver.Quit();
                            return "Maximum";
                        }
                        if (Max == 0)
                        {
                            HttpContext.Current.Session["Zero_QueensNY"] = "Zero";
                            gc.CreatePdf_WOP(orderNumber, "MultyAddressSearch", driver, "NY", "Queens");
                            driver.Quit();
                            return "No Records Found";

                        }
                    }
                    if (searchType == "parcel")
                    {
                        if (parcelNumber != "")
                        {
                            string[] ParcelSplitP = parcelNumber.Split('-', '/');
                            string parcelsplit1P = ParcelSplitP[1];
                            string parcelsplit2P = ParcelSplitP[2];
                            driver.FindElement(By.XPath("/html/body/center/form[1]/table[1]/tbody/tr/td/table/tbody/tr[6]/td[3]/input")).SendKeys(parcelsplit1P);
                            driver.FindElement(By.XPath("/html/body/center/form[1]/table[1]/tbody/tr/td/table/tbody/tr[7]/td[3]/input")).SendKeys(parcelsplit2P);
                            driver.FindElement(By.XPath("/html/body/center/form/table[1]/tbody/tr/td/table/tbody/tr[4]/td[2]/input[4]")).Click();
                            gc.CreatePdf_WOP(orderNumber, "Parcel search", driver, "NY", "Queens");
                            driver.FindElement(By.XPath("/html/body/center/form/table[1]/tbody/tr/td/table/tbody/tr[9]/td[4]/img")).Click();
                            Thread.Sleep(2000);
                        }
                        if (parcelNumber == "")
                        {
                            HttpContext.Current.Session["Zero_QueensNY"] = "Zero";
                            gc.CreatePdf_WOP(orderNumber, "MultyAddressSearch", driver, "NY", "Queens");
                            driver.Quit();
                            return "No Records Found";
                        }
                    }

                    //property Detail
                    try
                    {
                        IAlert alert = driver.SwitchTo().Alert();
                        alert.Accept();
                        Thread.Sleep(1000);
                    }
                    catch { }
                    int intrestcount = 0;
                    IWebElement propertydetail = driver.FindElement(By.XPath("/html/body/a/center/table[2]/tbody"));
                    string Ownername1 = gc.Between(propertydetail.Text, "Name(s):", "Mailing Address:").Trim();
                    if (!Ownername1.Contains(" more..."))
                    {
                        Owner_Name = Ownername1;
                    }
                    string MailingAddress = gc.Between(propertydetail.Text, "Mailing Address:", "Planned Payment").Trim();
                    string parcel_number = gc.Between(propertydetail.Text, "BBL:", "Date:").Trim();
                    string PlannedPayment_Date = GlobalClass.After(propertydetail.Text, "Date:").Trim();
                    gc.CreatePdf(orderNumber, parcel_number.Replace("/", ""), "Peoperty Detail", driver, "NY", "Queens");
                    try
                    {///html/body/a/center/form[1]/table[3]/tbody/tr[2]/td/table/tbody
                        IWebElement Detailtable = driver.FindElement(By.XPath("/html/body/a/center/form[1]/table[2]/tbody/tr[2]/td/table"));
                        IList<IWebElement> Detailrow = Detailtable.FindElements(By.TagName("tr"));
                        // IList<IWebElement> detailid;
                        IWebElement Propertyswitchpagedetail;
                        string current = driver.CurrentWindowHandle;

                        for (int i = 2; i <= Detailrow.Count; i++)
                        {
                            try
                            {
                                string Description = "", Charges = "", Interest = "", Balance = "", PeroidBegin = "", Discount = "";
                                driver.FindElement(By.XPath("/html/body/a/center/form[1]/table[2]/tbody/tr[2]/td/table/tbody/tr[" + i + "]/td[5]/a/img")).Click();
                                driver.SwitchTo().Window(driver.WindowHandles.Last());
                                if (Ownername1.Contains(" more..."))
                                {
                                    IWebElement ownernamemore = driver.FindElement(By.XPath("/html/body/table[2]/tbody/tr[4]/td/table"));
                                    Owner_Name = gc.Between(driver.FindElement(By.XPath("/html/body/table[2]/tbody/tr[4]/td/table")).Text, "Name:", "Address:").Replace("\r\n", " ");
                                }
                                Propertyswitchpagedetail = driver.FindElement(By.XPath("/html/body/table[3]/tbody/tr[4]/td/table/tbody"));
                                if (Propertyswitchpagedetail.Text.Contains("Interest:"))
                                {

                                    Interest = gc.Between(Propertyswitchpagedetail.Text, "Interest:", "Balance:").Replace("+", "").Replace("$", "").Trim();
                                    if (Interest != "0.00" && intrestcount == 0)
                                    {
                                        driver.SwitchTo().Window(current);
                                        delequenttax(orderNumber, parcel_number);
                                        //driver.FindElement(By.XPath("/html/body/a/center/form[1]/table[3]/tbody/tr[2]/td/table/tbody/tr[" + i + "]/td[5]/a/img")).Click();

                                        //Propertyswitchpagedetail = driver.FindElement(By.XPath("/html/body/table[3]/tbody/tr[4]/td/table/tbody"));

                                        try
                                        {
                                            Propertyswitchpagedetail = driver.FindElement(By.XPath("/html/body/table[3]/tbody/tr[4]/td/table/tbody"));
                                            Thread.Sleep(2000);
                                        }
                                        catch { }
                                        try
                                        {
                                            Propertyswitchpagedetail = driver.FindElement(By.XPath("/html/body/a/center/form[1]/table[2]/tbody/tr[2]/td/table/tbody"));
                                            Thread.Sleep(1000);
                                            //IWebElement Propertyswitchpagedetail=
                                        }
                                        catch { }
                                        driver.SwitchTo().Window(driver.WindowHandles.Last());
                                        gc.CreatePdf(orderNumber, parcel_number.Replace("/", ""), "intrestedadd", driver, "NY", "Queens");
                                        intrestcount++;
                                    }
                                }
                                IList<IWebElement> Propertyrow = Propertyswitchpagedetail.FindElements(By.TagName("tr"));
                                IList<IWebElement> propertyswitchid;
                                foreach (IWebElement pro in Propertyrow)
                                {
                                    if (pro.Text.Count() != 0)
                                    {
                                        if (pro.Text.Contains("Description:") && pro.Text.Contains("Charges:"))
                                        {
                                            Description = gc.Between(pro.Text, "Description:", "Charges:");
                                            Charges = GlobalClass.After(pro.Text, "Charges:");
                                        }
                                        if (pro.Text.Contains("Balance:"))
                                        {
                                            Balance = GlobalClass.After(pro.Text, "Balance:");
                                        }
                                        if (pro.Text.Contains("Period Begin:"))
                                        {
                                            if (pro.Text.Contains("Interest:"))
                                            {
                                                PeroidBegin = gc.Between(pro.Text, "Period Begin:", "Interest:");
                                                Interest = GlobalClass.After(pro.Text, "Interest:");
                                            }
                                            if (pro.Text.Contains("Discount:"))
                                            {
                                                PeroidBegin = gc.Between(pro.Text, "Period Begin:", "Discount:");
                                                Discount = GlobalClass.After(pro.Text, "Discount:");
                                            }
                                            if (pro.Text.Contains("Period Begin:") && !pro.Text.Contains("Interest:") && !pro.Text.Contains("Discount:"))
                                            {
                                                PeroidBegin = GlobalClass.After(pro.Text, "Period Begin:");
                                            }
                                        }
                                    }
                                }
                                string TaxPropertyDetails = Description.Trim() + "~" + PeroidBegin.Trim() + "~" + Charges.Trim() + "~" + Interest.Trim() + "~" + Discount.Trim() + "~" + Balance.Trim();
                                gc.insert_date(orderNumber, parcel_number, 1472, TaxPropertyDetails, 1, DateTime.Now);
                                gc.CreatePdf(orderNumber, parcel_number.Replace("/", ""), "Account Type" + i, driver, "NY", "Queens");
                                driver.Close();
                                driver.SwitchTo().Window(current);
                            }
                            catch { }
                        }
                        DUE = driver.FindElement(By.XPath("/html/body/a/center/form[1]/table[2]/tbody/tr[4]/td/table/tbody/tr/td[2]")).Text;
                    }
                    catch { }
                    try
                    {

                        IWebElement Detailtable = driver.FindElement(By.XPath("/html/body/a/center/form[1]/table[3]/tbody/tr[2]/td/table/tbody"));
                        IList<IWebElement> Detailrow = Detailtable.FindElements(By.TagName("tr"));
                        // IList<IWebElement> detailid;
                        IWebElement Propertyswitchpagedetail;
                        string current = driver.CurrentWindowHandle;
                        for (int i = 2; i <= Detailrow.Count; i++)
                        {
                            try
                            {
                                string Description = "", Charges = "", Interest = "", Balance = "", PeroidBegin = "", Discount = "";

                                driver.FindElement(By.XPath("/html/body/a/center/form[1]/table[3]/tbody/tr[2]/td/table/tbody/tr[" + i + "]/td[5]/a/img")).Click();
                                driver.SwitchTo().Window(driver.WindowHandles.Last());

                                if (Ownername1.Contains(" more..."))
                                {
                                    IWebElement ownernamemore = driver.FindElement(By.XPath("/html/body/a/center/form[1]/table[3]/tbody/tr[2]/td/table/tbody"));
                                    Owner_Name = gc.Between(driver.FindElement(By.XPath("/html/body/table[2]/tbody/tr[4]/td/table")).Text, "Name:", "Address:").Replace("\r\n", " ");
                                }
                                Propertyswitchpagedetail = driver.FindElement(By.XPath("/html/body/table[3]/tbody/tr[4]/td/table/tbody"));
                                if (Propertyswitchpagedetail.Text.Contains("Interest:"))
                                {
                                    Interest = gc.Between(Propertyswitchpagedetail.Text, "Interest:", "Balance:").Replace("+", "").Replace("$", "").Trim();
                                    if (Interest != "0.00" && intrestcount == 0)
                                    {
                                        delequenttax(orderNumber, parcel_number);
                                        driver.FindElement(By.XPath("/html/body/a/center/form[1]/table[3]/tbody/tr[2]/td/table/tbody/tr[" + i + "]/td[5]/a/img")).Click();
                                        driver.SwitchTo().Window(driver.WindowHandles.Last());
                                        gc.CreatePdf(orderNumber, parcel_number.Replace("/", ""), "intrestedadd", driver, "NY", "Queens");
                                        Propertyswitchpagedetail = driver.FindElement(By.XPath("/html/body/table[3]/tbody/tr[4]/td/table/tbody"));
                                        intrestcount++;
                                    }
                                }
                                IList<IWebElement> Propertyrow = Propertyswitchpagedetail.FindElements(By.TagName("tr"));
                                IList<IWebElement> propertyswitchid;
                                foreach (IWebElement pro in Propertyrow)
                                {
                                    if (pro.Text.Count() != 0)
                                    {
                                        if (pro.Text.Contains("Description:") && pro.Text.Contains("Charges:"))
                                        {
                                            Description = gc.Between(pro.Text, "Description:", "Charges:");
                                            Charges = GlobalClass.After(pro.Text, "Charges:");
                                        }
                                        if (pro.Text.Contains("Balance:"))
                                        {
                                            Balance = GlobalClass.After(pro.Text, "Balance:");
                                        }
                                        if (pro.Text.Contains("Period Begin:"))
                                        {
                                            if (pro.Text.Contains("Interest:"))
                                            {
                                                PeroidBegin = gc.Between(pro.Text, "Period Begin:", "Interest:");
                                                Interest = GlobalClass.After(pro.Text, "Interest:");
                                            }
                                            if (pro.Text.Contains("Discount:"))
                                            {
                                                PeroidBegin = gc.Between(pro.Text, "Period Begin:", "Discount:");
                                                Discount = GlobalClass.After(pro.Text, "Discount:");
                                            }
                                            if (pro.Text.Contains("Period Begin:") && !pro.Text.Contains("Interest:") && !pro.Text.Contains("Discount:"))
                                            {
                                                PeroidBegin = GlobalClass.After(pro.Text, "Period Begin:");
                                            }
                                        }
                                    }
                                }
                                string TaxPropertyDetails = Description.Trim() + "~" + PeroidBegin.Trim() + "~" + Charges.Trim() + "~" + Interest.Trim() + "~" + Discount.Trim() + "~" + Balance.Trim();
                                gc.insert_date(orderNumber, parcel_number, 626, TaxPropertyDetails, 1, DateTime.Now);
                                gc.CreatePdf(orderNumber, parcel_number.Replace("/", ""), "Account Type" + i, driver, "NY", "Queens");
                                driver.Close();
                                driver.SwitchTo().Window(current);
                            }
                            catch { }
                        }
                        DUE = driver.FindElement(By.XPath("/html/body/a/center/form[1]/table[3]/tbody/tr[4]/td/table/tbody/tr/td[2]")).Text;
                    }
                    catch { }
                    IWebElement Paymentclick = driver.FindElement(By.LinkText("last payment received"));
                    Paymentclick.SendKeys(Keys.Enter);
                    Thread.Sleep(2000);

                    AssessmentTime = DateTime.Now.ToString("HH:mm:ss");
                    string AccountType = driver.FindElement(By.XPath("/html/body/center/form[1]/table/tbody/tr[2]/td/table/tbody/tr[2]/td[1]")).Text;
                    string AccountID = driver.FindElement(By.XPath("/html/body/center/form[1]/table/tbody/tr[2]/td/table/tbody/tr[2]/td[2]")).Text;
                    string TransactionType = driver.FindElement(By.XPath("/html/body/center/form[1]/table/tbody/tr[2]/td/table/tbody/tr[2]/td[3]")).Text;
                    string PeriodBegin = driver.FindElement(By.XPath("/html/body/center/form[1]/table/tbody/tr[2]/td/table/tbody/tr[2]/td[4]")).Text;
                    string PaidAmount = driver.FindElement(By.XPath("/html/body/center/form[1]/table/tbody/tr[2]/td/table/tbody/tr[2]/td[5]")).Text;
                    string TotalAmountPaid = driver.FindElement(By.XPath("/html/body/center/form[1]/table/tbody/tr[4]/td/table/tbody/tr[1]/td[5]")).Text;

                    string propertydetailresult = Owner_Name + "~" + MailingAddress + "~" + PlannedPayment_Date + "~" + DUE + "~" + AccountType + "~" + AccountID + "~" + TransactionType + "~" + PeriodBegin + "~" + PaidAmount + "~" + TotalAmountPaid;
                    gc.insert_date(orderNumber, parcel_number, 1468, propertydetailresult, 1, DateTime.Now);
                    gc.CreatePdf(orderNumber, parcel_number.Replace("/", ""), "Assessment", driver, "NY", "Queens");
                    //Switch windo detail

                    driver.Navigate().GoToUrl("http://nycserv.nyc.gov/NYCServWeb/NYCSERVMain");
                    IWebElement PropertyHistry = driver.FindElement(By.XPath("/html/body/form[1]/center/table[2]/tbody/tr[1]/td/table/tbody/tr/td[1]/center/table/tbody/tr[3]/td/table/tbody/tr[3]"));
                    SelectElement PropertySelect = new SelectElement(driver.FindElement(By.Name("propertydropdownmenu")));
                    PropertySelect.SelectByIndex(2);
                    driver.FindElement(By.XPath("/html/body/form[1]/center/table[2]/tbody/tr[1]/td/table/tbody/tr/td[1]/center/table/tbody/tr[3]/td/table/tbody/tr[3]/td[2]/input")).SendKeys(Keys.Enter);
                    Thread.Sleep(2000);
                    ParcelSplit = parcel_number.Split('-', '/');
                    string parcelsplit1 = ParcelSplit[1];
                    string parcelsplit2 = ParcelSplit[2];
                    driver.FindElement(By.XPath("/html/body/center/form[1]/table[1]/tbody/tr/td/table/tbody/tr[6]/td[3]/input")).SendKeys(parcelsplit1);
                    driver.FindElement(By.XPath("/html/body/center/form[1]/table[1]/tbody/tr/td/table/tbody/tr[7]/td[3]/input")).SendKeys(parcelsplit2);

                    IWebElement ITaxcity = driver.FindElement(By.XPath("/html/body/center/form/table[1]/tbody/tr/td/table/tbody/tr[4]/td[2]/input[4]"));
                    ITaxcity.Click();
                    Thread.Sleep(2000);

                    driver.FindElement(By.XPath("/html/body/center/form[1]/table[1]/tbody/tr/td/table/tbody/tr[9]/td[4]/a/img")).Click();
                    gc.CreatePdf(orderNumber, parcel_number.Replace("/", ""), "Tax Payment History", driver, "NY", "Queens");
                    IWebElement PaymentHistory = driver.FindElement(By.XPath("/html/body/center/form[1]/table/tbody/tr[2]/td/table/tbody"));
                    IList<IWebElement> PaymentHistoryrow = PaymentHistory.FindElements(By.TagName("tr"));
                    IList<IWebElement> PaymentHistoryid;
                    foreach (IWebElement Payment in PaymentHistoryrow)
                    {
                        PaymentHistoryid = Payment.FindElements(By.TagName("td"));
                        if (PaymentHistoryid.Count != 0 && !Payment.Text.Contains("Account Type"))
                        {
                            string PaymentHistorydate = PaymentHistoryid[0].Text + "~" + PaymentHistoryid[1].Text + "~" + PaymentHistoryid[2].Text + "~" + PaymentHistoryid[3].Text + "~" + PaymentHistoryid[4].Text + "~" + PaymentHistoryid[5].Text;
                            gc.insert_date(orderNumber, parcel_number, 1469, PaymentHistorydate, 1, DateTime.Now);
                        }

                    }

                    driver.Navigate().GoToUrl("http://nycprop.nyc.gov/nycproperty/nynav/jsp/selectbbl.jsp");
                    IWebElement PropertyInformation = driver.FindElement(By.XPath("/html/body/div/center/table[2]/tbody/tr/td[1]/table/tbody/tr[2]/td[2]/div/table[3]/tbody/tr/td/div/p/table/tbody/tr/td/table/tbody/tr[2]/td/form/table/tbody/tr[2]/td[2]"));
                    SelectElement PropertyInformationSelect = new SelectElement(driver.FindElement(By.Name("FBORO")));
                    PropertyInformationSelect.SelectByIndex(3);
                    ParcelSplit = parcel_number.Split('-', '/');
                    parcelsplit1 = ParcelSplit[1];
                    parcelsplit2 = ParcelSplit[2];
                    driver.FindElement(By.XPath("/html/body/div/center/table[2]/tbody/tr/td[1]/table/tbody/tr[2]/td[2]/div/table[3]/tbody/tr/td/div/p/table/tbody/tr/td/table/tbody/tr[2]/td/form/table/tbody/tr[3]/td[2]/input")).SendKeys(parcelsplit1);
                    driver.FindElement(By.XPath("/html/body/div/center/table[2]/tbody/tr/td[1]/table/tbody/tr[2]/td[2]/div/table[3]/tbody/tr/td/div/p/table/tbody/tr/td/table/tbody/tr[2]/td/form/table/tbody/tr[4]/td[2]/input")).SendKeys(parcelsplit2);
                    driver.FindElement(By.XPath("/html/body/div/center/table[2]/tbody/tr/td[1]/table/tbody/tr[2]/td[2]/div/table[3]/tbody/tr/td/div/p/table/tbody/tr/td/table/tbody/tr[2]/td/form/table/tbody/tr[7]/td/input[1]")).Click();
                    driver.FindElement(By.XPath("/html/body/div/center/div/center/table[1]/tbody/tr[1]/td[1]/table/tbody/tr[2]/td[2]/div/table[3]/tbody/tr[2]/td/table/tbody/tr/td[2]/font/b/a")).Click();
                    Thread.Sleep(2000);
                    //Propertydetail
                    IWebElement Propertydetail2 = driver.FindElement(By.XPath("/html/body/div/center/table/tbody/tr[2]/td/table[5]/tbody"));
                    string Borough = gc.Between(Propertydetail2.Text, "Borough:", "Building Class:");
                    string Block = gc.Between(Propertydetail2.Text, "Block:", "Tax Class:");
                    string Lot = gc.Between(Propertydetail2.Text, "Lot:", "In Rem").Trim();
                    string PropertyAddress = gc.Between(Propertydetail2.Text, "Property Address:", " Exemption:");
                    string LastPaymentDate = gc.Between(Propertydetail2.Text, "Payments Thru:", "Borough:").Trim();
                    string BuildingClass = gc.Between(Propertydetail2.Text, "Building Class:", "Codes");
                    string TaxClass = gc.Between(Propertydetail2.Text, "Tax Class:", "Lot:");
                    string Rem = gc.Between(Propertydetail2.Text, "In Rem:", "Property Address:");
                    string Exemption = gc.Between(Propertydetail2.Text, "Exemption:", "Unused SCRIE credit:");
                    string UnusedSCRIEcredit = GlobalClass.After(Propertydetail2.Text, "Unused SCRIE credit:").Trim();
                    string TaxPropertyresult = Borough + "~" + Block + "~" + Lot + "~" + PropertyAddress + "~" + LastPaymentDate + "~" + BuildingClass + "~" + TaxClass + "~" + Rem + "~" + Exemption + "~" + UnusedSCRIEcredit;
                    gc.insert_date(orderNumber, parcel_number, 1470, TaxPropertyresult, 1, DateTime.Now);
                    gc.CreatePdf(orderNumber, parcel_number.Replace("/", ""), "Propertydetail", driver, "NY", "Queens");
                    //tax Information
                    int currentyear = DateTime.Now.Year;
                    driver.FindElement(By.LinkText("Click here for a more detailed explanation of the items below.")).Click();
                    Thread.Sleep(2000);
                    gc.CreatePdf(orderNumber, parcel_number.Replace("/", ""), "TAX DETAIL", driver, "NY", "Queens");
                    IWebElement Taxtable = driver.FindElement(By.XPath("/html/body/div/center/table/tbody/tr[2]/td/table[8]/tbody"));
                    IList<IWebElement> TaxtableRow = Taxtable.FindElements(By.TagName("tr"));
                    IList<IWebElement> taxtableid;
                    foreach (IWebElement tax in TaxtableRow)
                    {
                        try
                        {
                            taxtableid = tax.FindElements(By.TagName("td"));
                            if (taxtableid.Count != 0 && !tax.Text.Contains("Account"))
                            {
                                string Taxresult = taxtableid[1].Text + "~" + taxtableid[2].Text + "~" + taxtableid[3].Text + "~" + taxtableid[4].Text + "~" + taxtableid[5].Text + "~" + taxtableid[6].Text + "~" + taxtableid[7].Text + "~" + taxtableid[8].Text + "~" + taxtableid[9].Text + "~" + taxtableid[10].Text;
                                gc.insert_date(orderNumber, parcel_number, 1471, Taxresult, 1, DateTime.Now);
                            }
                        }
                        catch { }
                    }
                    // currentyear++;
                    TaxTime = DateTime.Now.ToString("HH:mm:ss");

                    LastEndTime = DateTime.Now.ToString("HH:mm:ss");
                    gc.insert_TakenTime(orderNumber, "NY", "Queens", StartTime, AssessmentTime, TaxTime, CitytaxTime, LastEndTime);

                    driver.Quit();

                    gc.mergpdf(orderNumber, "NY", "Queens");
                    return "Data Inserted Successfully";
                }

                catch (Exception ex)
                {
                    driver.Quit();
                    GlobalClass.LogError(ex, orderNumber);
                    throw ex;
                }
            }
        }
        public void delequenttax(string orderNumber, string parcel_number)
        {
            string strEffectiveDate = "";
            string currDate = DateTime.Now.ToString("MM/dd/yyyy");
            string dateChecking = DateTime.Now.ToString("MM") + "/15/" + DateTime.Now.ToString("yyyy");
            driver.Navigate().GoToUrl("http://nycserv.nyc.gov/NYCServWeb/NYCSERVMain");
            driver.FindElement(By.XPath("/html/body/form[1]/center/table[2]/tbody/tr[1]/td/table/tbody/tr/td[1]/center/table/tbody/tr[3]/td/table/tbody/tr[3]/td[2]/input")).SendKeys(Keys.Enter);
            Thread.Sleep(2000);

            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                alert.Accept();
                Thread.Sleep(1000);
            }
            catch { }
            if (Convert.ToDateTime(currDate) > Convert.ToDateTime(dateChecking))
            {
                string nextEndOfMonth = "";
                if ((Convert.ToInt16(Convert.ToInt16(DateTime.Now.ToString("MM"))) < 12))
                {
                    nextEndOfMonth = new DateTime(Convert.ToInt16(DateTime.Now.ToString("yyyy")), Convert.ToInt16(Convert.ToInt16(DateTime.Now.ToString("MM")) + 1), DateTime.DaysInMonth(Convert.ToInt16(DateTime.Now.ToString("yyyy")), Convert.ToInt16(DateTime.Now.ToString("MM")) + 1)).ToString("MM/dd/yyyy");

                }
                else
                {
                    int nxtYr = Convert.ToInt16(DateTime.Now.ToString("yyyy")) + 1;
                    nextEndOfMonth = nextEndOfMonth = new DateTime(nxtYr, 1, DateTime.DaysInMonth(Convert.ToInt16(DateTime.Now.ToString("yyyy")), 1)).ToString("MM/dd/yyyy");

                }
                strEffectiveDate = nextEndOfMonth;
            }
            else
            {
                string EndOfMonth = new DateTime(Convert.ToInt16(DateTime.Now.ToString("yyyy")), Convert.ToInt16(Convert.ToInt16(DateTime.Now.ToString("MM"))), DateTime.DaysInMonth(Convert.ToInt16(DateTime.Now.ToString("yyyy")), Convert.ToInt16(DateTime.Now.ToString("MM")))).ToString("MM/dd/yyyy");
                strEffectiveDate = EndOfMonth;
            }
            string[] deluquinttacxdate = strEffectiveDate.Split('/');
            string month = deluquinttacxdate[0];
            string date = deluquinttacxdate[1];
            string year = deluquinttacxdate[2];
            IWebElement monthclick = driver.FindElement(By.XPath("/html/body/center/form/table[3]/tbody/tr/td/table/tbody/tr[4]/td[3]/select[1]"));
            SelectElement monthclick1 = new SelectElement(driver.FindElement(By.Name("INTEREST_THROUGH_MONTH")));
            monthclick1.SelectByValue(month);
            driver.FindElement(By.XPath("/html/body/center/form/table[3]/tbody/tr/td/table/tbody/tr[4]/td[3]/select[2]")).SendKeys(date);
            driver.FindElement(By.XPath("/html/body/center/form/table[3]/tbody/tr/td/table/tbody/tr[4]/td[3]/select[3]")).SendKeys(year);
            ParcelSplit = parcel_number.Split('-', '/');
            parcelsplit1 = ParcelSplit[1];
            parcelsplit2 = ParcelSplit[2];
            driver.FindElement(By.XPath("/html/body/center/form/table[1]/tbody/tr/td/table/tbody/tr[6]/td[3]/input")).SendKeys(parcelsplit1);
            driver.FindElement(By.XPath("/html/body/center/form/table[1]/tbody/tr/td/table/tbody/tr[7]/td[3]/input")).SendKeys(parcelsplit2);
            driver.FindElement(By.XPath("/html/body/center/form/table[1]/tbody/tr/td/table/tbody/tr[4]/td[2]/input[4]")).Click();
            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                alert.Accept();
                Thread.Sleep(1000);
            }
            catch { }
            driver.FindElement(By.XPath("/html/body/center/form/table[1]/tbody/tr/td/table/tbody/tr[9]/td[4]/img")).Click();
            Thread.Sleep(2000);
            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                alert.Accept();
                Thread.Sleep(1000);
            }
            catch { }
            gc.CreatePdf(orderNumber, parcel_number.Replace("/", ""), "Deliquent", driver, "NY", "Queens");
        }
    }
}