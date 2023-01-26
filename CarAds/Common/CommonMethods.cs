using AdsScrapper.CarAds.Common.Enums;
using HtmlAgilityPack;

namespace AdsScrapper.CarAds.Common;

public static class CommonMethods
{
    // Make request to wanted html document file.
    public static HtmlDocument GetDocument(string url)
    {
        var web = new HtmlWeb();
        var doc = web.Load(url);
        return doc;
    }

    // Create file with no ads description.
    public static void WriteNoAdsToFile(AdType adType, HtmlDocument loadedDocument)
    {
        var filePath = adType + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".json";
        using var w = File.AppendText(filePath);
        w.WriteLine("NO DATA. Loaded document below...");
        w.WriteLine(loadedDocument.ParsedText);
    }
}