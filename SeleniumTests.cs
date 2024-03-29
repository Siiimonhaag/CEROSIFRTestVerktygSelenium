using System;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using WebDriverManager;
using OpenQA.Selenium.Support.UI;
using Xunit;
using Xunit.Abstractions;
using System.Threading;
using System.Collections.Generic;

namespace CEROSIFRTestVerktygSelenium
{

    public class SeleniumTests
    {
        ITestOutputHelper testOutput;
        ChromeOptions options = new ChromeOptions();
        IWebDriver driver;
        Helper helper;
        string url = "https://www.coop.se/";
        Actions actions;
        WebDriverWait wait;

        public SeleniumTests(ITestOutputHelper _testOutput)
        {
            options.AddArguments("--start-fullscreen");
            driver = new ChromeDriver(options);
            helper = new Helper(driver);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            actions = new Actions(driver);
            // Konstruktor f�r att
            // Klicka p� acceptera kakor varje g�ng vi k�r ett test
            // B�da click nedan funkar!
            testOutput = _testOutput;

            try
            {
                driver.Navigate().GoToUrl(url);
                Thread.Sleep(3000);
                driver.FindElement(By.XPath("//*[@id='cmpbntyestxt']")).Click();
            }
            catch (Exception)
            {
                driver.FindElement(By.XPath("//*[@id='cmpbntyestxt']")).Click();
            }
        }

        [Fact]
        [Trait("User story ID 1","Input, Button, Anchor")]
        public void ChangeQuantityInTheShoppingCart()
        {
            helper.LogInToWebsite("testcoop123@hotmail.com", "Cerosifr123!");

            IWebElement handlaOnline = driver.FindElement(By.LinkText("Handla online"));
            handlaOnline.Click();
            Thread.Sleep(2000);

            IWebElement searchBar = driver.FindElement(By.ClassName("Search-input"));
            searchBar.SendKeys("Kronf�gel Majskyckling");

            searchBar.SendKeys(Keys.Enter);
            Thread.Sleep(3000);

            IWebElement addProduct = driver.FindElement(By.XPath("//a[@aria-label='Br�stfil� Majskyckling']"));
            addProduct.Click();
            Thread.Sleep(2500);

            IWebElement plusButton = driver.FindElement(By.XPath("//button[@aria-label='�ka antal']"));
            plusButton.Click();
            Thread.Sleep(2500);

            IWebElement shoppingCart = driver.FindElement(By.XPath("//button[@aria-label='kundvagn']"));
            shoppingCart.Click();
            Thread.Sleep(2500);

            IWebElement increaseQuantity = driver.FindElement(By.XPath("//button[@aria-label='�ka antal']"));
            increaseQuantity.Click();
            Thread.Sleep(2500);

            string actual = driver.FindElement(By.XPath("//input[@type='numeric']")).GetAttribute("value").ToString();
            string expected = "2";
            Assert.Equal(expected,actual);
            testOutput.WriteLine("Added an extra 'Majskyckling' in the shopping cart.\n" +
                "Expected result: 2\n" +
                "Actual result: " + actual);

            helper.EmptyTheCart();
            Thread.Sleep(500);

            driver.Quit();
            driver.Dispose();
        }

