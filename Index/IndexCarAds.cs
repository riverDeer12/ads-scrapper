using System.Reflection.Metadata;
using AdsScrapper.Common;
using AdsScrapper.Common.Enums;
using HtmlAgilityPack;

namespace AdsScrapper.Index;

public static class IndexCarAds
{
    public static void GetAds(string carUrl)
    {
        var doc = CommonMethods.GetDocument(carUrl);

        if (!RootElementCheck(doc))
        {
            CommonMethods.WriteNoAdsToFile(AdType.Index, doc);
            return;
        }

        var carAds = ExtractAds(doc);

        WriteToFile(carAds);
    }

    // Check if root element for ads exists
    // In most cases there is situation where
    // IP is blocked so HTMl content is not loaded properly.
    private static bool RootElementCheck(HtmlDocument doc)
    {
        try
        {
            var rootElement = doc.DocumentNode.Descendants("div")
                .FirstOrDefault(x => x.HasClass("results"));

            return rootElement != null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return false;
        }
    }

    // Extract car ads from car ads wrappers
    private static List<HtmlNode> ExtractAds(HtmlDocument doc)
    {
        var rootElement = doc.DocumentNode.Descendants("div")
            .FirstOrDefault(x => x.HasClass("results"));

        if (rootElement == null) return new List<HtmlNode>();

        return rootElement
            .Descendants("div")
            .Where(x => x.HasClass("OglasiRezHolder"))
            .ToList();
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