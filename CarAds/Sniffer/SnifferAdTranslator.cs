using HtmlAgilityPack;
using Newtonsoft.Json;

namespace AdsScrapper.CarAds.Sniffer;

public class SnifferAdTranslator
{
    private readonly List<CarAd> _carAds = new();
    private readonly List<HtmlNode> _loadedAds;

    public SnifferAdTranslator(List<HtmlNode> loadedAds)
    {
        _loadedAds = loadedAds;

        ProcessAds();
    }

    /**
     * Convert car ad into
     * JSON object.
     */
    public string GetJson() =>
        JsonConvert.SerializeObject(_carAds);

    /*
     * Process incoming HTML ad document
     * into class properties.
     */
    private void ProcessAds()
    {
        foreach (var ad in _loadedAds)
        {
            var carAd = new CarAd();

            carAd.Title = ad
                .Descendants("span")
                .FirstOrDefault(d => d.HasClass("title"))
                ?.InnerText
                .Trim();

            var header = ad
                .Descendants("ul")
                .FirstOrDefault(d => d.HasClass("tags hide-on-small-only"));

            if (header == null)
            {
                _carAds.Add(carAd);
                continue;
            }

            var description = header.ChildNodes.Descendants("ul").FirstOrDefault();

            if (description == null)
            {
                _carAds.Add(carAd);
                continue;
            }

            var descriptionItems = description.Descendants("li").ToList();

            carAd.Year = descriptionItems[0].InnerText;
            carAd.Mileage = descriptionItems[1].InnerText;
            carAd.Power = descriptionItems[3].InnerText;
            carAd.Link = GetLink(ad);

            _carAds.Add(carAd);
        }
    }

    /**
     * Get car ad link for additional
     * info about ad.
     */
    private static string? GetLink(HtmlNode ad)
    {
        var carLinkElement = ad
            .Descendants("a")
            .FirstOrDefault(x => x.HasClass("result"));

        return carLinkElement == null ? string.Empty : carLinkElement.Attributes["href"].Value;
    }
}