namespace PlaywrightNUnitDemo.PageObjects
{
    public static class PropertyPageObjects
    {
        public const string MediaSection = "//*[@id='media']";
        public const string AddressSection = "//h1";
        public const string FeaturesSection = "//section[@id='features']";
        public const string NeighbourhoodSection = "//section[@id='neighbourhood']";

        public static string GetMenuLocator(string menuName)
        {
           return $"//span[.=\"{menuName}\"]";
        }
    }
}
