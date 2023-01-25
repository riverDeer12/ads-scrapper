using HtmlAgilityPack;

namespace AdsScrapper.Common;

public static class CommonMethods
{
    // Make request to wanted html document file
    public static HtmlDocument GetDocument(string url)
    {
        var web = new HtmlWeb();
        var doc = web.Load(url);
        return doc;
    }
}