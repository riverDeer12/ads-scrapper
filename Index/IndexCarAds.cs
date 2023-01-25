using AdsScrapper.Common;
using HtmlAgilityPack;

namespace AdsScrapper.Index;

public static class IndexCarAds
{
    public static void GetAds(string carUrl)
    {
        var doc = CommonMethods.GetDocument(carUrl);

        var carAdsWrappers = GetCarAdsWrappers(doc);

        var carAds = ExtractCarAds(carAdsWrappers);

        WriteCarAdsToFile(carAds);
    }
    
    // Get car ads wrappers
    // Car ads will be extracted from wrappers
    private static List<HtmlNode> GetCarAdsWrappers(HtmlDocument doc)
    {
        return doc.DocumentNode
            .Descendants("li")
            .Where(d => d.Attributes["class"].Value
                .Contains("EntityList-item EntityList-item--Regular")).ToList();
    }

    // Extract car ads from car ads wrappers
    private static List<HtmlNode> ExtractCarAds(List<HtmlNode> carAdsWrappers)
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
    private static void WriteCarAdsToFile(List<HtmlNode> carAds)
    {
        var filePath = "index_oglasi_" + DateTime.Now.ToString("ddMMyyyy") + ".txt";

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