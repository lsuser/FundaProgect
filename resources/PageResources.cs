namespace PlaywrightNUnitDemo
{
    public static class PageResources
    {
        public static string HomePage => "https://www.funda.nl/";
        public static string AmsterdamSearchPage =>
        "https://www.funda.nl/zoeken/koop?selected_area=%5B%22amsterdam%22%5D";
        public static string GetSearchPageUrl(string city) =>
            $"https://www.funda.nl/zoeken/koop?selected_area=%5B%22{city.ToLower()}%22%5D";
        public static string KoopPage =>"https://www.funda.nl/zoeken/koop/";
        public static string HuurPage =>"https://www.funda.nl/zoeken/huur/";
    }
}
