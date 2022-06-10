using System;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using WebDriverManager;
using OpenQA.Selenium.Support;
using Xunit;
using System.Threading;
using System.Collections.Generic;

namespace CEROSIFRTestVerktygSelenium
{

    public class SeleniumTests
    {

        //ChromeDriver driver = new ChromeDriver();
        ChromeDriver driver = new ChromeDriver(@"C:\Users\simon\Downloads");
        DriverManager driverManager = new DriverManager();
        string url = "https://www.coop.se/";

        public SeleniumTests()
        {

            // Konstruktor för att
            // Klicka på acceptera kakor varje gång vi kör ett test
            // Båda click nedan funkar!
            driver.Navigate().GoToUrl(url);
            driver.Manage().Window.FullScreen();
            Thread.Sleep(2000);
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
        [Trait("ID 1","Input form & button")]
        public void ChangeQuantityInTheShoppingCart()
        {
            
            IWebElement logIn = driver.FindElement(By.XPath("//a[@title='Logga in / Mitt Coop']"));
            logIn.Click();
            driver.Manage().Window.FullScreen();
            Thread.Sleep(1500);

            IWebElement enterEmail = driver.FindElement(By.XPath("//input[@id='loginEmail']"));
            enterEmail.SendKeys("testcoop123@hotmail.com");

            IWebElement enterPassword = driver.FindElement(By.XPath("//input[@id='loginPassword']"));
            enterPassword.SendKeys("Cerosifr123!");

            IWebElement login2 = driver.FindElement(By.XPath("//button[@type='submit']"));
            login2.Click();
            Thread.Sleep(3000);

            IWebElement handlaOnline = driver.FindElement(By.LinkText("Handla online"));
            handlaOnline.Click();

            driver.Manage().Window.FullScreen();
            Thread.Sleep(1000);

            IWebElement searchBar = driver.FindElement(By.ClassName("Search-input"));
            searchBar.SendKeys("Kronfågel Majskyckling");
            Thread.Sleep(1500);

            searchBar.SendKeys(Keys.Enter);
            Thread.Sleep(2000);

            IWebElement addProduct = driver.FindElement(By.XPath("//a[@aria-label='Bröstfilé Av Majskyckling']"));
            addProduct.Click();
            Thread.Sleep(2500);

            IWebElement plusButton = driver.FindElement(By.XPath("//button[@aria-label='Öka antal']"));
            plusButton.Click();
            Thread.Sleep(2000);


            IWebElement shoppingCart = driver.FindElement(By.XPath("//button[@aria-label='kundvagn']"));
            shoppingCart.Click();
            Thread.Sleep(2000);

            IWebElement increaseQuantity = driver.FindElement(By.XPath("//button[@aria-label='Öka antal']"));
            increaseQuantity.Click();
            Thread.Sleep(1000);
            string actual = driver.FindElement(By.XPath("//input[@type='numeric']")).GetAttribute("value").ToString();
            string expected = "2";
            Assert.Equal(expected,actual);

            for (int i = 0; i < 2; i++)
            {
                IWebElement decreaseQuantity = driver.FindElement(By.XPath("//button[@aria-label='Minska antal']"));
                decreaseQuantity.Click();
                Thread.Sleep(500);

            }
            Thread.Sleep(2000);

            driver.Quit();
            driver.Dispose();
        }

        /*
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
