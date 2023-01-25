using AdsScrapper.Common;
using HtmlAgilityPack;

namespace AdsScrapper.Sniffer;

public static class SnifferCarAds
{
    public static void GetAds(string carUrl)
    {
        var doc = CommonMethods.GetDocument(carUrl);

        var carAdsWrappers = GetAdWrappers(doc);

        var carAds = ExtractAds(carAdsWrappers);

        WriteToFile(carAds);
    }

    // Get car ads wrappers
    // Car ads will be extracted from wrappers
    private static List<HtmlNode> GetAdWrappers(HtmlDocument doc)
    {
        var finalAdWrappers = new List<HtmlNode>();

        var featuredAdWrappers = doc.DocumentNode
            .Descendants("li")
            .Where(d => d.Attributes["class"].Value
                .Contains("EntityList-item EntityList-item--VauVau"));

        var regularAdWrappers = doc.DocumentNode
            .Descendants("li")
            .Where(d => d.Attributes["class"].Value
                .Contains("EntityList-item EntityList-item--Regular")).ToList();

        finalAdWrappers.AddRange(featuredAdWrappers);
        
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
        var timestamp = DateTime.Now.ToString("ddMMyyyyHHmmss");

        var filePath = "njuskalo_oglasi_" + timestamp + ".txt";

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