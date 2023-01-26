using AdsScrapper.CarAds.Common;
using AdsScrapper.Common;
using AdsScrapper.Common.Enums;
using HtmlAgilityPack;

namespace AdsScrapper.Sniffer;

public static class SnifferCarAds
{
    public static void GetAds(string carUrl)
    {
        var loadedDocument = CommonMethods.GetDocument(carUrl);

        var carAdsWrappers = GetAdWrappers(loadedDocument);

        if (!carAdsWrappers.Any())
        {
            CommonMethods.WriteNoAdsToFile(AdType.Sniffer, loadedDocument);
            return;
        }

        var carAds = ExtractAds(carAdsWrappers);

        WriteToFile(carAds);
    }

    // Get car ads wrappers
    // Car ads will be extracted from wrappers
    private static List<HtmlNode> GetAdWrappers(HtmlDocument doc)
    {
        var finalAdWrappers = new List<HtmlNode>();

        var regularAdWrappers = doc.DocumentNode
            .Descendants("li")
            .Where(d => d.Attributes["class"].Value
                .Contains("EntityList-item EntityList-item--Regular"))?.ToList();

        if (regularAdWrappers == null || !regularAdWrappers.Any())
        {
            return new List<HtmlNode>();
        }

        var featuredAdWrappers = doc.DocumentNode
            .Descendants("li")
            .Where(d => d.Attributes["class"].Value
                .Contains("EntityList-item EntityList-item--VauVau"))?.ToList();

        if (featuredAdWrappers != null && featuredAdWrappers.Any())
        {
            finalAdWrappers.AddRange(featuredAdWrappers);
        }

        finalAdWrappers.AddRange(regularAdWrappers);

        return finalAdWrappers;
    }

    // Extract car ads from car ads wrappers
    private static List<HtmlNode> ExtractAds(List<HtmlNode> carAdsWrappers)
    {
        var carAds = new List<HtmlNode>();

        foreach (var wrapper in carAdsWrappers)
        {
            var wrapperCarAds = wrapper
                .Descendants("article")
                .Where(d => d.Attributes["class"].Value
                    .Contains("entity-body cf")).ToList();

            carAds.AddRange(wrapperCarAds);
        }

        return carAds;
    }

    // Write car ads to .txt file. 
    private static void WriteToFile(List<HtmlNode> carAds)
    {
        var filePath = AdType.Sniffer + "_" +
                       DateTime.Now.ToString("ddMMyyyyHHmmss") + ".txt";

        using var w = File.AppendText(filePath);

        foreach (var ad in carAds)
        {
            var titleText = ad
                .Descendants("a")
                .FirstOrDefault()?.InnerText;

            var description = ad
                .Descendants("div")
                .FirstOrDefault(d => d.Attributes["class"].Value
                    .Contains("entity-description-main"))
                ?.InnerText;

            var formattedDescription = description?.Trim();

            var carAd = $"Naziv: {titleText}, Opis: {formattedDescription} ";

            w.WriteLine(carAd);
        }
    }
}