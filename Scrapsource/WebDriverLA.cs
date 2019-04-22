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
//using iTextSharp.text;
using System.Text.RegularExpressions;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Support.Extensions;
using System.Net;

namespace ScrapMaricopa
{
    public class WebDriverLA
    {
        IWebDriver driver;
        DBconnection db = new DBconnection();
        GlobalClass gc = new GlobalClass();
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ToString());
        string outputPath = "";
        public void FTP_LA(string address, string parcelNumber, string searchType, string orderNumber, string directParcel, string treasurerCaptcha)
        {
            GlobalClass.global_orderNo = orderNumber;
            HttpContext.Current.Session["orderNo"] = orderNumber;
            GlobalClass.global_parcelNo = parcelNumber;

            if (treasurerCaptcha == "")
            {
                var driverService = PhantomJSDriverService.CreateDefaultService();
                driverService.HideCommandPromptWindow = true;
                driver = new PhantomJSDriver();
                try
                {
                    if (searchType == "address")
                    {
                        if (Assessor_pageLoad())
                        {
                            GlobalClass.multiParcel_la = "Yes"; ;
                            driver.FindElement(By.Name("basicsearchterm")).SendKeys(address);
                            driver.FindElement(By.XPath("/html/body/div/div[2]/div/form[1]/div/span/button")).Click();
                            Thread.Sleep(3000);

                            //Select address from list....
                            IWebElement tableElement = driver.FindElement(By.XPath("/html/body/div/div[3]/div/table"));
                            IList<IWebElement> tableRow = tableElement.FindElements(By.TagName("tr"));
                            IList<IWebElement> rowTD;

                            foreach (IWebElement row in tableRow)
                            {
                                rowTD = row.FindElements(By.TagName("td"));
                                if (rowTD.Count != 0)
                                {
                                    string multi = rowTD[1].Text + "~" + rowTD[2].Text.Replace("'", "");
                                    gc.insert_data(orderNumber,DateTime.Now , rowTD[0].Text.Trim(), 51, multi, 1);
                                    //db.ExecuteQuery("insert into la_multiowner(parcel_no,situs_address,legal_description,order_no) values('" + rowTD[0].Text + "','" + rowTD[1].Text + "','" + rowTD[2].Text.Replace("'", "") + "','" + orderNumber + "') ");
                                }
                            }
                            driver.Quit();
                            return;
                        }
                    }
                    else if (searchType == "parcel")
                    {
                        string parcelUrl = "https://portal.assessor.lacounty.gov/parceldetail/" + parcelNumber;
                        driver.Navigate().GoToUrl(parcelUrl);

                        GlobalClass.parcel_status = driver.FindElement(By.XPath("/html/body/div/div/section[1]/div[2]/div/div/div[1]/div[2]/div[3]/dl/dd[1]")).Text.TrimStart().TrimEnd();

                        if (GlobalClass.parcel_status == "DELETED" || GlobalClass.parcel_status == "INACTIVE" || GlobalClass.parcel_status == "IN ACTIVE")
                        {
                            driver.Quit();
                            return;
                        }

                        GlobalClass.parcelNumber_la = driver.FindElement(By.XPath("/html/body/div/div/section[1]/div[2]/div/div/div[1]/div[1]/h2/div")).Text;
                        GlobalClass.parcelNumber_la = WebDriverTest.Between(GlobalClass.parcelNumber_la, " ", " ");
                        gc.CreatePdf(orderNumber, parcelNumber, "Assessor",driver, "CA", "Los Angeles");

                        //scrap exemption...
                        IWebElement exemption = driver.FindElement(By.XPath("/html/body/div/div/section[1]/div[2]/div/div/div[1]/div[2]/div[3]/dl/dd[6]"));
                        string exemp_Value = "-",usetype="-",taxratearea="-",yearbuilt="-";
                        exemp_Value = exemption.Text;

                        //if (directParcel == "Yes")
                        //{
                            IWebElement sit_addr = driver.FindElement(By.XPath("/html/body/div/div/section[2]/div[2]/div/div[2]/div[1]/div[3]/dl/dd"));
                            string situs_address = sit_addr.Text;
                            IWebElement legal_desc = driver.FindElement(By.XPath("/html/body/div/div/section[2]/div[2]/div/div[2]/div[1]/div[4]/dl/dd"));
                            string legal_description = legal_desc.Text;
                            usetype = driver.FindElement(By.XPath("/html/body/div/div/section[1]/div[2]/div/div/div[1]/div[2]/div[2]/dl/dd[1]")).Text.Trim();
                            if (usetype == "")
                            {
                                usetype = "-";
                            }
                            taxratearea = driver.FindElement(By.XPath("/html/body/div/div/section[1]/div[2]/div/div/div[1]/div[2]/div[2]/dl/dd[3]")).Text.Trim();
                            if (taxratearea == "")
                            {
                                taxratearea = "-";
                            }
                            yearbuilt = driver.FindElement(By.XPath("/html/body/div/div/section[1]/div[2]/div/div/div[1]/div[4]/div[3]/dl/dd[1]")).Text.Trim();
                            if (yearbuilt == "")
                            {
                                yearbuilt = "-";
                            }
                            string property = situs_address + "~" + legal_description + "~" + usetype + "~" + taxratearea + "~" + yearbuilt;
                            gc.insert_data(orderNumber,DateTime.Now , parcelNumber, 50, property, 1);
                            //  db.ExecuteQuery("insert into la_multiowner(parcel_no,situs_address,legal_description,order_no) values('" + parcelNumber + "','" + situs_address + "','" + legal_description + "','" + orderNumber + "') ");

                      //  }
                        //scrap table...
                        IWebElement tblAssess = driver.FindElement(By.XPath("/html/body/div/div/section[1]/div[2]/div/div/div[1]/div[5]/table"));

                        string yr1 = driver.FindElement(By.XPath("/html/body/div/div/section[1]/div[2]/div/div/div[1]/div[5]/table/thead/tr/th[2]")).Text;
                        yr1 = WebDriverTest.Before(yr1, " Roll");
                        string yr2 = driver.FindElement(By.XPath("/html/body/div/div/section[1]/div[2]/div/div/div[1]/div[5]/table/thead/tr/th[3]")).Text;
                        yr2 = WebDriverTest.Before(yr2, " Current");
                        List<string> data1 = getTableData1(tblAssess);
                        string land = "-", improvement = "-", total = "-";

                        land = "$" + WebDriverTest.After(data1[1], "$").TrimStart();
                        improvement = "$" + WebDriverTest.After(data1[2], "$").TrimStart();
                        total = "$" + WebDriverTest.After(data1[3], "$").TrimStart();
                        string ass = yr1 + "~" + land + "~" + improvement + "~" + total + "~" + exemp_Value;
                        gc.insert_data(orderNumber,DateTime.Now , parcelNumber, 52, ass, 1);
                        //db.ExecuteQuery("insert into la_assessor (order_no,parcel_no,year,land,improvements,total,exemption_type) values ('" + orderNumber + "','" + parcelNumber + "','" + yr1 + "','" + land + "','" + improvement + "','" + total + "','" + exemp_Value + "')");

                        List<string> data2 = getTableData(tblAssess);
                        land = "$" + WebDriverTest.After(data2[1], "$").TrimStart();
                        improvement = "$" + WebDriverTest.After(data2[2], "$").TrimStart();
                        total = "$" + WebDriverTest.After(data2[3], "$").TrimStart();
                        string ass1 = yr2 + "~" + land + "~" + improvement + "~" + total + "~" + exemp_Value;
                        gc.insert_data(orderNumber,DateTime.Now, parcelNumber, 52, ass1, 1);
                        //db.ExecuteQuery("insert into la_assessor (order_no,parcel_no,year,land,improvements,total,exemption_type) values ('" + orderNumber + "','" + parcelNumber + "','" + yr2 + "','" + land + "','" + improvement + "','" + total + "','" + exemp_Value + "')");



                        //treasurer details.....
                        driver.Navigate().GoToUrl("https://vcheck.ttc.lacounty.gov/");
                        Thread.Sleep(5000);
                        var imgId = driver.FindElement(By.Id("recaptcha_challenge_image"));

                        GlobalClass.imgURL = imgId.GetAttribute("src");

                        string outPath = System.Web.HttpContext.Current.Server.MapPath("~/captcha\\") + parcelNumber + ".png";
                        WebClient captcha = new WebClient();
                        captcha.DownloadFile(GlobalClass.imgURL, outPath);

                        if (GlobalClass.imgURL != "")
                        {
                            //db.ExecuteQuery("insert into la_url (parcel_no,order_no,src) values ('" + parcelNumber + "','" + orderNumber + "','" + GlobalClass.imgURL + "')");
                            GlobalClass.sDriver = driver;
                            return;
                        }

                    }
                }

                catch (Exception ex)
                {
                    if (driver != null)
                    {
                        driver.Quit();
                    }
                    throw ex;
                }
            }
            else
            {
                //try
                //{
                GlobalClass.sDriver.FindElement(By.Id("recaptcha_response_field")).SendKeys(treasurerCaptcha);
                GlobalClass.sDriver.FindElement(By.Id("next")).Click();

                IList<IWebElement> optionsClass = GlobalClass.sDriver.FindElements(By.TagName("span"));
                foreach (IWebElement strOption in optionsClass)
                {
                    if (strOption.Text.Contains("Property Tax Inquiry/One-"))
                    {
                        strOption.Click();
                        break;
                    }
                }

                string[] treasParclNo = GlobalClass.parcelNumber_la.Split('-');
                GlobalClass.sDriver.FindElement(By.Name("mapbook")).SendKeys(treasParclNo[0]);
                GlobalClass.sDriver.FindElement(By.Name("page")).SendKeys(treasParclNo[1]);
                GlobalClass.sDriver.FindElement(By.Name("parcel")).SendKeys(treasParclNo[2]);

                Thread.Sleep(3000);
                if (GlobalClass.sDriver.FindElement(By.Id("inquirebutton")).Enabled == true)
                {
                    GlobalClass.sDriver.FindElement(By.Id("inquirebutton")).Click();
                }


                outputPath = "";
                outputPath = ConfigurationManager.AppSettings["screenShotPath-la"];
                outputPath = outputPath + orderNumber + "\\" + parcelNumber + "\\";
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }
                string img = outputPath + "taxSummary" + ".png";
                string pdf = outputPath + "taxSummary" + ".pdf";

                //driver.Manage().Window.Maximize();
                GlobalClass.sDriver.TakeScreenshot().SaveAsFile(img, ScreenshotImageFormat.Png);

                WebDriverTest.ConvertImageToPdf(img, pdf);
                if (File.Exists(img))
                {
                    File.Delete(img);
                }

                // CreatePdf(orderNumber, parcelNumber, "taxSummary");

                IList<IWebElement> installInfos = GlobalClass.sDriver.FindElements(By.ClassName("installmentinfo"));
                int i = 0;
                string insInfo = "-", tax_year = "-", seq_no = "-", insInfo1 = "-", tax_year1 = "-", seq_no1 = "-";
                foreach (IWebElement installInfo in installInfos)
                {
                    if (i == 0)
                    {
                        insInfo = WebDriverTest.After(installInfo.Text, "YEAR: ");
                        tax_year = WebDriverTest.Before(insInfo, "SEQUENCE:").TrimStart().TrimEnd();
                        seq_no = WebDriverTest.After(insInfo, "SEQUENCE:").TrimStart().TrimEnd();
                    }
                    else
                    {
                        insInfo1 = WebDriverTest.After(installInfo.Text, "YEAR: ");
                        tax_year1 = WebDriverTest.Before(insInfo1, "SEQUENCE:").TrimStart().TrimEnd();
                        seq_no1 = WebDriverTest.After(insInfo1, "SEQUENCE:").TrimStart().TrimEnd();
                    }
                    i++;

                }


                IList<IWebElement> tableElements = GlobalClass.sDriver.FindElements(By.ClassName("installmenttable"));
                int j = 0;
                foreach (IWebElement tableElement in tableElements)
                {
                    IList<IWebElement> tableRow = tableElement.FindElements(By.TagName("tr"));
                    IList<IWebElement> rowTD;
                    List<string> installment1 = new List<string>();
                    List<string> installment2 = new List<string>();
                    foreach (IWebElement row in tableRow)
                    {
                        rowTD = row.FindElements(By.TagName("td"));
                        if (rowTD.Count == 5)
                        {
                            installment1.Add(rowTD[1].Text);
                            installment2.Add(rowTD[4].Text);
                        }
                    }

                    string tax_amt = "-", penalty_amt = "-", total_due = "-", paid_amt = "-", balance_due = "-", delinquent = "-", tax_status = "-";

                    tax_status = WebDriverTest.After(GlobalClass.sDriver.FindElement(By.ClassName("installmentstatus")).Text, "Tax Status:").TrimStart();

                    //installment1....
                    //Installment~Tax_amount~Penalty_amount~Total_due~Paid_amount~Balance_due~Delinquent~Tax_status~Tax_year~Seq_no

                    tax_amt = installment1[0]; penalty_amt = installment1[1]; total_due = installment1[2]; paid_amt = installment1[3]; balance_due = installment1[4];
                    delinquent = installment1[5];
                    if (delinquent == " ")
                    {
                        delinquent = "-";
                    }
                    if (j == 0)
                    {
                        string inst = "1" + "~" + tax_amt + "~" + penalty_amt + "~" + total_due + "~" + paid_amt + "~" + balance_due + "~" + delinquent + "~" + tax_status + "~" + tax_year + "~" + seq_no;
                        gc.insert_data(orderNumber,DateTime.Now , parcelNumber, 53, inst, 1);
                        

                        //installment1....

                        tax_amt = installment2[0]; penalty_amt = installment2[1]; total_due = installment2[2]; paid_amt = installment2[3]; balance_due = installment2[4]; delinquent = installment2[5];
                        if (delinquent == " ")
                        {
                            delinquent = "-";
                        }
                        string inst1 = "2" + "~" + tax_amt + "~" + penalty_amt + "~" + total_due + "~" + paid_amt + "~" + balance_due + "~" + delinquent + "~" + tax_status + "~" + tax_year + "~" + seq_no;
                        gc.insert_data(orderNumber,DateTime.Now,parcelNumber, 53, inst1, 1);
                        
                    }
                    else
                    {
                        string inst = "1" + "~" + tax_amt + "~" + penalty_amt + "~" + total_due + "~" + paid_amt + "~" + balance_due + "~" + delinquent + "~" + "SUPPLEMENTAL" + "~" + tax_year1 + "~" + seq_no1;
                        gc.insert_data(orderNumber,DateTime.Now,parcelNumber, 53, inst, 1);
                        //installment1....

                        tax_amt = installment2[0]; penalty_amt = installment2[1]; total_due = installment2[2]; paid_amt = installment2[3]; balance_due = installment2[4]; delinquent = installment2[5];
                        if (delinquent == " ")
                        {
                            delinquent = "-";
                        }
                        string inst1 = "2" + "~" + tax_amt + "~" + penalty_amt + "~" + total_due + "~" + paid_amt + "~" + balance_due + "~" + delinquent + "~" + "SUPPLEMENTAL" + "~" + tax_year1 + "~" + seq_no1;
                        gc.insert_data(orderNumber,DateTime.Now, parcelNumber, 53, inst1, 1);
                        
                    }
                    j++;

                }

            

            //delinquent data....
            try
            {
                IWebElement delinqTable = GlobalClass.sDriver.FindElement(By.ClassName("installmentdelinq"));
                IList<IWebElement> tblRow = delinqTable.FindElements(By.TagName("tr"));
                IList<IWebElement> rowtD;
                List<string> tr1 = new List<string>();
                List<string> tr3 = new List<string>();

                foreach (IWebElement row in tblRow)
                {
                    rowtD = row.FindElements(By.TagName("td"));
                    tr1.Add(rowtD[1].Text);
                    if (rowtD.Count >= 4)
                    {
                        tr3.Add(rowtD[3].Text);
                    }
                }
                //insert data
            }
            catch
            {

            }

            GlobalClass.sDriver.Quit();

            //}
            ////catch (Exception ex)
            ////{
            //    GlobalClass.sDriver.Quit();
            //    throw ex;
            //}

        }

    }


    private bool Assessor_pageLoad()
    {
        try
        {
            driver.Navigate().GoToUrl("https://portal.assessor.lacounty.gov/");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            var myElement = wait.Until(x => x.FindElement(By.Name("basicsearchterm")));
            return myElement.Displayed;
        }
        catch
        {
            return false;
        }
    }


    private bool Treasurer_pageLoad()
    {
        try
        {
            driver.Navigate().GoToUrl("https://vcheck.ttc.lacounty.gov/");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            var myElement = wait.Until(x => x.FindElement(By.Name("basicsearchterm")));
            return myElement.Displayed;
        }
        catch
        {
            return false;
        }
    }



    public List<string> getTableData(IWebElement tbl)
    {
        int nrec = 0;
        List<string> data = new List<string>();
        string rowBuff = "";

        IList<IWebElement> rows = tbl.FindElements(By.TagName("tr"));
        IList<IWebElement> cols;

        foreach (IWebElement tr in rows)
        {
            int tdx = 0;
            cols = tr.FindElements(By.TagName("td"));
            nrec++;
            rowBuff = nrec.ToString();
            foreach (IWebElement td in cols)
            {
                if (tdx++ == 2)
                {
                    rowBuff += "\t" + System.Net.WebUtility.HtmlDecode(td.Text);
                    break;
                }
            }
            data.Add(rowBuff);
        }

        return data;
    }


    public List<string> getTableData1(IWebElement tbl)
    {
        int nrec = 0;
        List<string> data = new List<string>();
        string rowBuff = "";

        IList<IWebElement> rows = tbl.FindElements(By.TagName("tr"));
        IList<IWebElement> cols;

        foreach (IWebElement tr in rows)
        {
            int tdx = 0;
            cols = tr.FindElements(By.TagName("td"));
            nrec++;
            rowBuff = nrec.ToString();
            foreach (IWebElement td in cols)
            {
                if (tdx++ == 1)
                {
                    rowBuff += "\t" + System.Net.WebUtility.HtmlDecode(td.Text);
                    break;
                }
            }
            data.Add(rowBuff);
        }

        return data;
    }

    public void CreatePdf(string orderno, string parcelno, string pdfName)
    {
        outputPath = "";
        outputPath = ConfigurationManager.AppSettings["screenShotPath-la"];
        outputPath = outputPath + orderno + "\\" + parcelno + "\\";
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        string img = outputPath + pdfName + ".png";
        string pdf = outputPath + pdfName + ".pdf";

        driver.Manage().Window.Maximize();
        driver.TakeScreenshot().SaveAsFile(img, ScreenshotImageFormat.Png);

        WebDriverTest.ConvertImageToPdf(img, pdf);
        if (File.Exists(img))
        {
            File.Delete(img);
        }

    }

    public void CreatePdf_WOP(string orderno, string pdfName)
    {
        outputPath = "";
        outputPath = ConfigurationManager.AppSettings["screenShotPath-la"];
        outputPath = outputPath + orderno + "\\";
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        string img = outputPath + pdfName + ".png";
        string pdf = outputPath + pdfName + ".pdf";

        driver.Manage().Window.Maximize();
        driver.TakeScreenshot().SaveAsFile(img, ScreenshotImageFormat.Png);

        WebDriverTest.ConvertImageToPdf(img, pdf);
        if (File.Exists(img))
        {
            File.Delete(img);
        }

    }


}


}