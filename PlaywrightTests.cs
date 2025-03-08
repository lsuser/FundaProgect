using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;
using AventStack.ExtentReports;

namespace PlaywrightNUnitDemo.Pages
{
    [TestFixture]
    public class HomePageTests
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        private BrowserSetup _browserSetup;
        private IPage _page;
        private HomePageMethods _homePageMethods;
        private PropertyPageMethods _propertyPageMethods;
        private BaseFunctionality _baseFunctionality;
        private SoftAssert softAssert;
        private ExtentTest test;
        private static string MessageTemplate = "{0} should be visible";
        private static string URL_Message = "The current URL should match the expected search results URL";
        private static string TestPassedMessage = "Test passed successfully";
        private static string NavigationMessage = "Navigating to the Home Page";
        private static string OpFundaMessage = "op funda";
        private static string Huur = "Huur";

        [SetUp]
        public async Task Setup()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            _browserSetup = new BrowserSetup();
            _page = await _browserSetup.SetupWithCustomUserAgentAsync();
            _baseFunctionality = new BaseFunctionality(_browser, _page);
            _homePageMethods = new HomePageMethods(_browser, _page);
            _propertyPageMethods = new PropertyPageMethods(_browser, _page);
            softAssert = new SoftAssert();
            _baseFunctionality.InitializeReport();
            _baseFunctionality.StartTest(TestContext.CurrentContext.Test.Name);
        }

        [Test, Category("Smoke")]
        [TestCase(false, TestName = "OpenHomePageAndCheckElements_Desktop")]
        [TestCase(true, TestName = "OpenHomePageAndCheckElements_Mobile")]
        public async Task OpenHomePageAndCheckElements(bool isMobile)
        {
            SetupDataForTest($"Open Home Page and Check Elements - {(isMobile ? "Mobile" : "Desktop")}", isMobile);
            await _page.GotoAsync(PageResources.HomePage);
            await _homePageMethods.ClickButtonAndApplyCookiesAsync();

            test.Log(AventStack.ExtentReports.Status.Info, "Check Search field on Home page");
            softAssert.AssertTrue(await _homePageMethods.IsSearchFieldVisible(),
                string.Format(MessageTemplate, "Search field"));

            test.Log(AventStack.ExtentReports.Status.Info, "Check Menu on Home page");
            softAssert.AssertTrue(await _homePageMethods.IsMenuVisible("Koop"), string.Format(MessageTemplate, "Koop"));
            softAssert.AssertTrue(await _homePageMethods.IsMenuVisible(Huur), string.Format(MessageTemplate, Huur));
            softAssert.AssertTrue(await _homePageMethods.IsMenuVisible("Nieuwbouw"),
                string.Format(MessageTemplate, "Nieuwbouw"));

            softAssert.AssertAll();
            test.Log(AventStack.ExtentReports.Status.Pass, TestPassedMessage);
        }

        [Test, Category("Smoke")]
        [TestCase(false, TestName = "SearchFunctionalityBuyWithoutEnteringDataTest_Desktop")]
        [TestCase(true, TestName = "SearchFunctionalityBuyWithoutEnteringDataTest_Mobile")]
        public async Task SearchFunctionalityBuyWithoutEnteringDataTest(bool isMobile)
        {
            SetupDataForTest($"Search Functionality Buy without entered data - {(isMobile ? "Mobile" : "Desktop")}", isMobile);
            await _page.GotoAsync(PageResources.HomePage);
            await _homePageMethods.ClickButtonAndApplyCookiesAsync();
            await _homePageMethods.PressEnterWithoutSearchDataAsync();

            softAssert.AssertTrue(await _homePageMethods.IsElementByTextVisible(OpFundaMessage),
                string.Format(MessageTemplate, OpFundaMessage));
            softAssert.AssertAreEqual(PageResources.KoopPage, _page.Url, URL_Message);
            softAssert.AssertAll();
            test.Log(AventStack.ExtentReports.Status.Pass, TestPassedMessage);
        }

        [Test, Category("Smoke")]
        [TestCase("Amsterdam", false, TestName = "SearchFunctionalityBuyWithEnteredDataTest_Amsterdam_Desktop")]
        [TestCase("Amsterdam", true, TestName = "SearchFunctionalityBuyWithEnteredDataTest_Amsterdam_Mobile")]
        [TestCase("Rotterdam", false, TestName = "SearchFunctionalityBuyWithEnteredDataTest_Rotterdam_Desktop")]
        [TestCase("Rotterdam", true, TestName = "SearchFunctionalityBuyWithEnteredDataTest_Rotterdam_Mobile")]
        [TestCase("Utrecht", false, TestName = "SearchFunctionalityBuyWithEnteredDataTest_Utrecht_Desktop")]
        [TestCase("Utrecht", true, TestName = "SearchFunctionalityBuyWithEnteredDataTest_Utrecht_Mobile")]
        public async Task SearchFunctionalityBuyWithEnteredDataTest(string city, bool isMobile)
        {
            SetupDataForTest($"Search Functionality with entered data - {city} - {(isMobile ? "Mobile" : "Desktop")}", isMobile);
            await _page.GotoAsync(PageResources.HomePage);
            await _homePageMethods.ClickButtonAndApplyCookiesAsync();
            await _homePageMethods.InputTextAndPressEnterAsync(city);

            softAssert.AssertTrue(await _homePageMethods.IsElementByTextVisible($"in {city}"),
                string.Format(MessageTemplate, $"in {city}"));
            softAssert.AssertAreEqual(PageResources.GetSearchPageUrl(city), _page.Url, URL_Message);
            softAssert.AssertAll();
            test.Log(AventStack.ExtentReports.Status.Pass, TestPassedMessage);
        }

        [Test, Category("Smoke")]
        [TestCase(false, TestName = "OpenPropertyPageAndCheckElementsTest_Desktop")]
        [TestCase(true, TestName = "OpenPropertyPageAndCheckElementsTest_Mobile")]
        public async Task OpenPropertyPageAndCheckElementsTest(bool isMobile)
        {
            SetupDataForTest($"Open first Property Page and check elements - {(isMobile ? "Mobile" : "Desktop")}", isMobile);
            await _page.GotoAsync(PageResources.AmsterdamSearchPage);
            await _homePageMethods.ClickButtonAndApplyCookiesAsync();
            await _homePageMethods.ClickFirstProperty();

            softAssert.AssertTrue(await _propertyPageMethods.IsMediaSectionVisible(),
                string.Format(MessageTemplate, "Media Section"));
            softAssert.AssertTrue(await _propertyPageMethods.IsAddressSectionVisible(),
                string.Format(MessageTemplate, "Address Section"));
            softAssert.AssertAll();
            test.Log(AventStack.ExtentReports.Status.Pass, TestPassedMessage);
        }

        [Test, Category("Regression")]
        [TestCase(false, TestName = "CheckForBrokenLinks_Desktop")]
        [TestCase(true, TestName = "CheckForBrokenLinks_Mobile")]
        public async Task CheckForBrokenLinks(bool isMobile)
        {
            SetupDataForTest($"Check for Broken Links - {(isMobile ? "Mobile" : "Desktop")}", isMobile);
            await _page.GotoAsync(PageResources.HomePage);
            await _homePageMethods.ClickButtonAndApplyCookiesAsync();
            var links = await _page.Locator("a").EvaluateAllAsync<string[]>("elements => elements.map(e => e.href)");

            foreach (var link in links)
            {
                if (!string.IsNullOrEmpty(link) && (link.StartsWith("http") || link.StartsWith("https")))
                {
                    var response = await _page.APIRequest.GetAsync(link);
                    int statusCode = response.Status;
                    softAssert.AssertTrue(statusCode == 200, $"Broken link detected: {link} (Status: {statusCode})");
                }
            }

            softAssert.AssertAll();
            test.Log(AventStack.ExtentReports.Status.Pass, TestPassedMessage);
        }

        private async Task SetupDataForTest(string testName, bool isMobile = false)
        {
            softAssert = new SoftAssert();
            test = _baseFunctionality.CreateTest(testName);
            test.Log(AventStack.ExtentReports.Status.Info, NavigationMessage);
            if (isMobile)
            {
                await _page.SetViewportSizeAsync(375, 812); // iPhone X resolution
            }
        }

        [TearDown]
        public async Task TearDown()
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                await _baseFunctionality.TakeScreenshotOnFailureAsync(TestContext.CurrentContext.Test.Name);
            }
            await _browser.CloseAsync();
            _playwright.Dispose();
            _baseFunctionality.FinalizeReport();
        }
    }
}
