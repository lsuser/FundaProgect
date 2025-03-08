using Microsoft.Playwright;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlaywrightNUnitDemo
{
    public class BrowserSetup
    {
        public async Task<IPage> SetupWithCustomUserAgentAsync()
        {
            var browserType = await Playwright.CreateAsync();
            var browser = await browserType.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });

            string userAgent = LoadUserAgentFromConfig();

            var contextOptions = new BrowserNewContextOptions
            {
                UserAgent = userAgent
            };

            var context = await browser.NewContextAsync(contextOptions);
            var page = await context.NewPageAsync();

            return page;
        }

        private string LoadUserAgentFromConfig()
        {
           string configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");

               if (!File.Exists(configPath))
               {
                   throw new FileNotFoundException($"Configuration file not found at: {configPath}");
               }

               string json = File.ReadAllText(configPath);
               var config = JsonSerializer.Deserialize<ConfigModel>(json);

               return config?.UserAgent ?? throw new Exception("UserAgent not found in config.json");
        }
    }

    public class ConfigModel
    {
        public string UserAgent { get; set; }
    }
}
