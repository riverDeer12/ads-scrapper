using AdsScrapper.Common;
using AdsScrapper.Common.Enums;
using HtmlAgilityPack;

namespace AdsScrapper.Index;

public static class IndexCarAds
{
    public static void GetAds(string carUrl)
    {
        var doc = CommonMethods.GetDocument(carUrl);

        var carAdsWrappers = GetAdsWrappers(doc);

        if (!carAdsWrappers.Any())
        {
            CommonMethods.WriteNoAdsToFile(AdType.Index);
            return;
        }

        var carAds = ExtractAds(carAdsWrappers);

        WriteToFile(carAds);
    }

    // Get car ads wrappers
    // Car ads will be extracted from wrappers
    private static List<HtmlNode> GetAdsWrappers(HtmlDocument doc)
    {
        var adsWrappers = doc.DocumentNode
            .Descendants("div")
            .Where(d => d.Attributes["class"].Value
                .Contains("results"))?.ToList()
            .ToList();

        if (adsWrappers == null || !adsWrappers.Any())
        {
            return new List<HtmlNode>();
        }

        return adsWrappers;
    }

    // Extract car ads from car ads wrappers
    private static List<HtmlNode> ExtractAds(List<HtmlNode> carAdsWrappers)
    {
        var carAds = new List<HtmlNode>();

        foreach (var wrapper in carAdsWrappers)
        {
            var wrapperCarAds = wrapper
                .Descendants("div")
                .Where(d => d.Attributes["class"].Value
                    .Contains("OglasiRezHolder")).ToList();

            carAds.AddRange(wrapperCarAds);
        }

        return carAds;
    }

    // Write car ads to .txt file. 
    private static void WriteToFile(List<HtmlNode> carAds)
    {
        var filePath = AdType.Index + "_" +
                       DateTime.Now.ToString("ddMMyyyyHHmmss") + ".txt";

        using var w = File.AppendText(filePath);

        foreach (var ad in carAds)
        {
            var titleText = ad
                .Descendants("span").FirstOrDefault(d => d.Attributes["class"].Value
                    .Contains("title"))?.InnerText;

            var description = ad
                .Descendants("ul")
                .FirstOrDefault(d => d.Attributes["class"].Value
                    .Contains("tags hide-on-small-only"))
                ?.InnerText;

            var formattedDescription = description?.Trim();

            var carAd = $"Naziv: {titleText}, Opis: {formattedDescription} ";

            w.WriteLine(carAd);
        }
    }
}