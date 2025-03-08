using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using Microsoft.Playwright;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PlaywrightNUnitDemo
{
    public class BaseFunctionality
    {
        protected IPage _page;
        private readonly IBrowser _browser;
        private ExtentReports _extent;
        private ExtentTest _test;
        private ExtentHtmlReporter _reporter;

        public BaseFunctionality(IBrowser browser, IPage page)
        {
            _browser = browser;
            _page = page;
        }

        public void InitializeReport()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var reportPath = Path.Combine(Directory.GetCurrentDirectory(),
             "TestReport", $"TestReport_{timestamp}.html");
             var directoryPath = Path.GetDirectoryName(reportPath);
             if (!Directory.Exists(directoryPath))
             {
                 Directory.CreateDirectory(directoryPath);
             }
            _reporter = new ExtentHtmlReporter(reportPath);
            _extent = new ExtentReports();
            _extent.AttachReporter(_reporter);
            Console.WriteLine("Report will be generated at: " + reportPath);
        }

        public async Task TakeScreenshotOnFailureAsync(string testName)
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var screenshotPath = Path.Combine(Directory.GetCurrentDirectory(),
                "Screenshots", $"{testName}_Fail_{timestamp}.png");

                var directoryPath = Path.GetDirectoryName(screenshotPath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                await _page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = screenshotPath,
                    FullPage = true
                });

                Console.WriteLine($"Screenshot captured: {screenshotPath}");
                _test?.AddScreenCaptureFromPath(screenshotPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error taking failure screenshot: {ex.Message}");
            }
        }
        public ExtentTest CreateTest(string testName)
        {
            return _extent.CreateTest(testName);
        }

        public void StartTest(string testName)
        {
           _test = _extent.CreateTest(testName);
        }

        public void FinalizeReport()
        {
            _extent.Flush();
            Console.WriteLine("Report finalized.");
        }
    }
}