        [Fact]
        [Trait("User Story ID 11", "Div")]
        public void ShoppingCartShowCorrectPriceDiscount()
        {
            helper.LogInToWebsite("testcoop123@hotmail.com", "Cerosifr123!");
            Thread.Sleep(2000);

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            var onlineShop = driver.FindElement(By.LinkText("Handla online"));
            onlineShop.Click();
            Thread.Sleep(1200);

            IList<IWebElement> miniArticles = wait.Until(driver =>
            driver.FindElements(By.ClassName("ItemTeaser-content")));

            for (int i = 0; i < miniArticles.Count; i++)
            {
                try
                {
                    if (miniArticles[i].Text.Contains("f�r") &&
                    miniArticles[i].Text.Contains("Medlemspris") == false)
                    {
                        actions.ScrollToElement(miniArticles[i]);
                        actions.Perform();
                        Thread.Sleep(1200);
                        miniArticles[i].Click();
                        break;
                    }
                }
                catch (StaleElementReferenceException)
                {
                    miniArticles = wait.Until(driver =>
                    driver.FindElements(By.ClassName("ItemTeaser-content")));
                    i = 0;
                }
            }

            int quantity = 0;
            int discountPrice = 0;
            double originalPrice = 0;
            int incentive = 0;

            for (int i = 0; i < 2; i++)
            {
                try
                {
                    Thread.Sleep(2000);
                    string quantityText = wait.Until(driver =>
                    driver.FindElement(By.ClassName("Splash-pricePre"))).Text.Substring(0, 1);
                    quantity = int.Parse(quantityText.Trim());

                    string discountPriceText = wait.Until(driver =>
                    driver.FindElement(
                        By.ClassName("Splash-priceLarge"))).Text.
                        Replace(":-", "");
                    discountPrice = int.Parse(discountPriceText.Trim());

                    string originalPriceText = wait.Until(driver =>
                    driver.FindElement(
                        By.CssSelector("span[aria-label='Pris']"))).Text.
                        Replace("/st", "").Replace(":", "");
                    originalPrice = double.Parse(originalPriceText.Trim()) / 100;

                    // L�gg till pantpris p� rabatt och originalpriset om pant finns
                    try
                    {
                        char[] recyclingInfo = driver.FindElement(
                            By.ClassName("ItemInfo-extra")).Text.ToCharArray();
                        int index = recyclingInfo.Length - 3;
                        bool parse = 
                            int.TryParse(recyclingInfo[index].ToString(), out incentive);

                        discountPrice += incentive * quantity;
                        originalPrice += incentive;
                        testOutput.WriteLine("originalPrice with incentive: " + originalPrice);
                    }
                    catch (NoSuchElementException)
                    {
                    }
                }
                catch (StaleElementReferenceException)
                {
                }
            }
            IWebElement addIcon;
            for (int i = 0; i < quantity; i++)
            {
                addIcon = wait.Until(product =>
                product.FindElement(By.CssSelector("button[aria-label='�ka antal']")));
                addIcon.Click();
            }

            //Navigera till kundvagn
            IWebElement miniCart;
            miniCart = wait.Until(driver =>
            driver.FindElement(By.CssSelector("button[aria-label='kundvagn']")));
            Thread.Sleep(1000);
            miniCart.Click();

            //H�mta avdraget pris och totalpriset
            Thread.Sleep(2000);
            IList<IWebElement> cartSummary;
            do
            {
                cartSummary = wait.Until(driver =>
                driver.FindElements(By.CssSelector("span[class=Cart-summaryItem]")));
            } while (cartSummary.Count == 0);
            
            string cartDiscountText = string.Join
                ("", cartSummary[1].Text.Split(':', '-')).Replace("kr", "").Trim();

            double discSubtraction = double.Parse(cartDiscountText) / 100;

            //T�m kundkorgen innan dispose
            helper.EmptyTheCart();

            driver.Quit();
            driver.Dispose();

            // Validera om kundvagnen visar r�tt prisavdrag
            double expected = Math.Round(originalPrice * quantity - discountPrice, 2);
            double actual = discSubtraction;
            Assert.Equal(expected, actual);
            testOutput.WriteLine("Expected: " + expected + "\nActual: " + actual);
        }

        [Fact]
        [Trait("User story ID 2", "Anchor, Button, Input, Div")]
        public void ShopForASmallerAmountThan500()

        {

            helper.LogInToWebsite("testcoop123@hotmail.com", "Cerosifr123!");
            Thread.Sleep(2000);

            var HandlaOnline = driver.FindElement(By.LinkText("Handla online"));
            HandlaOnline.Click();
            Thread.Sleep(1000);

            IWebElement Searching = driver.FindElement(By.ClassName("Search-input"));
            Searching.SendKeys("Baguette Vitl�k 6-pack");
            Thread.Sleep(1000);

            Searching.SendKeys(Keys.Enter);
            Thread.Sleep(2500);

            IWebElement viewProduct = driver.FindElement(By.XPath("//a[@aria-label='Baguette Vitl�k 6-pack']"));
            viewProduct.Click();
            Thread.Sleep(2000);

            IWebElement addTheProduct = driver.FindElement(By.XPath("//button[@class='AddToCart-button AddToCart-button--add']"));
            addTheProduct.Click();
            Thread.Sleep(2000);

            IWebElement clickTheCartButton = driver.FindElement(By.XPath("//div[@class='CartButton-icon CartButton-icon--small']"));
            clickTheCartButton.Click();
            Thread.Sleep(1500);

            IWebElement toTheRegister = driver.FindElement(By.XPath("//a[@data-test='minicart-gotocheckoutbutton']"));
            toTheRegister.Click();
            Thread.Sleep(1500);

            IWebElement moveForward;
            try
            {
                moveForward = driver.FindElement(By.XPath("//button[@class='Button Button--green Button--radius Button--responsivePadding']"));
                moveForward.Click();
                testOutput.WriteLine("Inne i Try-f�ltet");
            }
            catch (Exception)
            {
                moveForward = driver.FindElement(By.XPath("//nav/button[text()=\"G� vidare\"]"));
                moveForward.Click();
                testOutput.WriteLine("Inne i Catch-f�ltet");
            }
            Thread.Sleep(1500);

            bool shopForMoreInfo = driver.FindElement(By.ClassName("Notice-content")).Text
                .Contains("Handla f�r ytterligare");
            
            var removeProduct = wait.Until(driver =>
                driver.FindElement(By.CssSelector(".Cart-item button[class*=\"subtract\"]")));
                removeProduct.Click();
            Thread.Sleep(1500);

            testOutput.WriteLine("shopForMoreInfo = " + shopForMoreInfo);

            driver.Quit();
            driver.Dispose();

            Assert.True(shopForMoreInfo);
        }

