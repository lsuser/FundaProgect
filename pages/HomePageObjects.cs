namespace PlaywrightNUnitDemo.PageObjects
{
    public static class HomePageObjects
    {
        public const string SearchBoxField = "[data-testid='search-box']";
        public const string SearchButton = "//*[@data-testid='search-box']//..//button";
        public const string AgreeButton = "[id='didomi-notice-agree-button']";
        public const string FirstSuggestionSearchBox = "//li[@data-testid='SearchBox-location-suggestion'][1]";
        public const string Header = "header nav";
        public const string Footer = "footer";
        public const string ButtonByText = "//button[normalize-space()='{0}']";
        public const string DivLocatorByText = "//div[normalize-space()='{0}']";
        public const string FirstProperty = "(//h2//a)[1]";

        public static string GetMenuLocator(string menuName)
        {
           return $"//button[@data-text='{menuName}']";
        }
    }
}
