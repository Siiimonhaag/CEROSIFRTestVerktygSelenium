using System;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using WebDriverManager;
using OpenQA.Selenium.Support;
using Xunit;
using Xunit.Abstractions;
using System.Threading;
using System.Collections.Generic;

namespace CEROSIFRTestVerktygSelenium
{

    public class SeleniumTests
    {
        ITestOutputHelper testOutput;
        ChromeDriver driver = new ChromeDriver();
        DriverManager driverManager = new DriverManager();
        string url = "https://www.coop.se/";


        public SeleniumTests(ITestOutputHelper _testOutput)
        {

            // Konstruktor för att
            // Klicka på acceptera kakor varje gång vi kör ett test
            // Båda click nedan funkar!
            testOutput = _testOutput;
            driver.Navigate().GoToUrl(url);
            driver.Manage().Window.FullScreen();
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//*[@id='cmpbntyestxt']")).Click();

        }

        [Fact]
        public void FullscreenAndSearchForKetchup()
        {
            
            IWebElement handlaOnline = driver.FindElement(By.LinkText("Handla online"));
            handlaOnline.Click();
            driver.Manage().Window.FullScreen();
            Thread.Sleep(1000);
            IWebElement searchBar = driver.FindElement(By.ClassName("Search-input"));
            searchBar.SendKeys("Ketchup");
            Thread.Sleep(2000);
            searchBar.SendKeys(Keys.Enter);
            Thread.Sleep(1500);
            driver.Quit();
            driver.Dispose();

        }

        [Fact]
        public void ThisIsTheFirstTestAgainJustForShowcase()
        {
            IWebElement handlaOnline = driver.FindElement(By.LinkText("Handla online"));
            handlaOnline.Click();
            driver.Manage().Window.FullScreen();
            Thread.Sleep(1000);
            IWebElement searchBar = driver.FindElement(By.ClassName("Search-input"));
            searchBar.SendKeys("Ketchup");
            Thread.Sleep(2000);
            searchBar.SendKeys(Keys.Enter);
            Thread.Sleep(1500);
            driver.Quit();
            driver.Dispose();
        }
        
        [Fact]
        [Trait("Home page", "Products")]
        public void PriceOnDiscountsAreProperlyCalculated()
        {
            var onlineShop = driver.FindElement(By.LinkText("Handla online"));
            onlineShop.Click();

            //var discountQuantity = driver.FindElement(By.CssSelector("span[class=Splash-pricePre]")).Text;
            //testOutput.WriteLine(discountQuantity);
            //driver.Quit();
            //driver.Dispose();
        }

        [Fact]
        public void Test4()
        {

        }
    }
}