        [Fact]
        [Trait("User story ID 3","Input")]
        public void Add10pcsDirectlyOnInputOfTheProduct()
        {
            //Wait anv�nds ej i detta test ens?
            //var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            var HandlaOnline = driver.FindElement(By.LinkText("Handla online"));
            HandlaOnline.Click();
            Thread.Sleep(2000);

            IWebElement Search = driver.FindElement(By.ClassName("Search-input"));
            Search.SendKeys("�delost Bl� eko");
            Thread.Sleep(1500);

            Search.SendKeys(Keys.Enter);
            Thread.Sleep(2000);

            IWebElement AddAmount = driver.FindElement(By.ClassName("AddToCart-input"));
            AddAmount.SendKeys("10");
            Thread.Sleep(2000);

            IWebElement ClickOk = driver.FindElement(By.ClassName("AddToCart-input"));
            ClickOk.SendKeys(Keys.Enter);
            Thread.Sleep(3000);
           
            IList<IWebElement> AddZipCode = driver.FindElements(By.CssSelector("input"));
            int stop = 0;
                       
            foreach (var box in AddZipCode)
                {
                    if (stop == 5)
                        {
                            break;
                        }
                    box.SendKeys("4");
                    Thread.Sleep(500);
                    stop++;
                }

            Thread.Sleep(3500);
            IWebElement ClickTime = driver.FindElement(By.ClassName("TimeslotCell-content"));
            ClickTime.SendKeys(Keys.Enter);
            Thread.Sleep(2000);

            string actual = driver.FindElement(By.XPath("//input[@type='numeric']")).GetAttribute("value").ToString();
            string expected = "10";
            Assert.Equal(expected, actual);
            testOutput.WriteLine("Added 10pcs of '�delost Bl� Eko' in the shopping cart.\n" +
                "Expected result: 10\n" +
                "Actual result: " + actual);
            Thread.Sleep(1000);

            helper.EmptyTheCart();
            driver.Quit();
            driver.Dispose();
        }

        [Fact]
        [Trait("User story ID 5", "Li, A, Input, Button, Span")]
        public void NavigateWithSearchForRecipes()
        {
            helper.LogInToWebsite("testcoop123@hotmail.com", "Cerosifr123!");

            //Check if the Linktext shows up. If not navigate to menu
            try
            {
                IWebElement recept = driver.FindElement(By.LinkText("Recept"));
                recept.Click();
            }
            catch (NoSuchElementException)
            {
                IWebElement menu = driver.FindElement(By.CssSelector("button[aria-label=Meny]"));
                menu.Click();
                Thread.Sleep(1000);
                IWebElement menuRecept = driver.FindElement(By.CssSelector("li[class*=link2]"));
                menuRecept.Click();
            }
            Thread.Sleep(1500);

            //Navigate
            driver.FindElement(By.XPath("//button/span[text()='M�ltid']")).Click();
            Thread.Sleep(1500);

            driver.FindElement(By.LinkText("Huvudr�tt")).Click();
            Thread.Sleep(1500);

            driver.FindElement(By.XPath("//button/span[text()='Vegetariskt/veganskt']")).Click();
            Thread.Sleep(1500);

            driver.FindElement(By.LinkText("Veganskt")).Click();
            Thread.Sleep(1500);

            driver.FindElement(By.XPath("//button/span[text()='Sv�righetsgrad']")).Click();
            Thread.Sleep(1500);

            driver.FindElement(By.LinkText("Snabb")).Click();
            Thread.Sleep(1500);

            //Scroll into view for the header
            //driver.ExecuteScript("document.querySelector(\"div[class*='Hero-content']\").scrollIntoView()");

            Thread.Sleep(1500);
            var result = driver.FindElement(By.XPath("//p/b"));

            Assert.NotNull(result.Text);
            var productView = driver.FindElement(By.CssSelector("article a[href='/recept/vegetarisk-chiligryta/']"));

            testOutput.WriteLine("Number of result: " + result.Text);
            Thread.Sleep(1500);

            productView.Click();
            Thread.Sleep(1500);

            driver.Quit();
            driver.Dispose();
        }

