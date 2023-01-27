using AdsScrapper.CarAds.Common;
using AdsScrapper.CarAds.Common.Enums;
using HtmlAgilityPack;

namespace AdsScrapper.CarAds.Sniffer;

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
            .Where(d => d.HasClass("EntityList-item EntityList-item--Regular"))?.ToList();

        if (regularAdWrappers == null || !regularAdWrappers.Any())
        {
            return new List<HtmlNode>();
        }

        var featuredAdWrappers = doc.DocumentNode
            .Descendants("li")
            .Where(d => d.HasClass("EntityList-item EntityList-item--VauVau"))?.ToList();

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
                .Where(d => d.HasClass("entity-body cf"))?.ToList();

            if (wrapperCarAds != null) carAds.AddRange(wrapperCarAds);
        }

        return carAds;
    }

    // Write car ads to .txt file. 
    private static void WriteToFile(List<HtmlNode> carAds)
    {
        var filePath = AdType.Sniffer + "/" + AdType.Sniffer + "_" +
                       DateTime.Now.ToString("ddMMyyyyHHmmss") + ".json";

        using var w = File.AppendText(filePath);

        foreach (var ad in carAds)
        {
            var titleText = ad
                .Descendants("a")
                .FirstOrDefault()?.InnerText;

            var description = ad
                .Descendants("div")
                .FirstOrDefault(x => x.HasClass("entity-description-main"))
                ?.InnerText;

            var formattedDescription = description?.Trim();

            var carAd = $"Naziv: {titleText}, Opis: {formattedDescription} ";

            w.WriteLine(carAd);
        }
    }
}