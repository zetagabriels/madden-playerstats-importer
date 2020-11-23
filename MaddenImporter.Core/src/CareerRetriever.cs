using AngleSharp;

namespace MaddenImporter.Core
{
    public class CareerRetriever
    {
        private IBrowsingContext browser;

        public CareerRetriever(IBrowsingContext br = null)
        {
            browser = br ?? Extensions.GetDefaultBrowser();
        }

        private static string GetCareerUrl(int year, PlayerType playerType) => "";
    }
}