        [Fact]
        [Trait("User story ID 7", "Li, Input, Button, Anchor")]
        public void AddingIngredientsFromRecipeIsVisibleInShoppingList()
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            helper.LogInToWebsite("testcoop123@hotmail.com", "Cerosifr123!");
            Thread.Sleep(2000);

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // Navigera till recept
            try
            {
                IWebElement recept = driver.FindElement(By.LinkText("Recept"));
                recept.Click();
            }
            catch (NoSuchElementException)
            {
                IWebElement menu = driver.FindElement(By.CssSelector("button[aria-label=Meny]"));
                menu.Click();
                Thread.Sleep(1000);
                IWebElement menuRecept = driver.FindElement(By.CssSelector("li[class*=link2]"));
                menuRecept.Click();
            }
            Thread.Sleep(1200);

            // S�k efter pasta och v�lj f�rsta b�sta alternativ
            var pastaFilter = wait.Until(driver =>
            driver.FindElement(By.LinkText("Pasta")));
            Thread.Sleep(1200);
            pastaFilter.Click();

            var recipeArticle = wait.Until(driver =>
            driver.FindElement(By.CssSelector("article a")));
            Thread.Sleep(1200);
            recipeArticle.Click();

            
            Thread.Sleep(1500);
            // R�kna upp hur m�nga ingredienser receptet har
            var quantityElement = wait.Until(driver =>
            driver.FindElement(By.XPath(
                "//li[contains(@class,'List-heading')]/span[contains(text(),'(')]")));
            int[] parentheses =
            {
                quantityElement.Text.IndexOf("(") + 1,
                quantityElement.Text.IndexOf(")") - 1
            };

            int recipeQuantity = int.Parse
                (quantityElement.Text.Substring(parentheses[0], parentheses[1]));

            // L�gg till i ett klick alla ingredienser till en ink�pslista
            var addAllIngredients = wait.Until(driver =>
            driver.FindElement(By.CssSelector("div[class='js-shoppingList'] button")));
            /*
             * Selenium kan inte genom vanliga medel klicka p� knappen pga av att elementet
             * �r inne i ett "g�mt" tillst�nd, trots att den �r fullt synlig och klickbar f�r
             * en anv�ndare. L�sningen blir att l�ta Javascript klicka �t oss ist�llet f�r Selenium
             */
            js.ExecuteScript("document.querySelector('div[class=\"js-shoppingList\"] button').click()");
            Thread.Sleep(1200);
            
            var newShoppingListButton = wait.Until(driver =>
            driver.FindElement(By.XPath("//button[text()='+ L�gg till i ny lista']")));
            Thread.Sleep(1200);
            newShoppingListButton.Click();

            var closeWindow = wait.Until(driver =>
            driver.FindElement(By.CssSelector("button[class*='close']")));
            Thread.Sleep(1200);
            closeWindow.Click();
            
            // Navigera till testkontot med ink�pslistor
            Thread.Sleep(1200);
            js.ExecuteScript("document.querySelector('a[title=\"Mitt Coop\"]').click()");

            wait.Until(driver =>
            driver.FindElement(By.LinkText("Mina ink�pslistor"))).Click();
            Thread.Sleep(1200);

            IList<IWebElement> shoppingList = wait.Until(driver =>
            driver.FindElements(By.ClassName("Checkbox")));
            
            //Radera ink�pslistan f�r att h�lla testkontot st�dat
            wait.Until(driver =>
            driver.FindElement(By.XPath("//button[text()=\"Redigera\"]"))).Click();
            Thread.Sleep(2000);

            wait.Until(driver =>
            driver.FindElement(By.XPath("//button[text()=\"Ta bort ink�pslistan\"]"))).Click();
            Thread.Sleep(1200);

            wait.Until(driver =>
            driver.FindElement(By.XPath("//button[text()=\"Radera\"]"))).Click();
            Thread.Sleep(500);

