using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using PlaywrightNUnitDemo.PageObjects;

namespace PlaywrightNUnitDemo.Pages
{
    public class PropertyPageMethods
    {
        private readonly IBrowser _browser;
        private readonly IPage _page;

        public PropertyPageMethods(IBrowser browser, IPage page)
        {
            _browser = browser ?? throw new ArgumentNullException(nameof(browser));
            _page = page ?? throw new ArgumentNullException(nameof(page));
        }

        public async Task<bool> IsMediaSectionVisible()
        {
            await _page.WaitForSelectorAsync(PropertyPageObjects.MediaSection);
            return await _page.IsVisibleAsync(PropertyPageObjects.MediaSection);
        }

        public async Task<bool> IsAddressSectionVisible()
        {
            await _page.WaitForSelectorAsync(PropertyPageObjects.AddressSection);
            return await _page.IsVisibleAsync(PropertyPageObjects.AddressSection);
        }

        public async Task<bool> IsFeaturesSectionVisible()
        {
            await _page.WaitForSelectorAsync(PropertyPageObjects.FeaturesSection);
            return await _page.IsVisibleAsync(PropertyPageObjects.FeaturesSection);
        }

        public async Task<bool> IsNeighbourhoodSectionVisible()
        {
            await _page.WaitForSelectorAsync(PropertyPageObjects.NeighbourhoodSection);
            return await _page.IsVisibleAsync(PropertyPageObjects.NeighbourhoodSection);
        }

        public async Task<bool> IsMenuVisible(string menuName)
        {
            return await _page.IsVisibleAsync(PropertyPageObjects.GetMenuLocator(menuName));
        }
    }
}
