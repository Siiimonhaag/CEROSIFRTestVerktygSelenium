using System;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using WebDriverManager;
using OpenQA.Selenium.Support;
using Xunit;
using System.Collections.Generic;

namespace CEROSIFRTestVerktygSelenium
{

    public class SeleniumTests
    {

        ChromeDriver driver = new ChromeDriver();
        //ChromeDriver driver = new ChromeDriver(@"C:\Users\simon\Downloads");
        DriverManager driverManager = new DriverManager();


        public SeleniumTests()
        {
            // Konstruktor för att
            // Klicka på acceptera kakor varje gång vi kör ett test

            driver.Navigate().GoToUrl("www.google.com");

        }

        [Fact]
        public void Test1()
        {

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
