using Microsoft.Playwright;
using System.Threading.Tasks;
using PlaywrightNUnitDemo.PageObjects;

namespace PlaywrightNUnitDemo.Pages
{
    public class HomePageMethods
        {
            private readonly IBrowser _browser;
            private readonly IPage _page;
            private const string KoopMenu = "Koop";
            private const string EnterKey = "Enter";

        public HomePageMethods(IBrowser browser, IPage page)
        {
            _browser = browser ?? throw new ArgumentNullException(nameof(browser));
            _page = page ?? throw new ArgumentNullException(nameof(page));
        }

        public async Task ClickSearch()
        {
           await _page.ClickAsync(HomePageObjects.SearchButton);
           bool isSearchButtonVisible = await _page.IsVisibleAsync(HomePageObjects.SearchButton);
           if (!isSearchButtonVisible) {}
        }

        public async Task<bool> IsSearchFieldVisible()
        {
            await _page.WaitForSelectorAsync(HomePageObjects.GetMenuLocator(KoopMenu));
            return await _page.IsVisibleAsync(HomePageObjects.SearchBoxField);
        }

        public async Task<bool> IsSearchButtonVisible()
        {
           await _page.WaitForSelectorAsync(HomePageObjects.GetMenuLocator(KoopMenu));
           return await _page.IsVisibleAsync(HomePageObjects.SearchButton);
        }

        public async Task<bool> IsMenuVisible(string menuName)
        {
           return await _page.IsVisibleAsync(HomePageObjects.GetMenuLocator(menuName));
        }

        public async Task<bool> IsFirstPropertyVisible()
        {
           return await _page.IsVisibleAsync(HomePageObjects.FirstProperty);
        }

        public async Task<bool> IsHeaderVisible()
        {
          return await _page.IsVisibleAsync(HomePageObjects.Header);
        }

        public async Task<bool> IsFooterVisible()
        {
           return await _page.IsVisibleAsync(HomePageObjects.Footer);
        }

        public async Task ClickButtonAndApplyCookiesAsync()
        {
           try
              {
              if (_page == null || _browser == null)
              {
                 throw new InvalidOperationException("Browser or page is not initialized.");
              }
                 await _page.WaitForSelectorAsync(HomePageObjects.AgreeButton);
                 await _page.ClickAsync(HomePageObjects.AgreeButton);
              }
              catch (Exception ex)
              {
                 Console.WriteLine($"Error: {ex.Message}");
              }
        }

        public async Task InputTextAndPressEnterAsync(string text)
        {
            await _page.WaitForSelectorAsync(HomePageObjects.SearchBoxField);
            await _page.FillAsync(HomePageObjects.SearchBoxField, "");
            await _page.FillAsync(HomePageObjects.SearchBoxField, text);
            await _page.WaitForSelectorAsync(HomePageObjects.FirstSuggestionSearchBox);
            await _page.PressAsync(HomePageObjects.SearchBoxField, EnterKey);
        }

        public async Task PressEnterWithoutSearchDataAsync()
        {
           await _page.WaitForSelectorAsync(HomePageObjects.SearchBoxField);
           await _page.FillAsync(HomePageObjects.SearchBoxField, "");
           await _page.PressAsync(HomePageObjects.SearchBoxField, EnterKey);
        }

        public async Task<bool> IsElementByTextVisible(string text)
        {
            var locator = GetLocatorByText(text);
            return await locator.IsVisibleAsync();
        }

        public async Task ClickElementByText(string text)
        {
            var locator = GetButtonByText(text);
            await locator.ClickAsync();
        }

        public ILocator GetButtonByText(string text)
        {
            return _page.Locator(string.Format(HomePageObjects.ButtonByText, text));
        }

        public ILocator GetLocatorByText(string text)
        {
            return _page.Locator(string.Format(HomePageObjects.DivLocatorByText, text));
        }

        public async Task ClickFirstProperty()
        {
           await _page.ClickAsync(HomePageObjects.FirstProperty);
           bool isFirstPropertyVisible = await _page.IsVisibleAsync(HomePageObjects.FirstProperty);
           if (!isFirstPropertyVisible) {}
        }
    }
}
