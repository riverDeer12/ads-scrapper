using System.Configuration;
using AdsScrapper.CarAds.Common;
using AdsScrapper.CarAds.Common.Enums;
using HtmlAgilityPack;

namespace AdsScrapper.CarAds.Index;

public static class IndexCarAds
{
    private static CarType _carType;

    private static readonly string CarUrl = ConfigurationManager.AppSettings["Index" + _carType]!;

    public static void GetAds(CarType carType)
    {
        _carType = carType;

        var loadedDocument = CommonMethods.GetDocument(CarUrl);

        if (!RootElementCheck(loadedDocument))
        {
            CommonMethods.WriteNoAdsToFile(AdType.Index, loadedDocument);
            return;
        }

        var carAds = ExtractAds(loadedDocument);

        WriteToFile(carAds);
    }

    /**
     * Check if root element for ads exists
     * In most cases there is situation where
     * IP is blocked so HTMl content is not loaded properly.
     */
    private static bool RootElementCheck(HtmlDocument doc)
    {
        try
        {
            var rootElement = doc.DocumentNode
                .Descendants("div")
                .FirstOrDefault(x => x.HasClass("results"));

            return rootElement != null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return false;
        }
    }

    /**
     * Extract car ads from car ads wrappers.
     */
    private static List<HtmlNode> ExtractAds(HtmlDocument doc)
    {
        var rootElement = doc.DocumentNode
            .Descendants("div")
            .FirstOrDefault(x => x.HasClass("results"));

        if (rootElement == null) return new List<HtmlNode>();

        return rootElement
            .Descendants("div")
            .Where(x => x.HasClass("OglasiRezHolder"))
            .ToList();
    }

    /**
     * Write car ads to .txt file.
     */ 
    private static void WriteToFile(List<HtmlNode> carAds)
    {
        var filePath = CommonMethods.GenerateFilePath(AdType.Index, _carType);

        using var w = File.AppendText(filePath);

        var adTranslator = new IndexCarAdTranslator(carAds);

        var carAd = adTranslator.GetJson();

        w.WriteLine(carAd);
    }
}