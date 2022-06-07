using System;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using WebDriverManager;
using OpenQA.Selenium.Support;
using Xunit;
using System.Collections.Generic;
using System.Threading;

namespace CEROSIFRTestVerktygSelenium
{

    public class SeleniumTests
    {

        //ChromeDriver driver = new ChromeDriver();
        ChromeDriver driver = new ChromeDriver(@"C:\Users\simon\Downloads");
        DriverManager driverManager = new DriverManager();


        public SeleniumTests()
        {
            // Konstruktor för att
            // Klicka på acceptera kakor varje gång vi kör ett test
            // Båda click nedan funkar!

            driver.Navigate().GoToUrl("https://www.komplett.se/");
            Thread.Sleep(1500);
            
            driver.FindElement(By.CssSelector(".btn-large")).Click();
            driver.FindElement(By.XPath("//button[@class='btn-large primary']")).Click();

        }

        [Fact]
        public void Test1()
        {
            //driver.Navigate().GoToUrl("https://www.komplett.se/");
        }

       /* [Fact]
        public void Test2()
        {

        }

        [Fact]
        public void Test3()
        {

        }

        [Fact]
        public void Test4()
        {

        }*/
    }
}
