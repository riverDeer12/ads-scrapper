using AdsScrapper.Common.Enums;
using HtmlAgilityPack;

namespace AdsScrapper.Common;

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
    public static void WriteNoAdsToFile(AdType adType)
    {
        var filePath = adType + "_" + DateTime.Now.ToString("ddMMyyyy") + ".txt";
        using var w = File.AppendText(filePath);
        w.WriteLine("NO DATA.");
    }
}