            driver.Quit();
            driver.Dispose();

            // Validera testet med att kolla om det finns r�tt antal ingredienser i listan kontra receptet
            int expected = recipeQuantity;
            int actual = shoppingList.Count;
            testOutput.WriteLine("Expected: " + expected + "\nActual: " + actual);

            Assert.Equal(expected, actual);
        }
        [Fact]
        [Trait("User story ID 8", "Search")]
        public void SearchForSpecificStoreInfo()
        {
            helper.LogInToWebsite("testcoop123@hotmail.com", "Cerosifr123!");

            Thread.Sleep(2000);
            var onlineShop = driver.FindElement(By.LinkText("Butiker & erbjudanden"));
            onlineShop.Click();

            Thread.Sleep(2000);
            IWebElement searchBar = driver.FindElement(By.CssSelector("input[placeholder*=postnummer]"));
            searchBar.SendKeys("Stora Coop Bor�s");

            Thread.Sleep(1000);
            searchBar.SendKeys(Keys.Enter);

            Thread.Sleep(2000);
            var storeInfo = driver.FindElement(By.LinkText("Erbjudanden och butiksinfo"));
            storeInfo.Click();

            Thread.Sleep(2000);
            var actual = driver.FindElement(By.CssSelector("div[class=StoreSelector--headerDesktop] span[class=Link2-text]")).Text;
            string expected = "Enedalsg. 10, BOR�S";

            Thread.Sleep(1000);
            Assert.Equal(expected, actual);
            testOutput.WriteLine("Expected result: " + expected + "\nActual result: " + actual);

            driver.Quit();
            driver.Dispose();
        }

        [Fact]
        [Trait("User story ID 6", "Li, Input, A, Button, H1, H3, P")]
        public void SaveRecipe()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            helper.LogInToWebsite("testcoop123@hotmail.com", "Cerosifr123!");
            Thread.Sleep(2000);

            try
            {
                IWebElement recept = driver.FindElement(By.LinkText("Recept"));
                recept.Click();
            }
            catch (NoSuchElementException)
            {
                IWebElement menu = driver.FindElement(By.CssSelector("button[aria-label=Meny]"));
                menu.Click();
                Thread.Sleep(1000);
                IWebElement menuRecept = driver.FindElement(By.CssSelector("li[class*=link2]"));
                menuRecept.Click();
            }
            

            Thread.Sleep(1500);
            IWebElement searchBar = driver.FindElement(By.CssSelector("input[placeholder*=filter]"));
            searchBar.Click();

            Thread.Sleep(1500);
            searchBar.SendKeys("Sommar");

            Thread.Sleep(1000);
            searchBar.SendKeys(Keys.Enter);

            Thread.Sleep(1500);
            IWebElement choosenRecipe = driver.FindElement(By.XPath("//a[@href='/recept/zucchinitzatziki/']"));
            choosenRecipe.Click();

            Thread.Sleep(1000);
            IList<IWebElement> likeButton = wait.Until(product =>
               product.FindElements(By.CssSelector("button[title='Favorit']")));
            likeButton[1].Click();

            Thread.Sleep(1500);
            try
            {
                IWebElement recept = driver.FindElement(By.LinkText("Recept"));
                recept.Click();
            }
            catch (NoSuchElementException)
            {
                IWebElement menu = driver.FindElement(By.CssSelector("button[aria-label=Meny]"));
                menu.Click();
                Thread.Sleep(1000);
                IWebElement menuRecept = driver.FindElement(By.CssSelector("li[class*=link2]"));
                menuRecept.Click();
            }

            Thread.Sleep(1500);
            IWebElement savedRecipes = driver.FindElement(By.CssSelector("p[class*=u-marginAz]"));
            savedRecipes.Click();

            Thread.Sleep(1500);
            var myRecipes = driver.FindElement(By.CssSelector("h1")).Text;
            var mySavedRecipe = driver.FindElement(By.CssSelector("h3[class=u-marginAz]")).Text;
            var expectedMyRecipe = "Mina recept";
            var expectedMySavedRecipe = "Zucchinitzatziki";

            Thread.Sleep(1000);
            Assert.Equal(myRecipes, expectedMyRecipe);
            Assert.Equal(mySavedRecipe, expectedMySavedRecipe);

            IWebElement removeRecipe = driver.FindElement(By.CssSelector("button[title*='Ta bort']"));
            removeRecipe.Click();

            driver.Quit();
            driver.Dispose();
        }
    }
}
